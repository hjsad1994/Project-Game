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
        private AnimationManager preDeadAnimation; // Thêm AnimationManager cho Pre-Dead
        private AnimationManager deadAnimation;

        private bool isDead = false;
        private bool isPreDead = false; // Cờ để xác định trạng thái Pre-Dead
        private bool hasDroppedItem = false; // Cờ để đảm bảo DropItem chỉ được gọi một lần
        private string currentAnimationDirection = "";
        private bool isMoving = false;
        private string baseFolderPath;

        public override bool IsAttacking { get; protected set; } = false;
        public override bool ShouldRemove { get; protected set; } = false;

        public int AttackRange { get; private set; } = 26;
        public int DetectionRange { get; private set; } = 120;

        private int attackCooldown = 1000;
        private DateTime lastAttackTime = DateTime.MinValue;

        private List<string> possibleDirections = new List<string> { "Left", "Right", "Up", "Down" };
        private int directionChangeCooldown = 1500;
        private DateTime lastDirectionChangeTime = DateTime.MinValue;

        private static Random random = new Random(); // Sử dụng 'random' duy nhất

        // Tốc độ
        private float chaseSpeed = 0.5f;
        private float roamSpeed = 0.3f;

        // Lưu vị trí dưới dạng float để di chuyển mượt mà
        private float posX, posY;

        public TestEnemy(string baseFolder, int maxHealth = 50, int startX = 500, int startY = 100)
            : base("TestEnemy", maxHealth)
        {
            Speed = roamSpeed;
            posX = startX;
            posY = startY;
            baseFolderPath = baseFolder;

            movementAnimation = new AnimationManager(frameRate: 15);
            attackAnimation = new AnimationManager(frameRate: 15);
            idleAnimations = new Dictionary<string, AnimationManager>();

            LoadEnemyImages(Path.Combine(baseFolderPath, "Movement"));
            LoadIdleAnimations(Path.Combine(baseFolderPath, "Idle"));
            LoadPreDeadAnimations(Path.Combine(baseFolderPath, "Pre-Dead")); // Tải Pre-Dead cho từng hướng
            LoadDeadAnimation(Path.Combine(baseFolderPath, "Dead"));
        }

        // Ghi đè X, Y để lấy từ posX, posY với giới hạn
        public override int X
        {
            get { return (int)posX; }
            set
            {
                posX = value;
                // Giới hạn posX trong khoảng từ 0 đến 800
                if (posX < 0) posX = 0;
                if (posX > 800) posX = 800;
            }
        }

        public override int Y
        {
            get { return (int)posY; }
            set
            {
                posY = value;
                // Giới hạn posY trong khoảng từ 0 đến 630
                if (posY < 0) posY = 0;
                if (posY > 630) posY = 630;
            }
        }

        public override void LoadEnemyImages(string baseFolderPath)
        {
            // Tải hoạt ảnh Movement cho từng hướng
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Down"));
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Up"));
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Left"));
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Right"));
            Console.WriteLine($"[Info] Loaded Movement animations from {baseFolderPath}");
        }

        private void LoadIdleAnimations(string baseFolderPath)
        {
            string[] directions = { "Down", "Up", "Left", "Right" };
            foreach (var dir in directions)
            {
                var anim = new AnimationManager(frameRate: 15);
                anim.LoadFrames(Path.Combine(baseFolderPath, dir));
                idleAnimations[dir] = anim;
                Console.WriteLine($"[Info] Loaded Idle animation for direction '{dir}' from {Path.Combine(baseFolderPath, dir)}");
            }
        }

        private void LoadPreDeadAnimations(string preDeadFolderPath)
        {
            // Tải Pre-Dead cho từng hướng
            preDeadAnimation = new AnimationManager(frameRate: 30);
            string[] directions = { "Down", "Up", "Left", "Right" };
            foreach (var dir in directions)
            {
                string dirPath = Path.Combine(preDeadFolderPath, dir);
                preDeadAnimation.LoadFrames(dirPath);
                Console.WriteLine($"[Info] Loaded Pre-Dead animation for direction '{dir}' from {dirPath}");
            }
        }

        private void LoadDeadAnimation(string deadFolderPath)
        {
            deadAnimation = new AnimationManager(frameRate: 30);
            deadAnimation.LoadFrames(deadFolderPath);
            Console.WriteLine($"[Info] Loaded Dead animation from {deadFolderPath}");
        }

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
                // Khi thấy player, tăng tốc
                Speed = chaseSpeed;
                MoveTowardsPlayer(target.playerX, target.playerY, obstacles);
            }
            else
            {
                // Không thấy player -> roam chậm
                Roam(obstacles);
            }
        }

        private void MoveTowardsPlayer(int playerX, int playerY, List<GameObject> obstacles)
        {
            if (isDead || IsAttacking) return;

            int deltaX = playerX - X;
            int deltaY = playerY - Y;
            int distanceSquared = deltaX * deltaX + deltaY * deltaY;

            if (distanceSquared > DetectionRange * DetectionRange)
            {
                Roam(obstacles);
                return;
            }

            int absDeltaX = Math.Abs(deltaX);
            int absDeltaY = Math.Abs(deltaY);

            const int directionThreshold = 5;
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

            SetDirectionAndMove(newDirection, deltaX, deltaY, obstacles);
        }

        private void Roam(List<GameObject> obstacles)
        {
            Speed = roamSpeed;
            var now = DateTime.Now;
            // Chỉ đổi hướng nếu đã đến cooldown
            if (!isMoving && (now - lastDirectionChangeTime).TotalMilliseconds > directionChangeCooldown)
            {
                string newDirection = possibleDirections[random.Next(possibleDirections.Count)];
                ChangeDirection(newDirection);
                lastDirectionChangeTime = now;
            }

            int deltaX = 0;
            int deltaY = 0;
            switch (Direction)
            {
                case "Left":
                    deltaX = -1;
                    break;
                case "Right":
                    deltaX = 1;
                    break;
                case "Up":
                    deltaY = -1;
                    break;
                case "Down":
                    deltaY = 1;
                    break;
            }

            SetDirectionAndMove(Direction, deltaX, deltaY, obstacles, isRoaming: true);
        }

        private void SetDirectionAndMove(string newDirection, int deltaX, int deltaY, List<GameObject> obstacles, bool isRoaming = false)
        {
            if (newDirection != currentAnimationDirection)
            {
                currentAnimationDirection = newDirection;
                ChangeDirection(newDirection);
            }

            // Di chuyển bằng float, không ép kiểu Speed thành int
            float moveX = Math.Sign(deltaX) * Speed;
            float moveY = Math.Sign(deltaY) * Speed;

            bool moved = false;

            // Kiểm tra va chạm X
            if (moveX != 0 && !CheckCollisionWithObstacles((int)(posX + moveX), (int)posY, obstacles))
            {
                posX += moveX;
                moved = true;
            }

            // Kiểm tra va chạm Y
            if (moveY != 0 && !CheckCollisionWithObstacles((int)posX, (int)(posY + moveY), obstacles))
            {
                posY += moveY;
                moved = true;
            }

            // Giới hạn posX và posY sau khi di chuyển
            if (posX < 0)
            {
                posX = 0;
                ChangeDirection("Right"); // Đổi hướng về phải
            }
            if (posX > 800)
            {
                posX = 800;
                ChangeDirection("Left"); // Đổi hướng về trái
            }
            if (posY < 0)
            {
                posY = 0;
                ChangeDirection("Down"); // Đổi hướng xuống dưới
            }
            if (posY > 630)
            {
                posY = 630;
                ChangeDirection("Up"); // Đổi hướng lên trên
            }

            isMoving = moved;
            if (isMoving)
            {
                movementAnimation.UpdateAnimation();
            }
            else
            {
                // Không dịch chuyển được, không lập tức đổi hướng 
                // (Không đặt lại lastDirectionChangeTime)
                // Kẻ địch sẽ đợi hết cooldown mới đổi hướng khác
            }
        }

        private void ChangeDirection(string newDirection)
        {
            Direction = newDirection;
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Movement", Direction));
            Console.WriteLine($"[Info] {Name} đổi hướng thành '{Direction}'");
        }

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

                    target.TakeDamage(5);
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
                Console.WriteLine($"[Info] {Name} đối diện hướng '{Direction}' để tấn công.");
            }
        }

        public override void TakeDamage(int damage)
        {
            if (isDead) return; // Nếu đã chết, không làm gì cả

            base.TakeDamage(damage);

            if (Health <= 0 && !isDead)
            {
                isDead = true;
                isPreDead = true;
                preDeadAnimation.ResetAnimation();
                Console.WriteLine($"{Name} đã chết và bắt đầu trạng thái Pre-Dead.");
            }
        }

        public override Image GetCurrentFrame()
        {
            if (isPreDead)
            {
                // Chạy hoạt ảnh Pre-Dead
                Console.WriteLine($"[Debug] {Name} đang chạy Pre-Dead.");
                if (!preDeadAnimation.IsComplete())
                {
                    preDeadAnimation.UpdateAnimation(loop: false);
                    return preDeadAnimation.GetCurrentFrame();
                }
                else
                {
                    isPreDead = false;
                    deadAnimation.ResetAnimation(); // Bắt đầu hoạt ảnh Dead
                    Console.WriteLine($"{Name} đã hoàn thành Pre-Dead và chuyển sang Dead.");
                }
            }

            if (isDead)
            {
                // Chạy hoạt ảnh Dead
                Console.WriteLine($"[Debug] {Name} đang chạy Dead.");
                if (!deadAnimation.IsComplete())
                {
                    deadAnimation.UpdateAnimation(loop: false);
                    return deadAnimation.GetCurrentFrame();
                }
                else
                {
                    if (!hasDroppedItem)
                    {
                        DropItem(); // Gọi DropItem khi hoàn thành Dead animation
                        hasDroppedItem = true;
                        Console.WriteLine($"{Name} đã rớt item sau khi chết.");
                    }

                    ShouldRemove = true;
                    Console.WriteLine($"{Name} hoàn thành Dead và sẽ bị loại bỏ khỏi trò chơi.");
                    return null;
                }
            }

            if (IsAttacking)
            {
                Console.WriteLine($"[Debug] {Name} đang tấn công.");
                return attackAnimation.GetCurrentFrame();
            }

            if (!IsAttacking && !isDead && !isMoving)
            {
                if (idleAnimations.ContainsKey(Direction))
                {
                    idleAnimations[Direction].UpdateAnimation();
                    Console.WriteLine($"[Debug] {Name} đang Idle với hướng '{Direction}'.");
                    return idleAnimations[Direction].GetCurrentFrame();
                }
            }

            Console.WriteLine($"[Debug] {Name} đang di chuyển với hướng '{Direction}'.");
            return movementAnimation.GetCurrentFrame();
        }

        public static List<TestEnemy> CreateEnemies(string baseFolder, int count, int startX, int startY)
        {
            var enemies = new List<TestEnemy>();
            for (int i = 0; i < count; i++)
            {
                int x = startX + random.Next(-100, 100); // Sử dụng 'random' duy nhất
                int y = startY + random.Next(-100, 100); // Sử dụng 'random' duy nhất
                var enemy = new TestEnemy(baseFolder, 50, x, y);
                enemies.Add(enemy);
            }
            Console.WriteLine($"[Info] Tạo {count} TestEnemy từ thư mục '{baseFolder}' tại vị trí bắt đầu ({startX}, {startY}).");
            return enemies;
        }

        public void Update(List<GameObject> obstacles, Player target)
        {
            if (isDead && !isPreDead) return; // Nếu đã chết và không phải Pre-Dead, không cần cập nhật thêm

            if (isPreDead)
            {
                preDeadAnimation.UpdateAnimation(loop: false);
                // Không thực hiện các hành động khác trong trạng thái Pre-Dead
                return;
            }

            if (isDead)
            {
                deadAnimation.UpdateAnimation(loop: false);
                return;
            }

            if (!IsAttacking)
            {
                movementAnimation.UpdateAnimation();
            }

            HandleAttack(target, obstacles);

            if (IsAttacking)
            {
                UpdateAttack();
            }
        }

        private void DropItem()
        {
            // Tạo một item mới (cập nhật đường dẫn hình ảnh sau)
            string itemName = "Ores_1"; // Ví dụ: đặt tên item
            string itemImagePath = Path.Combine("Assets", "Items", "Ores", "ores_1.png"); // Đường dẫn hình ảnh item

            if (File.Exists(itemImagePath))
            {
                try
                {
                    Image itemIcon = Image.FromFile(itemImagePath);
                    Item droppedItem = new Item(itemName, itemIcon);

                    // Tính toán khoảng cách và hướng ngẫu nhiên
                    double angle = random.NextDouble() * 2 * Math.PI; // Sử dụng 'random' thay vì 'rand'
                    float distance = random.Next(10, 40); // Sử dụng 'random' thay vì 'rand'
                    int offsetX = (int)(Math.Cos(angle) * distance);
                    int offsetY = (int)(Math.Sin(angle) * distance);

                    // Tạo DroppedItem tại vị trí offset
                    DroppedItem newDroppedItem = new DroppedItem(droppedItem, X + offsetX, Y + offsetY);

                    // Thêm DroppedItem vào GameObjectManager
                    GameObjectManager.Instance.AddDroppedItem(newDroppedItem);

                    Console.WriteLine($"{Name} đã rớt ra item: {itemName} tại ({newDroppedItem.X}, {newDroppedItem.Y})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi tạo DroppedItem: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Không tìm thấy hình ảnh cho item: {itemImagePath}");
            }
        }
    }
}
