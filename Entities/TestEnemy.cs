//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;

//namespace Project_Game.Entities
//{
//    public class TestEnemy : Enemy
//    {
//        private AnimationManager movementAnimation;
//        private AnimationManager attackAnimation;
//        private Dictionary<string, AnimationManager> idleAnimations;
//        private AnimationManager deadAnimation;

//        private bool isDead = false;
//        private string currentAnimationDirection = "";
//        private bool isMoving = false;
//        private string baseFolderPath;

//        public override bool IsAttacking { get; protected set; } = false;
//        public override bool ShouldRemove { get; protected set; } = false;
//        public  int AttackRange { get; private set; } = 26; // Bạn có thể điều chỉnh giá trị này
//        public  int DetectionRange { get; private set; } = 250;

//        private int attackCooldown = 500; // Thời gian nghỉ giữa các đòn tấn công (ms)
//        private DateTime lastAttackTime = DateTime.MinValue;

//        public TestEnemy(string baseFolder, int maxHealth = 50, int startX = 500, int startY = 100)
//            : base("TestEnemy", maxHealth)
//        {
//            Speed = 3;
//            X = startX;
//            Y = startY;
//            baseFolderPath = baseFolder;

//            movementAnimation = new AnimationManager(frameRate: 4);
//            attackAnimation = new AnimationManager(frameRate: 3);
//            idleAnimations = new Dictionary<string, AnimationManager>();

//            LoadEnemyImages(Path.Combine(baseFolderPath, "Movement"));
//            LoadIdleAnimations(Path.Combine(baseFolderPath, "Idle"));
//            LoadDeadAnimation(Path.Combine(baseFolderPath, "Dead"));
//        }

//        public override void LoadEnemyImages(string baseFolderPath)
//        {
//            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Down"));
//            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Up"));
//            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Left"));
//            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Right"));
//        }

//        private void LoadIdleAnimations(string baseFolderPath)
//        {
//            string[] directions = { "Down", "Up", "Left", "Right" };
//            foreach (var dir in directions)
//            {
//                var anim = new AnimationManager(frameRate: 12);
//                anim.LoadFrames(Path.Combine(baseFolderPath, dir));
//                idleAnimations[dir] = anim;
//            }
//        }

//        private void LoadDeadAnimation(string baseFolderPath)
//        {
//            deadAnimation = new AnimationManager(frameRate: 13);
//            deadAnimation.LoadFrames(baseFolderPath);
//        }

//        // Override HandleAttack để nhận obstacles
//        public override void HandleAttack(Player target, List<GameObject> obstacles)
//        {
//            if (IsDead()) return;

//            double distance = Math.Sqrt((target.playerX - X) * (target.playerX - X) + (target.playerY - Y) * (target.playerY - Y));

//            if (distance <= AttackRange)
//            {
//                if (!IsAttacking)
//                {
//                    PerformAttack(target);
//                }
//            }
//            else if (distance <= DetectionRange)
//            {
//                // Gọi Move với obstacles thực tế
//                Move(target.playerX, target.playerY, 2000, 6000, obstacles);
//            }
//            else
//            {
//                isMoving = false;
//            }
//        }

//        public void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
//        {
//            if (isDead || IsAttacking) return;

//            int deltaX = playerX - X;
//            int deltaY = playerY - Y;
//            int absDeltaX = Math.Abs(deltaX);
//            int absDeltaY = Math.Abs(deltaY);

//            const int directionThreshold = 5;
//            if (absDeltaX < directionThreshold && absDeltaY < directionThreshold)
//            {
//                isMoving = false;
//                return;
//            }

//            const int stableThreshold = 20;
//            string newDirection = currentAnimationDirection;

//            if (absDeltaX > absDeltaY + stableThreshold)
//            {
//                newDirection = (deltaX < 0) ? "Left" : "Right";
//            }
//            else if (absDeltaY > absDeltaX + stableThreshold)
//            {
//                newDirection = (deltaY < 0) ? "Up" : "Down";
//            }
//            else
//            {
//                newDirection = currentAnimationDirection;
//            }

