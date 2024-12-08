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

//        private bool isDead = false;
//        private string currentAnimationDirection = "";
//        private bool isMoving = false; // Biến theo dõi trạng thái di chuyển

//        public override bool IsAttacking { get; protected set; } = false;
//        public override bool ShouldRemove { get; protected set; } = false;
//        private int framesSinceLastDirectionChange = 0;
//        private const int AttackRange = 100;
//        private const int DetectionRange = 300; // Phạm vi phát hiện

//        public TestEnemy() : base("TestEnemy", maxHealth: 50)
//        {
//            Speed = 3;
//            movementAnimation = new AnimationManager(frameRate: 5);
//            attackAnimation = new AnimationManager(frameRate: 4);
//            idleAnimations = new Dictionary<string, AnimationManager>();

//            LoadEnemyImages("Char_MoveMent");
//            LoadIdleAnimations("Char_Idle");
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
//                var anim = new AnimationManager(frameRate: 8);
//                anim.LoadFrames(Path.Combine(baseFolderPath, dir));
//                idleAnimations[dir] = anim;
//            }
//        }

//        public override void HandleAttack(Player target)
//        {
//            double distance = Math.Sqrt(Math.Pow(target.playerX - X, 2) + Math.Pow(target.playerY - Y, 2));

//            if (distance <= AttackRange)
//            {
//                // Gần đủ để tấn công
//                if (!IsAttacking)
//                {
//                    PerformAttack(target);
//                }
//            }
//            else if (distance <= DetectionRange)
//            {
//                // Trong phạm vi phát hiện, nhưng không đủ gần để tấn công, đuổi theo
//                Move(target.playerX, target.playerY, 2000, 6000, new List<GameObject>());
//            }
//            else
//            {
//                // Player ở quá xa, không di chuyển
//                isMoving = false;
//                // Có thể reset animation chuyển động nếu muốn
//                // movementAnimation.ResetAnimation();
//            }
//        }

//        public override void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
//        {
//            if (isDead || IsAttacking) return;

//            int deltaX = playerX - X;
//            int deltaY = playerY - Y;
//            int absDeltaX = Math.Abs(deltaX);
//            int absDeltaY = Math.Abs(deltaY);

//            // Ngưỡng khoảng cách nhỏ
//            const int directionThreshold = 2;
//            if (absDeltaX < directionThreshold && absDeltaY < directionThreshold)
//            {
//                // Quá gần, không đổi hướng, idle
//                isMoving = false;
//                return;
//            }

//            // Ngưỡng chênh lệch để đổi hướng
//            const int stableThreshold = 5;
//            string newDirection = currentAnimationDirection;

//            // Chỉ cho đổi hướng nếu đã qua một số frame nhất định
//            // hoặc nếu chênh lệch hướng rất rõ ràng
//            if (framesSinceLastDirectionChange > 10) // số frame tùy chỉnh
//            {
//                if (absDeltaX - absDeltaY > stableThreshold)
//                {
//                    newDirection = (deltaX < 0) ? "Left" : "Right";
//                }
//                else if (absDeltaY - absDeltaX > stableThreshold)
//                {
//                    newDirection = (deltaY < 0) ? "Up" : "Down";
//                }

//                if (newDirection != currentAnimationDirection)
//                {
//                    currentAnimationDirection = newDirection;
//                    movementAnimation.LoadFrames($"Char_MoveMent/{currentAnimationDirection}");
//                    framesSinceLastDirectionChange = 0; // reset khi đổi hướng
//                }
//            }

//            // Dù không đổi hướng, ta vẫn có thể di chuyển
//            int stepX = Math.Sign(deltaX) * Speed;
//            int stepY = Math.Sign(deltaY) * Speed;

//            bool moved = false;

//            if (!CheckCollisionWithObstacles(X + stepX, Y, obstacles))
//            {
//                X += stepX;
//                moved = true;
//            }

//            if (!CheckCollisionWithObstacles(X, Y + stepY, obstacles))
//            {
//                Y += stepY;
//                moved = true;
//            }

//            isMoving = moved;
//            if (isMoving)
//            {
//                movementAnimation.UpdateAnimation();
//            }

//            // Tăng khung đếm
//            framesSinceLastDirectionChange++;
//        }


//        public override void PerformAttack(Player target)
//        {
//            if (!IsAttacking)
//            {
//                IsAttacking = true;
//                UpdateDirectionFacingPlayer(target.playerX, target.playerY);

//                attackAnimation.LoadFrames($"Enemy_Attack/{Direction}");
//                attackAnimation.ResetAnimation();

//                target.TakeDamage(10);
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

//            if (Math.Abs(deltaX) > Math.Abs(deltaY))
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
//                movementAnimation.LoadFrames($"Char_MoveMent/{Direction}");
//            }
//        }

//        public override Image GetCurrentFrame()
//        {
//            if (IsAttacking)
//            {
//                return attackAnimation.GetCurrentFrame();
//            }

//            // Nếu không tấn công, không chết và không di chuyển => idle
//            if (!IsAttacking && !isDead && !isMoving)
//            {
//                if (idleAnimations.ContainsKey(Direction))
//                {
//                    // Cập nhật animation idle trước khi lấy frame
//                    idleAnimations[Direction].UpdateAnimation();
//                    return idleAnimations[Direction].GetCurrentFrame();
//                }
//            }

