using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Project_Game.Entities
{
    public class Player
    {
        public bool GoLeft { get; set; }
        public bool GoRight { get; set; }
        public bool GoUp { get; set; }
        public bool GoDown { get; set; }

        public int PreviousX { get; set; }
        public int PreviousY { get; set; }

        public int playerX { get; set; } = 100;
        public int playerY { get; set; } = 100;
        // Đặt kích thước mong muốn
        public int playerWidth { get; set; } = 22;
        public int playerHeight { get; set; } = 35;

        public int playerSpeed { get; set; } = 2;

        public int Health { get; private set; } = 100;
        public int MaxHealth { get; private set; } = 100;
        public event Action<int> OnHealthChanged;

        public AnimationManager movementAnimation { get; private set; }
        private AnimationManager idleAnimation;
        private AnimationManager attackAnimation;
        private bool wasMovingPreviously = false;
        public bool IsAttacking { get; private set; } = false;
        private string currentDirection = "Down";

        private List<GameObject> obstacles;
        private List<TestEnemy> enemies;
        private List<Chicken> chickens;

        public bool BlockedLeft { get; private set; } = false;
        public bool BlockedRight { get; private set; } = false;
        public bool BlockedUp { get; private set; } = false;
        public bool BlockedDown { get; private set; } = false;

        public bool IsBlockedLeft => BlockedLeft;
        public bool IsBlockedRight => BlockedRight;
        public bool IsBlockedUp => BlockedUp;
        public bool IsBlockedDown => BlockedDown;
        public string CurrentDirection => currentDirection;

        public Player(List<GameObject> obstacles, List<TestEnemy> enemies, List<Chicken> chickens)
        {
            this.obstacles = obstacles;
            this.enemies = enemies;
            this.chickens = chickens;

            movementAnimation = new AnimationManager(frameRate: 10);
            idleAnimation = new AnimationManager(frameRate: 10);
            attackAnimation = new AnimationManager(frameRate: 10);

            // Load default animations
            movementAnimation.LoadFrames("Char_MoveMent/MoveDown");
            idleAnimation.LoadFrames("Char_Idle/Down");
        }

        public void UnblockDirection(string direction)
        {
            switch (direction)
            {
                case "Left":
                    BlockedLeft = false;
                    break;
                case "Right":
                    BlockedRight = false;
                    break;
                case "Up":
                    BlockedUp = false;
                    break;
                case "Down":
                    BlockedDown = false;
                    break;
            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
            OnHealthChanged?.Invoke(Health);
        }

        public void ResetHealth()
        {
            Health = MaxHealth;
        }

        public void Move()
        {
            if (IsAttacking) return;

            bool isMoving = false;
            int newX = playerX;
            int newY = playerY;

            PreviousX = playerX;
            PreviousY = playerY;

            // Chỉ 4 hướng: Up, Down, Left, Right
            if (GoLeft)
            {
                if (currentDirection != "Left")
                {
                    movementAnimation.LoadFrames("Char_MoveMent/MoveLeft");
                    currentDirection = "Left";
                }
                newX -= playerSpeed;
                isMoving = true;
            }
            else if (GoRight)
            {
                if (currentDirection != "Right")
                {
                    movementAnimation.LoadFrames("Char_MoveMent/MoveRight");
                    currentDirection = "Right";
                }
                newX += playerSpeed;
                isMoving = true;
            }
            else if (GoUp)
            {
                if (currentDirection != "Up")
                {
                    movementAnimation.LoadFrames("Char_MoveMent/MoveUp");
                    currentDirection = "Up";
                }
                newY -= playerSpeed;
                isMoving = true;
            }
            else if (GoDown)
            {
                if (currentDirection != "Down")
                {
                    movementAnimation.LoadFrames("Char_MoveMent/MoveDown");
                    currentDirection = "Down";
                }
                newY += playerSpeed;
                isMoving = true;
            }

            // Giới hạn vùng di chuyển (ví dụ 800x630)
            if (newX < 0) newX = 0;
            if (newX + playerWidth > 800) newX = 800 - playerWidth;
            if (newY < 0) newY = 0;
            if (newY + playerHeight > 630) newY = 630 - playerHeight;

            Rectangle newRect = new Rectangle(newX, newY, playerWidth, playerHeight);
            bool collisionDetected = false;
            string collisionDirection = "";

            // Kiểm tra va chạm obstacles
            foreach (var obstacle in obstacles)
            {
                Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
                if (newRect.IntersectsWith(obstacleRect))
                {
                    collisionDetected = true;

                    if (GoLeft) collisionDirection = "Left";
                    else if (GoRight) collisionDirection = "Right";
                    else if (GoUp) collisionDirection = "Up";
                    else if (GoDown) collisionDirection = "Down";

                    break;
                }
            }

            if (collisionDetected)
            {
                UpdateIdleAnimation();
                switch (collisionDirection)
                {
                    case "Left":
                        BlockedLeft = true;
                        GoLeft = false;
                        break;
                    case "Right":
                        BlockedRight = true;
                        GoRight = false;
                        break;
                    case "Up":
                        BlockedUp = true;
                        GoUp = false;
                        break;
                    case "Down":
                        BlockedDown = true;
                        GoDown = false;
                        break;
                }
                wasMovingPreviously = false;
                return;
            }

            if (isMoving)
            {
                playerX = newX;
                playerY = newY;
                movementAnimation.UpdateAnimation();
            }
            else
            {
                if (wasMovingPreviously)
                {
                    UpdateIdleAnimation();
                }
                AnimateIdle();
            }

            wasMovingPreviously = isMoving;
        }

        private void UpdateIdleAnimation()
        {
            switch (currentDirection)
            {
                case "Left":
                    idleAnimation.LoadFrames("Char_Idle/Left");
                    break;
                case "Right":
                    idleAnimation.LoadFrames("Char_Idle/Right");
                    break;
                case "Up":
                    idleAnimation.LoadFrames("Char_Idle/Up");
                    break;
                case "Down":
                    idleAnimation.LoadFrames("Char_Idle/Down");
                    break;
            }
            idleAnimation.ResetAnimation();
        }

        private void AnimateIdle()
        {
            idleAnimation.UpdateAnimation();
        }

        public void ResetPlayer()
        {
            playerX = 100;
            playerY = 100;
            Health = MaxHealth;
            GoLeft = GoRight = GoUp = GoDown = false;
            IsAttacking = false;
            BlockedLeft = BlockedRight = BlockedUp = BlockedDown = false;
        }

        public void PerformAttack(List<Enemy> targets)
        {
            if (!IsAttacking)
            {
                IsAttacking = true;
                attackAnimation.LoadFrames($"Player_Attack/{currentDirection}");
                attackAnimation.ResetAnimation();

                if (targets.Any())
                {
                    foreach (var target in targets)
                    {
                        target.TakeDamage(20);
                    }
                }
                else
                {
                    Console.WriteLine("Không có kẻ địch, chỉ tấn công không mục tiêu.");
                }
            }
        }

        public void UpdateAttack()
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

        public Image GetCurrentFrame()
        {
            if (IsAttacking)
            {
                return attackAnimation.CurrentFrame;
            }
            return (GoLeft || GoRight || GoUp || GoDown) ? movementAnimation.CurrentFrame : idleAnimation.CurrentFrame;
        }
    }
}