//            if (newDirection != currentAnimationDirection)
//            {
//                currentAnimationDirection = newDirection;
//                movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Movement", currentAnimationDirection));
//            }

//            Direction = currentAnimationDirection;

//            int stepX = 0;
//            int stepY = 0;

//            if (currentAnimationDirection == "Left" || currentAnimationDirection == "Right")
//            {
//                stepX = Math.Sign(deltaX) * Speed;
//            }

//            if (currentAnimationDirection == "Up" || currentAnimationDirection == "Down")
//            {
//                stepY = Math.Sign(deltaY) * Speed;
//            }

//            bool moved = false;

//            // Kiểm tra va chạm trục X
//            if (stepX != 0 && !CheckCollisionWithObstacles(X + stepX, Y, obstacles))
//            {
//                X += stepX;
//                moved = true;
//                Console.WriteLine($"Enemy di chuyển X tới ({X}, {Y})");
//            }
//            else
//            {
//                Console.WriteLine($"Enemy không thể di chuyển X tới ({X + stepX}, {Y}) do va chạm.");
//            }

//            // Kiểm tra va chạm trục Y
//            if (stepY != 0 && !CheckCollisionWithObstacles(X, Y + stepY, obstacles))
//            {
//                Y += stepY;
//                moved = true;
//                Console.WriteLine($"Enemy di chuyển Y tới ({X}, {Y})");
//            }
//            else
//            {
//                Console.WriteLine($"Enemy không thể di chuyển Y tới ({X}, {Y + stepY}) do va chạm.");
//            }

//            isMoving = moved;
//            if (isMoving)
//            {
//                movementAnimation.UpdateAnimation();
//            }
//        }


//        public override void PerformAttack(Player target)
//        {
//            if (!IsAttacking && !isDead)
//            {
//                var now = DateTime.Now;
//                if ((now - lastAttackTime).TotalMilliseconds >= attackCooldown)
//                {
//                    IsAttacking = true;
//                    lastAttackTime = now;
//                    UpdateDirectionFacingPlayer(target.playerX, target.playerY);

//                    attackAnimation.LoadFrames(Path.Combine(baseFolderPath, "Attack", Direction));
//                    attackAnimation.ResetAnimation();

//                    target.TakeDamage(1); // Tăng giá trị nếu cần
//                    Console.WriteLine($"{Name} đã tấn công Player!");
//                }
//            }
//        }

//        public override void UpdateAttack()
//        {
//            if (IsAttacking)
//            {
//                attackAnimation.UpdateAnimation(loop: false);

//                if (attackAnimation.IsComplete())
//                {
//                    IsAttacking = false;
//                }
//            }
//        }

//        private void UpdateDirectionFacingPlayer(int playerX, int playerY)
//        {
//            int deltaX = playerX - X;
//            int deltaY = playerY - Y;

//            int absDeltaX = Math.Abs(deltaX);
//            int absDeltaY = Math.Abs(deltaY);

//            if (absDeltaX > absDeltaY)
//            {
//                Direction = deltaX < 0 ? "Left" : "Right";
//            }
//            else
//            {
//                Direction = deltaY < 0 ? "Up" : "Down";
//            }

//            if (Direction != currentAnimationDirection)
//            {
//                currentAnimationDirection = Direction;
//                movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Movement", Direction));
//            }
//        }

//        public override void TakeDamage(int damage)
//        {
//            base.TakeDamage(damage);
//            if (Health <= 0 && !isDead)
//            {
//                isDead = true;
//                deadAnimation.ResetAnimation();
//            }
//        }

//        public override Image GetCurrentFrame()
//        {
//            if (isDead)
//            {
//                if (!deadAnimation.IsComplete())
//                {
//                    deadAnimation.UpdateAnimation(loop: false);
//                    return deadAnimation.GetCurrentFrame();
//                }
//                else
//                {
//                    ShouldRemove = true;
//                    return null;
//                }
//            }

//            if (IsAttacking)
//            {
//                return attackAnimation.GetCurrentFrame();
//            }

//            if (!IsAttacking && !isDead && !isMoving)
//            {
//                if (idleAnimations.ContainsKey(Direction))
//                {
//                    idleAnimations[Direction].UpdateAnimation();
//                    return idleAnimations[Direction].GetCurrentFrame();
//                }
//            }

