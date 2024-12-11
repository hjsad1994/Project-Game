using Project_Game;
using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

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

        private List<GameObject> obstacles; // Lưu danh sách obstacles                        asdsad          s

        // Các cờ bị chặn
        public bool BlockedLeft { get; private set; } = false;
        public bool BlockedRight { get; private set; } = false;
        public bool BlockedUp { get; private set; } = false;
        public bool BlockedDown { get; private set; } = false;

        // Các thuộc tính công khai để kiểm tra trạng thái bị chặn
        public bool IsBlockedLeft => BlockedLeft;
        public bool IsBlockedRight => BlockedRight;
        public bool IsBlockedUp => BlockedUp;
        public bool IsBlockedDown => BlockedDown;
        public string CurrentDirection => currentDirection;

        private List<TestEnemy> enemies; // Danh sách TestEnemy
        private List<Chicken> chickens; // Danh sách Chicken

        // Phương thức để bỏ chặn hướng khi phím được thả
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

        public Player(List<GameObject> obstacles, List<TestEnemy> enemies, List<Chicken> chickens)
        {
            this.obstacles = obstacles; // Gán danh sách obstacles vào trường
            movementAnimation = new AnimationManager(frameRate: 10);
            idleAnimation = new AnimationManager(frameRate: 10);
            attackAnimation = new AnimationManager(frameRate: 10);

            this.obstacles = obstacles;
            this.enemies = enemies;
            this.chickens = chickens;

            // Load default animations
            movementAnimation.LoadFrames("Char_MoveMent/MoveDown");
            idleAnimation.LoadFrames("Char_Idle/Down");
        }

        public void TakeDamage(int damage)
        {
            Console.WriteLine($"Player nhận {damage} damage trước khi giảm: {Health}");
            Health -= damage;
            if (Health < 0) Health = 0;
            Console.WriteLine($"Player nhận {damage} damage sau khi giảm: {Health}");

            // Gọi sự kiện
            OnHealthChanged?.Invoke(Health);
        }


        public void ResetHealth()
        {
            Health = MaxHealth;
        }

        //public void Move()
        //{
        //    if (IsAttacking) return;

        //    bool isMoving = false;
        //    int newX = playerX;
        //    int newY = playerY;

        //    // Lưu lại vị trí trước khi di chuyển
        //    PreviousX = playerX;
        //    PreviousY = playerY;

        //    if (GoLeft)
        //    {
        //        if (currentDirection != "Left")
        //        {
        //            movementAnimation.LoadFrames("Char_MoveMent/MoveLeft");
        //            currentDirection = "Left";
        //        }
        //        newX -= playerSpeed;
        //        isMoving = true;
        //    }
        //    else if (GoRight)
        //    {
        //        if (currentDirection != "Right")
        //        {
        //            movementAnimation.LoadFrames("Char_MoveMent/MoveRight");
        //            currentDirection = "Right";
        //        }
        //        newX += playerSpeed;
        //        isMoving = true;
        //    }
        //    else if (GoUp)
        //    {
        //        if (currentDirection != "Up")
        //        {
        //            movementAnimation.LoadFrames("Char_MoveMent/MoveUp");
        //            currentDirection = "Up";
        //        }
        //        newY -= playerSpeed;
        //        isMoving = true;
        //    }
        //    else if (GoDown)
        //    {
        //        if (currentDirection != "Down")
        //        {
        //            movementAnimation.LoadFrames("Char_MoveMent/MoveDown");
        //            currentDirection = "Down";
        //        }
        //        newY += playerSpeed;
        //        isMoving = true;
        //    }

        //    Console.WriteLine($"Attempting to move to ({newX}, {newY})");

        //    // Kiểm tra va chạm trước khi di chuyển
        //    // ai biet gi dau
        //    Rectangle newRect = new Rectangle(newX, newY, playerWidth, playerHeight);
        //    bool collisionDetected = false;
        //    string collisionDirection = ""; // Để xác định hướng va chạm

        //    foreach (var obstacle in obstacles)
        //    {
        //        Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
        //        if (newRect.IntersectsWith(obstacleRect))
        //        {
        //            // Va chạm xảy ra, khôi phục vị trí trước đó
        //            Console.WriteLine($"Player gặp obstacle tại ({obstacle.X}, {obstacle.Y})");
        //            playerX = PreviousX;
        //            playerY = PreviousY;
        //            collisionDetected = true;

        //            // Xác định hướng va chạm dựa trên sự di chuyển
        //            if (GoLeft)
        //                collisionDirection = "Left";
        //            else if (GoRight)
        //                collisionDirection = "Right";
        //            else if (GoUp)
        //                collisionDirection = "Up";
        //            else if (GoDown)
        //                collisionDirection = "Down";

        //            break; // Thoát khỏi vòng lặp sau khi phát hiện va chạm
        //        }
        //    }

        //    if (collisionDetected)
        //    {
        //        // Đặt animation về trạng thái idle theo hướng hiện tại
        //        UpdateIdleAnimation();

        //        // Đánh dấu hướng bị chặn và đặt lại cờ di chuyển
        //        switch (collisionDirection)
        //        {
        //            case "Left":
        //                BlockedLeft = true;
        //                GoLeft = false;
        //                break;
        //            case "Right":
        //                BlockedRight = true;
        //                GoRight = false;
        //                break;
        //            case "Up":
        //                BlockedUp = true;
        //                GoUp = false;
        //                break;
        //            case "Down":
        //                BlockedDown = true;
        //                GoDown = false;
        //                break;
        //        }

        //        wasMovingPreviously = false; // Player đang ở trạng thái idle
        //        return; // Thoát khỏi phương thức Move
        //    }

        //    // Nếu không va chạm, cập nhật vị trí
        //    if (isMoving)
        //    {
        //        playerX = newX;
        //        playerY = newY;
        //        movementAnimation.UpdateAnimation();
        //        Console.WriteLine($"Player moved to ({playerX}, {playerY})");
        //    }
        //    else
        //    {
        //        // Nếu frame trước đang di chuyển, còn frame này không di chuyển nữa, nghĩa là vừa dừng
        //        if (wasMovingPreviously)
        //        {
        //            UpdateIdleAnimation();
        //        }
        //        AnimateIdle();
        //    }

        //    // Cập nhật trạng thái wasMovingPreviously cho frame tiếp theo
        //    wasMovingPreviously = isMoving;
        //}
        public void Move()
        {
            if (IsAttacking) return;

            bool isMoving = false;
            int newX = playerX;
            int newY = playerY;

            // Lưu lại vị trí trước khi di chuyển
            PreviousX = playerX;
            PreviousY = playerY;

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

            // Giới hạn di chuyển trong vùng 800x600
            if (newX < 0) newX = 0;
            if (newX + playerWidth > 800) newX = 800 - playerWidth;
            if (newY < 0) newY = 0;
            if (newY + playerHeight > 600) newY = 600 - playerHeight;

            // Kiểm tra va chạm với các chướng ngại vật tĩnh
            Rectangle newRect = new Rectangle(newX, newY, playerWidth, playerHeight);
            bool collisionDetected = false;
            string collisionDirection = "";

            foreach (var obstacle in obstacles)
            {
                Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
                if (newRect.IntersectsWith(obstacleRect))
                {
                    Console.WriteLine($"Player gặp obstacle tại ({obstacle.X}, {obstacle.Y})");
                    collisionDetected = true;

                    if (GoLeft)
                        collisionDirection = "Left";
                    else if (GoRight)
                        collisionDirection = "Right";
                    else if (GoUp)
                        collisionDirection = "Up";
                    else if (GoDown)
                        collisionDirection = "Down";

                    break;
                }
            }

            // Kiểm tra va chạm với TestEnemy
            if (!collisionDetected)
            {
                foreach (var enemy in enemies)
                {
                    Rectangle enemyRect = new Rectangle(enemy.X, enemy.Y, enemy.Width, enemy.Height);
                    if (newRect.IntersectsWith(enemyRect))
                    {
                        Console.WriteLine($"Player gặp TestEnemy tại ({enemy.X}, {enemy.Y})");
                        collisionDetected = true;

                        if (GoLeft)
                            collisionDirection = "Left";
                        else if (GoRight)
                            collisionDirection = "Right";
                        else if (GoUp)
                            collisionDirection = "Up";
                        else if (GoDown)
                            collisionDirection = "Down";

                        break;
                    }
                }
            }

            // Kiểm tra va chạm với Chicken
            if (!collisionDetected)
            {
                foreach (var chicken in chickens)
                {
                    Rectangle chickenRect = new Rectangle(chicken.X, chicken.Y, chicken.Width, chicken.Height);
                    if (newRect.IntersectsWith(chickenRect))
                    {
                        Console.WriteLine($"Player gặp Chicken tại ({chicken.X}, {chicken.Y})");
                        collisionDetected = true;

                        if (GoLeft)
                            collisionDirection = "Left";
                        else if (GoRight)
                            collisionDirection = "Right";
                        else if (GoUp)
                            collisionDirection = "Up";
                        else if (GoDown)
                            collisionDirection = "Down";

                        break;
                    }
                }
            }

            if (collisionDetected)
            {
                // Đặt animation về trạng thái idle theo hướng hiện tại
                UpdateIdleAnimation();

                // Đánh dấu hướng bị chặn và đặt lại cờ di chuyển
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

                wasMovingPreviously = false; // Player đang ở trạng thái idle
                return; // Thoát khỏi phương thức Move
            }

            // Nếu không va chạm, cập nhật vị trí mới
            if (isMoving)
            {
                playerX = newX;
                playerY = newY;
                movementAnimation.UpdateAnimation();
                Console.WriteLine($"Player moved to ({playerX}, {playerY})");
            }
            else
            {
                // Nếu frame trước đang di chuyển, còn frame này không di chuyển nữa, nghĩa là vừa dừng
                if (wasMovingPreviously)
                {
                    UpdateIdleAnimation();
                }
                AnimateIdle();
            }

            // Cập nhật trạng thái wasMovingPreviously cho frame tiếp theo
            wasMovingPreviously = isMoving;
        }

        // ... Các phương thức khác của Player như PerformAttack, UpdateAttack, etc. ..

        public void AnimateIdle()
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

            // Reset các cờ bị chặn
            BlockedLeft = BlockedRight = BlockedUp = BlockedDown = false;
        }

        // Trong Player.cs
        public void PerformAttack(List<Enemy> targets)
        {
            if (!IsAttacking)
            {
                IsAttacking = true;
                attackAnimation.LoadFrames($"Player_Attack/{currentDirection}");
                attackAnimation.ResetAnimation();

                if (targets.Any()) // Nếu có kẻ địch trong danh sách
                {
                    foreach (var target in targets)
                    {
                        target.TakeDamage(10); // Ví dụ, gây 10 sát thương
                        Console.WriteLine($"Player đã tấn công {target.Name}!");
                    }
                }
                else
                {
                    // Nếu không có kẻ địch, chỉ đơn giản thực hiện hoạt ảnh tấn công mà không gây sát thương
                    Console.WriteLine("Không có kẻ địch, chỉ thực hiện hoạt ảnh tấn công.");
                }
            }
        }




        public void UpdateAttack()
        {
            if (IsAttacking)
            {
                // Đảm bảo animation attack không loop
                attackAnimation.UpdateAnimation(loop: false);
                if (attackAnimation.IsComplete())
                {
                    IsAttacking = false;
                }
            }
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

        public Image GetCurrentFrame()
        {
            if (IsAttacking)
            {
                return attackAnimation.CurrentFrame;
            }
            return GoLeft || GoRight || GoUp || GoDown ? movementAnimation.CurrentFrame : idleAnimation.CurrentFrame;
        }
    }
}
