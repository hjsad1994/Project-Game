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

        private const int AttackRange = 100;
        private const int DetectionRange = 300;

        public TestEnemy(string baseFolder, int maxHealth = 50, int startX = 500, int startY = 100)
            : base("TestEnemy", maxHealth)
        {
            Speed = 3;
            X = startX;
            Y = startY;
            baseFolderPath = baseFolder;

            movementAnimation = new AnimationManager(frameRate: 2);
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

            double distance = Math.Sqrt((target.playerX - X) * (target.playerX - X) + (target.playerY - Y) * (target.playerY - Y));

            if (distance <= AttackRange)
            {
                if (!IsAttacking)
                {
                    PerformAttack(target);
                }
            }
            else if (distance <= DetectionRange)
            {
                // Gọi Move với obstacles thực tế
                Move(target.playerX, target.playerY, 2000, 6000, obstacles);
            }
            else
            {
                isMoving = false;
            }
        }

        public  void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            if (isDead || IsAttacking) return;

            int deltaX = playerX - X;
            int deltaY = playerY - Y;
            int absDeltaX = Math.Abs(deltaX);
            int absDeltaY = Math.Abs(deltaY);

            const int directionThreshold = 2;
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

            // Kiểm tra va chạm trục Y
            if (stepY != 0 && !CheckCollisionWithObstacles(X, Y + stepY, obstacles))
            {
                Y += stepY;
                moved = true;
                Console.WriteLine($"Enemy di chuyển Y tới ({X}, {Y})");
            }

            isMoving = moved;
            if (isMoving)
            {
                movementAnimation.UpdateAnimation();
            }
        }

        public override void PerformAttack(Player target)
        {
            if (!IsAttacking && !isDead)
            {
                IsAttacking = true;
                UpdateDirectionFacingPlayer(target.playerX, target.playerY);

                attackAnimation.LoadFrames(Path.Combine(baseFolderPath, "Attack", Direction));
                attackAnimation.ResetAnimation();

                target.TakeDamage(1);
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