//            return movementAnimation.GetCurrentFrame();
//        }

//        public static List<TestEnemy> CreateEnemies(string baseFolder, int count, int startX, int startY)
//        {
//            var enemies = new List<TestEnemy>();
//            Random rand = new Random();
//            for (int i = 0; i < count; i++)
//            {
//                int x = startX + rand.Next(-100, 100);
//                int y = startY + rand.Next(-100, 100);
//                var enemy = new TestEnemy(baseFolder, 50, x, y);
//                enemies.Add(enemy);
//            }
//            return enemies;
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class TestEnemy : Enemy
    {
        private AnimationManager movementAnimation;
        private AnimationManager attackAnimation;
        private Dictionary<string, AnimationManager> idleAnimations;
        private AnimationManager deadAnimation;

        private bool isDead = false;
        private string currentAnimationDirection = "";
        private bool isMoving = false;
        private string baseFolderPath;

        public override bool IsAttacking { get; protected set; } = false;
        public override bool ShouldRemove { get; protected set; } = false;

        // Các thuộc tính công cộng
        public int AttackRange { get; private set; } = 26; // Tăng AttackRange
        public int DetectionRange { get; private set; } = 250; // Tăng DetectionRange

        private int attackCooldown = 1000; // Thời gian nghỉ giữa các đòn tấn công (ms)
        private DateTime lastAttackTime = DateTime.MinValue;

        // Thuộc tính mới để quản lý hướng di chuyển
        private List<string> possibleDirections = new List<string> { "Left", "Right", "Up", "Down" };
        private string previousDirection = "";
        private int directionChangeCooldown = 500; // Thời gian chờ trước khi thay đổi hướng (ms)
        private DateTime lastDirectionChangeTime = DateTime.MinValue;

        public TestEnemy(string baseFolder, int maxHealth = 50, int startX = 500, int startY = 100)
            : base("TestEnemy", maxHealth)
        {
            Speed = 5; // Tăng tốc độ di chuyển
            X = startX;
            Y = startY;
            baseFolderPath = baseFolder;

            movementAnimation = new AnimationManager(frameRate: 1); // Giảm frame rate để tăng tốc độ
            attackAnimation = new AnimationManager(frameRate: 2);
            idleAnimations = new Dictionary<string, AnimationManager>();

            LoadEnemyImages(Path.Combine(baseFolderPath, "Movement"));
            LoadIdleAnimations(Path.Combine(baseFolderPath, "Idle"));
            LoadDeadAnimation(Path.Combine(baseFolderPath, "Dead"));
        }

        public override void LoadEnemyImages(string baseFolderPath)
        {
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Down"));
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Up"));
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Left"));
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Right"));
        }

        private void LoadIdleAnimations(string baseFolderPath)
        {
            string[] directions = { "Down", "Up", "Left", "Right" };
            foreach (var dir in directions)
            {
                var anim = new AnimationManager(frameRate: 12);
                anim.LoadFrames(Path.Combine(baseFolderPath, dir));
                idleAnimations[dir] = anim;
            }
        }

        private void LoadDeadAnimation(string baseFolderPath)
        {
            deadAnimation = new AnimationManager(frameRate: 13);
            deadAnimation.LoadFrames(baseFolderPath);
        }

        // Override HandleAttack để nhận obstacles
        public override void HandleAttack(Player target, List<GameObject> obstacles)
        {
            if (IsDead()) return;

            int deltaX = target.playerX - X;
            int deltaY = target.playerY - Y;
            int distanceSquared = deltaX * deltaX + deltaY * deltaY;

            if (distanceSquared <= AttackRange * AttackRange)
            {
                if (!IsAttacking)
                {
                    PerformAttack(target);
                }
            }
            else if (distanceSquared <= DetectionRange * DetectionRange)
            {
                // Gọi Move với obstacles thực tế
                Move(target.playerX, target.playerY, 2000, 6000, obstacles);
            }
            else
            {
                isMoving = false;
            }
        }

        public void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            if (isDead || IsAttacking) return;

            int deltaX = playerX - X;
            int deltaY = playerY - Y;
            int absDeltaX = Math.Abs(deltaX);
            int absDeltaY = Math.Abs(deltaY);

            const int directionThreshold = 5; // Điều chỉnh để di chuyển khi gần hơn
            if (absDeltaX < directionThreshold && absDeltaY < directionThreshold)
            {
                isMoving = false;
                return;
            }

            const int stableThreshold = 20;
            string newDirection = currentAnimationDirection;

            if (absDeltaX > absDeltaY + stableThreshold)
            {
                newDirection = (deltaX < 0) ? "Left" : "Right";
            }
            else if (absDeltaY > absDeltaX + stableThreshold)
            {
                newDirection = (deltaY < 0) ? "Up" : "Down";
            }
            else
            {
                newDirection = currentAnimationDirection;
            }

            if (newDirection != currentAnimationDirection)
            {
                currentAnimationDirection = newDirection;
                movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Movement", currentAnimationDirection));
            }

            Direction = currentAnimationDirection;

            int stepX = 0;
            int stepY = 0;

            if (currentAnimationDirection == "Left" || currentAnimationDirection == "Right")
            {
                stepX = Math.Sign(deltaX) * Speed;
            }

            if (currentAnimationDirection == "Up" || currentAnimationDirection == "Down")
            {
                stepY = Math.Sign(deltaY) * Speed;
            }

            bool moved = false;

            // Kiểm tra va chạm trục X
            if (stepX != 0 && !CheckCollisionWithObstacles(X + stepX, Y, obstacles))
            {
                X += stepX;
                moved = true;
                Console.WriteLine($"Enemy di chuyển X tới ({X}, {Y})");
            }
            else if (stepX != 0)
            {
                Console.WriteLine($"Enemy không thể di chuyển X tới ({X + stepX}, {Y}) vì va chạm");
                // Thay đổi hướng di chuyển khi va chạm trục X
                TryChangeDirection(obstacles);
            }

            // Kiểm tra va chạm trục Y
            if (stepY != 0 && !CheckCollisionWithObstacles(X, Y + stepY, obstacles))
            {
                Y += stepY;
                moved = true;
                Console.WriteLine($"Enemy di chuyển Y tới ({X}, {Y})");
            }
            else if (stepY != 0)
            {
                Console.WriteLine($"Enemy không thể di chuyển Y tới ({X}, {Y + stepY}) vì va chạm");
                // Thay đổi hướng di chuyển khi va chạm trục Y
                TryChangeDirection(obstacles);
            }

            isMoving = moved;
            if (isMoving)
            {
                movementAnimation.UpdateAnimation();
            }
        }

        // Phương thức mới để thay đổi hướng di chuyểnn
        private void TryChangeDirection(List<GameObject> obstacles)
        {
            var now = DateTime.Now;
            if ((now - lastDirectionChangeTime).TotalMilliseconds < directionChangeCooldown)
            {
                // Chưa đủ thời gian để thay đổi hướng
                return;
            }

            lastDirectionChangeTime = now;

            // Tạo danh sách các hướng khả thi (không phải hướng trước đó)
            var availableDirections = new List<string>(possibleDirections);
            if (!string.IsNullOrEmpty(previousDirection))
            {
                availableDirections.Remove(previousDirection);
            }

            // Loại bỏ hướng hiện tại
            if (!string.IsNullOrEmpty(currentAnimationDirection))
            {
                availableDirections.Remove(currentAnimationDirection);
            }

            if (availableDirections.Count == 0)
            {
                // Nếu không còn hướng nào khả thi, giữ nguyên hướng hiện tại
                return;
            }

            // Chọn hướng mới ngẫu nhiên
            var random = new Random();
            string newDirection = availableDirections[random.Next(availableDirections.Count)];

            // Kiểm tra xem hướng mới có thể di chuyển không
            int potentialX = X;
            int potentialY = Y;

            switch (newDirection)
            {
                case "Left":
                    potentialX -= Speed;
                    break;
                case "Right":
                    potentialX += Speed;
                    break;
                case "Up":
                    potentialY -= Speed;
                    break;
                case "Down":
                    potentialY += Speed;
                    break;
            }

            if (!CheckCollisionWithObstacles(potentialX, potentialY, obstacles))
            {
                // Nếu hướng mới không va chạm, cập nhật hướng di chuyển
                currentAnimationDirection = newDirection;
                Direction = newDirection;
                movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Movement", Direction));
                previousDirection = Direction;
                Console.WriteLine($"Enemy thay đổi hướng di chuyển thành {Direction}");
            }
            else
            {
                // Nếu hướng mới vẫn va chạm, thử thay đổi hướng lại sau
                Console.WriteLine($"Enemy không thể thay đổi hướng thành {newDirection} vì va chạm.");
            }
        }

        // Phương thức kiểm tra va chạm với obstacles
        private bool CheckCollisionWithObstacles(int newX, int newY, List<GameObject> obstacles)
        {
            Rectangle newRect = new Rectangle(newX, newY, this.Width, this.Height);
            foreach (var obstacle in obstacles)
            {
                Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
                if (newRect.IntersectsWith(obstacleRect))
                {
                    return true;
                }
            }
            return false;
        }

        public override void PerformAttack(Player target)
        {
            if (!IsAttacking && !isDead)
            {
                var now = DateTime.Now;
                if ((now - lastAttackTime).TotalMilliseconds >= attackCooldown)
                {
                    IsAttacking = true;
                    lastAttackTime = now;
                    UpdateDirectionFacingPlayer(target.playerX, target.playerY);

                    attackAnimation.LoadFrames(Path.Combine(baseFolderPath, "Attack", Direction));
                    attackAnimation.ResetAnimation();

                    target.TakeDamage(5); // Tăng từ 1 lên 5 hoặc giá trị phù hợp
                    Console.WriteLine($"{Name} đã tấn công Player!");
                }
            }
        }

        public override void UpdateAttack()
        {
            if (IsAttacking)
            {
                attackAnimation.UpdateAnimation(loop: false);

                if (attackAnimation.IsComplete())
                {
                    IsAttacking = false;
                    Console.WriteLine($"{Name} hoàn thành tấn công.");
                }
            }
        }

        private void UpdateDirectionFacingPlayer(int playerX, int playerY)
        {
            int deltaX = playerX - X;
            int deltaY = playerY - Y;

            int absDeltaX = Math.Abs(deltaX);
            int absDeltaY = Math.Abs(deltaY);

            if (absDeltaX > absDeltaY)
            {
                Direction = deltaX < 0 ? "Left" : "Right";
            }
            else
            {
                Direction = deltaY < 0 ? "Up" : "Down";
            }

            if (Direction != currentAnimationDirection)
            {
                currentAnimationDirection = Direction;
                movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Movement", Direction));
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (Health <= 0 && !isDead)
            {
                isDead = true;
                deadAnimation.ResetAnimation();
                Console.WriteLine($"{Name} đã chết.");
            }
        }
        public override Image GetCurrentFrame()
        {
            if (isDead)
            {
                if (!deadAnimation.IsComplete())
                {
                    deadAnimation.UpdateAnimation(loop: false);
                    return deadAnimation.GetCurrentFrame();
                }
                else
                {
                    ShouldRemove = true;
                    return null;
                }
            }

            if (IsAttacking)
            {
                return attackAnimation.GetCurrentFrame();
            }

            if (!IsAttacking && !isDead && !isMoving)
            {
                if (idleAnimations.ContainsKey(Direction))
                {
                    idleAnimations[Direction].UpdateAnimation();
                    return idleAnimations[Direction].GetCurrentFrame();
                }
            }

            return movementAnimation.GetCurrentFrame();
        }

        public static List<TestEnemy> CreateEnemies(string baseFolder, int count, int startX, int startY)
        {
            var enemies = new List<TestEnemy>();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                int x = startX + rand.Next(-100, 100);
                int y = startY + rand.Next(-100, 100);
                var enemy = new TestEnemy(baseFolder, 50, x, y);
                enemies.Add(enemy);
            }
            return enemies;
        }
    }
}
