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

        public int AttackRange { get; private set; } = 26;
        public int DetectionRange { get; private set; } = 120;

        private int attackCooldown = 1000;
        private DateTime lastAttackTime = DateTime.MinValue;

        private List<string> possibleDirections = new List<string> { "Left", "Right", "Up", "Down" };
        private int directionChangeCooldown = 1500;
        private DateTime lastDirectionChangeTime = DateTime.MinValue;

        private static Random random = new Random();

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
            LoadDeadAnimation(Path.Combine(baseFolderPath, "Dead"));
        }

        // Ghi đè X, Y để lấy từ posX, posY
        public override int X { get { return (int)posX; } set { posX = value; } }
        public override int Y { get { return (int)posY; } set { posY = value; } }


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
                var anim = new AnimationManager(frameRate: 15);
                anim.LoadFrames(Path.Combine(baseFolderPath, dir));
                idleAnimations[dir] = anim;
            }
        }

        private void LoadDeadAnimation(string baseFolderPath)
        {
            deadAnimation = new AnimationManager(frameRate: 15);
            deadAnimation.LoadFrames(baseFolderPath);
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

        public void Update(List<GameObject> obstacles, Player target)
        {
            if (isDead) return;

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
    }
}
