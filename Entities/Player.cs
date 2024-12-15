using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Project_Game.Entities
{
    public class Player : GameObject
    {
        // Các thuộc tính di chuyển
        public bool GoLeft { get; set; }
        public bool GoRight { get; set; }
        public bool GoUp { get; set; }
        public bool GoDown { get; set; }

        // Vị trí trước khi di chuyển
        public int PreviousX { get; set; }
        public int PreviousY { get; set; }

        // Vị trí hiện tại của người chơi
        public int playerX { get; set; } = 100;
        public int playerY { get; set; } = 100;
        public int playerWidth { get; set; } = 22;
        public int playerHeight { get; set; } = 35;

        // Tốc độ di chuyển và phạm vi tấn công
        public int playerSpeed { get; set; } = 2;
        public int AttackRange { get; set; } = 50; // Phạm vi tấn công mặc định 50

        // Sức khỏe của người chơi
        public int Health { get; private set; } = 100;
        public int MaxHealth { get; private set; } = 100;
        public event Action<int> OnHealthChanged;

        // Quản lý hoạt ảnh
        public AnimationManager movementAnimation { get; private set; }
        private AnimationManager idleAnimation;
        private AnimationManager attackAnimation;
        private bool wasMovingPreviously = false;
        public bool IsAttacking { get; private set; } = false;
        private string currentDirection = "Down";

        // Danh sách các đối tượng game
        private List<GameObject> obstacles;
        private List<TestEnemy> enemies;
        private List<Chicken> chickens;

        // Trạng thái bị chặn
        public bool BlockedLeft { get; private set; } = false;
        public bool BlockedRight { get; private set; } = false;
        public bool BlockedUp { get; private set; } = false;
        public bool BlockedDown { get; private set; } = false;

        public bool IsBlockedLeft => BlockedLeft;
        public bool IsBlockedRight => BlockedRight;
        public bool IsBlockedUp => BlockedUp;
        public bool IsBlockedDown => BlockedDown;
        public string CurrentDirection => currentDirection;

        // Constructor nhận 3 tham số
        public Player(List<GameObject> obstacles, List<TestEnemy> enemies, List<Chicken> chickens)
            : base(0, 0, 22, 35, "Player", 100) // Gọi constructor của GameObject
        {
            this.obstacles = obstacles;
            this.enemies = enemies;
            this.chickens = chickens;

            // Khởi tạo các AnimationManager
            movementAnimation = new AnimationManager(frameRate: 10);
            idleAnimation = new AnimationManager(frameRate: 10);
            attackAnimation = new AnimationManager(frameRate: 8);

            // Load default animations với đường dẫn mới
            string movementPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveDown");
            string idlePath = Path.Combine("Assets", "Player", "Char_Idle", "Down");

            movementAnimation.LoadFrames(movementPath);
            idleAnimation.LoadFrames(idlePath);

            // Thêm kiểm tra để debug nếu không tải được khung hình
            if (movementAnimation.GetFrameCount() == 0)
            {
                Console.WriteLine($"Không thể tải khung hình từ {movementPath}");
            }

            if (idleAnimation.GetFrameCount() == 0)
            {
                Console.WriteLine($"Không thể tải khung hình từ {idlePath}");
            }
        }

        // Đặt lại trạng thái bị chặn khi di chuyển không thành công
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

        // Xử lý khi nhận sát thương
        public override void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
            OnHealthChanged?.Invoke(Health);
        }

        // Đặt lại sức khỏe
        public void ResetHealth()
        {
            Health = MaxHealth;
        }

        // Phương thức di chuyển
        public void Move()
        {
            if (IsAttacking) return;

            bool isMoving = false;
            int newX = playerX;
            int newY = playerY;

            PreviousX = playerX;
            PreviousY = playerY;

            // Xử lý di chuyển theo hướng
            if (GoLeft)
            {
                if (currentDirection != "Left")
                {
                    string moveLeftPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveLeft");
                    movementAnimation.LoadFrames(moveLeftPath);
                    currentDirection = "Left";
                }
                newX -= playerSpeed;
                isMoving = true;
            }
            else if (GoRight)
            {
                if (currentDirection != "Right")
                {
                    string moveRightPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveRight");
                    movementAnimation.LoadFrames(moveRightPath);
                    currentDirection = "Right";
                }
                newX += playerSpeed;
                isMoving = true;
            }
            else if (GoUp)
            {
                if (currentDirection != "Up")
                {
                    string moveUpPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveUp");
                    movementAnimation.LoadFrames(moveUpPath);
                    currentDirection = "Up";
                }
                newY -= playerSpeed;
                isMoving = true;
            }
            else if (GoDown)
            {
                if (currentDirection != "Down")
                {
                    string moveDownPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveDown");
                    movementAnimation.LoadFrames(moveDownPath);
                    currentDirection = "Down";
                }
                newY += playerSpeed;
                isMoving = true;
            }

            // Giới hạn vùng di chuyển
            if (newX < 0) newX = 0;
            if (newX + playerWidth > 800) newX = 800 - playerWidth;
            if (newY < 0) newY = 0;
            if (newY + playerHeight > 630) newY = 630 - playerHeight;

            Rectangle newRect = new Rectangle(newX, newY, playerWidth, playerHeight);
            bool collisionDetected = false;
            string collisionDirection = "";

            // Kiểm tra va chạm với obstacles
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

            // Kiểm tra va chạm với enemies nếu chưa va chạm obstacle
            if (!collisionDetected)
            {
                foreach (var enemy in enemies)
                {
                    Rectangle enemyRect = new Rectangle(enemy.X, enemy.Y, enemy.Width, enemy.Height);
                    if (newRect.IntersectsWith(enemyRect))
                    {
                        collisionDetected = true;

                        if (GoLeft) collisionDirection = "Left";
                        else if (GoRight) collisionDirection = "Right";
                        else if (GoUp) collisionDirection = "Up";
                        else if (GoDown) collisionDirection = "Down";

                        break;
                    }
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

        // Cập nhật hoạt ảnh idle dựa trên hướng hiện tại
        private void UpdateIdleAnimation()
        {
            string idlePath = string.Empty;
            switch (currentDirection)
            {
                case "Left":
                    idlePath = Path.Combine("Assets", "Player", "Char_Idle", "Left");
                    break;
                case "Right":
                    idlePath = Path.Combine("Assets", "Player", "Char_Idle", "Right");
                    break;
                case "Up":
                    idlePath = Path.Combine("Assets", "Player", "Char_Idle", "Up");
                    break;
                case "Down":
                    idlePath = Path.Combine("Assets", "Player", "Char_Idle", "Down");
                    break;
            }

            idleAnimation.LoadFrames(idlePath);
            idleAnimation.ResetAnimation();
        }

        // Cập nhật hoạt ảnh idle
        private void AnimateIdle()
        {
            idleAnimation.UpdateAnimation();
        }

        // Đặt lại trạng thái người chơi
        public void ResetPlayer()
        {
            playerX = 100;
            playerY = 100;
            Health = MaxHealth;
            GoLeft = GoRight = GoUp = GoDown = false;
            IsAttacking = false;
            BlockedLeft = BlockedRight = BlockedUp = BlockedDown = false;
        }

        // Thực hiện tấn công
        public void PerformAttack(List<Enemy> targets)
        {
            if (!IsAttacking)
            {
                IsAttacking = true;
                string attackPath = Path.Combine("Assets", "Player_Attack", currentDirection);
                attackAnimation.LoadFrames(attackPath);
                attackAnimation.ResetAnimation();

                if (targets.Any())
                {
                    foreach (var target in targets)
                    {
                        target.TakeDamage(50);
                    }
                }
                else
                {
                    Console.WriteLine("Không có kẻ địch, chỉ tấn công không mục tiêu.");
                }
            }
        }

        // Cập nhật trạng thái tấn công
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

        // Lấy khung hình hiện tại để vẽ
        public Image GetCurrentFrame()
        {
            if (IsAttacking)
            {
                return attackAnimation.GetCurrentFrame();
            }
            return (GoLeft || GoRight || GoUp || GoDown) ? movementAnimation.GetCurrentFrame() : idleAnimation.GetCurrentFrame();
        }
    }
}