//            // Nếu vẫn còn di chuyển thì trả về frame di chuyển
//            return movementAnimation.GetCurrentFrame();
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

        private bool isDead = false;
        private string currentAnimationDirection = "";
        private bool isMoving = false; // Theo dõi trạng thái di chuyển

        public override bool IsAttacking { get; protected set; } = false;
        public override bool ShouldRemove { get; protected set; } = false;

        private const int AttackRange = 100;
        private const int DetectionRange = 300; // Phạm vi phát hiện

        // Biến đếm frame để hạn chế tần suất đổi hướng
        private int framesSinceLastDirectionChange = 0;
        // Số khung hình chờ giữa hai lần đổi hướng
        private const int framesBetweenDirectionChanges = 10;

        public TestEnemy() : base("TestEnemy", maxHealth: 50)
        {
            Speed = 3;
            movementAnimation = new AnimationManager(frameRate: 5);
            attackAnimation = new AnimationManager(frameRate: 4);
            idleAnimations = new Dictionary<string, AnimationManager>();

            LoadEnemyImages("Char_MoveMent");
            LoadIdleAnimations("Char_Idle");
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
                var anim = new AnimationManager(frameRate: 8);
                anim.LoadFrames(Path.Combine(baseFolderPath, dir));
                idleAnimations[dir] = anim;
            }
        }

        public override void HandleAttack(Player target)
        {
            double distance = Math.Sqrt(Math.Pow(target.playerX - X, 2) + Math.Pow(target.playerY - Y, 2));

            if (distance <= AttackRange)
            {
                // Gần đủ để tấn công
                if (!IsAttacking)
                {
                    PerformAttack(target);
                }
            }
            else if (distance <= DetectionRange)
            {
                // Đuổi theo player
                Move(target.playerX, target.playerY, 2000, 6000, new List<GameObject>());
            }
            else
            {
                // Player quá xa, không di chuyển
                isMoving = false;
            }
        }

        public override void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            if (isDead || IsAttacking) return;

            int deltaX = playerX - X;
            int deltaY = playerY - Y;

            int absDeltaX = Math.Abs(deltaX);
            int absDeltaY = Math.Abs(deltaY);

            // Ngưỡng để tránh đổi hướng liên tục khi quá gần
            const int directionThreshold = 2;
            // Nếu quá gần player, không đổi hướng
            if (absDeltaX < directionThreshold && absDeltaY < directionThreshold)
            {
                isMoving = false;
                return;
            }

            // Ngưỡng chênh lệch để xác định rõ hướng
            const int stableThreshold = 10;
            string newDirection = currentAnimationDirection;

            // Chỉ cho đổi hướng khi đã qua một số frame nhất định
            if (framesSinceLastDirectionChange > framesBetweenDirectionChanges)
            {
                // Kiểm tra xem có sự chênh lệch rõ ràng không
                if (absDeltaX - absDeltaY > stableThreshold)
                {
                    // Chênh lệch theo trục X rõ ràng
                    newDirection = (deltaX < 0) ? "Left" : "Right";
                }
                else if (absDeltaY - absDeltaX > stableThreshold)
                {
                    // Chênh lệch theo trục Y rõ ràng
                    newDirection = (deltaY < 0) ? "Up" : "Down";
                }
                else
                {
                    // Chênh lệch không rõ ràng, giữ nguyên hướng cũ
                    newDirection = currentAnimationDirection;
                }

                // Nếu có đổi hướng, reset bộ đếm
                if (newDirection != currentAnimationDirection)
                {
                    currentAnimationDirection = newDirection;
                    movementAnimation.LoadFrames($"Char_MoveMent/{currentAnimationDirection}");
                    framesSinceLastDirectionChange = 0;
                }
            }

            Direction = currentAnimationDirection;

            int stepX = Math.Sign(deltaX) * Speed;
            int stepY = Math.Sign(deltaY) * Speed;

            bool moved = false;

            if (!CheckCollisionWithObstacles(X + stepX, Y, obstacles))
            {
                X += stepX;
                moved = true;
            }

            if (!CheckCollisionWithObstacles(X, Y + stepY, obstacles))
            {
                Y += stepY;
                moved = true;
            }

            isMoving = moved;
            if (isMoving)
            {
                movementAnimation.UpdateAnimation();
            }

            framesSinceLastDirectionChange++;
        }

        public override void PerformAttack(Player target)
        {
            if (!IsAttacking)
            {
                IsAttacking = true;
                UpdateDirectionFacingPlayer(target.playerX, target.playerY);

                attackAnimation.LoadFrames($"Enemy_Attack/{Direction}");
                attackAnimation.ResetAnimation();

                target.TakeDamage(10);
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
                }
            }
        }

        private void UpdateDirectionFacingPlayer(int playerX, int playerY)
        {
            int deltaX = playerX - X;
            int deltaY = playerY - Y;

            int absDeltaX = Math.Abs(deltaX);
            int absDeltaY = Math.Abs(deltaY);

            // Chọn hướng tấn công tương tự logic di chuyển
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
                movementAnimation.LoadFrames($"Char_MoveMent/{Direction}");
            }
        }

        public override Image GetCurrentFrame()
        {
            if (IsAttacking)
            {
                return attackAnimation.GetCurrentFrame();
            }

            // Nếu không tấn công, không chết và không di chuyển => idle
            if (!IsAttacking && !isDead && !isMoving)
            {
                if (idleAnimations.ContainsKey(Direction))
                {
                    idleAnimations[Direction].UpdateAnimation();
                    return idleAnimations[Direction].GetCurrentFrame();
                }
            }

            // Nếu vẫn còn di chuyển thì trả về frame di chuyển
            return movementAnimation.GetCurrentFrame();
        }
    }
}
