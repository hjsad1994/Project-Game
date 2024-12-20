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
        public int playerWidth { get; set; } = 16;
        public int playerHeight { get; set; } = 19;

        // Tốc độ di chuyển và phạm vi tấn công
        public int playerSpeed { get; set; } = 2;
        public int AttackRange { get; set; } = 30; // Phạm vi tấn công mặc định 30

        // Sức khỏe của người chơi
        public int Health { get; private set; } = 100;
        public int MaxHealth { get; private set; } = 100;
        public event Action<int> OnHealthChanged;

        // Quản lý hoạt ảnh
        public AnimationManager movementAnimation { get; private set; }
        public AnimationManager idleAnimation { get; private set; }
        public AnimationManager attackAnimation { get; private set; }
        public AnimationManager chopAnimation { get; private set; }

        private bool wasMovingPreviously = false;
        public bool IsAttacking { get; private set; } = false;
        private string currentDirection = "Down";

        // Danh sách các đối tượng game
        private List<GameObject> obstacles;
        private List<TestEnemy> enemies;
        private List<Chicken> chickens;
        private List<Ore> ores; // Thêm danh sách Ores

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

        public string CurrentWeapon { get; private set; } = "Sword"; // Vũ khí mặc định

        public InventoryManager InventoryManager { get; set; }

        // Đồng bộ hóa X và Y với playerX và playerY
        public override int X
        {
            get { return playerX; }
            set { playerX = value; }
        }

        public override int Y
        {
            get { return playerY; }
            set { playerY = value; }
        }

        // Constructor nhận 4 tham số (thêm danh sách Ores)
        public Player(List<GameObject> obstacles, List<TestEnemy> enemies, List<Chicken> chickens, List<Ore> ores)
            : base(0, 0, 20, 33, "Player", 100) // Gọi constructor của GameObject
        {
            this.obstacles = obstacles;
            this.enemies = enemies;
            this.chickens = chickens;
            this.ores = ores;

            // Khởi tạo các AnimationManager
            movementAnimation = new AnimationManager(frameRate: 12);
            idleAnimation = new AnimationManager(frameRate: 12);
            attackAnimation = new AnimationManager(frameRate: 13);
            chopAnimation = new AnimationManager(frameRate: 13);   // Tốc độ chậm hơn cho chop

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

            // Khởi tạo InventoryManager nếu chưa
            InventoryManager = new InventoryManager();
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

        public void SetCurrentWeapon(string weaponName)
        {
            CurrentWeapon = weaponName;
            Console.WriteLine($"Player equipped: {CurrentWeapon}");
        }

        // Xử lý khi nhận sát thương
        public override void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
            OnHealthChanged?.Invoke(Health);
            Console.WriteLine($"Player took {damage} damage. Current Health: {Health}");
        }

        // Đặt lại sức khỏe
        public void ResetHealth()
        {
            Health = MaxHealth;
            OnHealthChanged?.Invoke(Health);
            Console.WriteLine("Player health reset to MaxHealth.");
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

            // Handle movement
            if (GoLeft)
            {
                if (currentDirection != "Left")
                {
                    string moveLeftPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveLeft");
                    movementAnimation.ClearCache(); // Xóa cache trước khi tải mới

                    movementAnimation.LoadFrames(moveLeftPath);
                    currentDirection = "Left";
                    Console.WriteLine("Loaded MoveLeft Animation");
                }
                newX -= playerSpeed;
                isMoving = true;
            }
            else if (GoRight)
            {
                if (currentDirection != "Right")
                {
                    string moveRightPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveRight");
                    movementAnimation.ClearCache(); // Xóa cache trước khi tải mới

                    movementAnimation.LoadFrames(moveRightPath);
                    currentDirection = "Right";
                    Console.WriteLine("Loaded MoveRight Animation");
                }
                newX += playerSpeed;
                isMoving = true;
            }
            else if (GoUp)
            {
                if (currentDirection != "Up")
                {
                    string moveUpPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveUp");
                    movementAnimation.ClearCache(); // Xóa cache trước khi tải mới

                    movementAnimation.LoadFrames(moveUpPath);
                    currentDirection = "Up";
                    Console.WriteLine("Loaded MoveUp Animation");
                }
                newY -= playerSpeed;
                isMoving = true;
            }
            else if (GoDown)
            {
                if (currentDirection != "Down")
                {
                    string moveDownPath = Path.Combine("Assets", "Player", "Char_Movement", "MoveDown");
                    movementAnimation.ClearCache(); // Xóa cache trước khi tải mới

                    movementAnimation.LoadFrames(moveDownPath);

                    currentDirection = "Down";
                    Console.WriteLine("Loaded MoveDown Animation");
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

                    Console.WriteLine($"Collision detected: {collisionDirection}");
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

                        Console.WriteLine($"Collision with Enemy detected: {collisionDirection}");
                        break;
                    }
                }
            }

            // Kiểm tra va chạm với quặng nếu chưa va chạm obstacle và enemy
            if (!collisionDetected)
            {
                foreach (var ore in ores)
                {
                    Rectangle oreRect = new Rectangle(ore.X, ore.Y, ore.Width, ore.Height);
                    if (newRect.IntersectsWith(oreRect))
                    {
                        collisionDetected = true;

                        if (GoLeft) collisionDirection = "Left";
                        else if (GoRight) collisionDirection = "Right";
                        else if (GoUp) collisionDirection = "Up";
                        else if (GoDown) collisionDirection = "Down";

                        Console.WriteLine($"Collision with Ore detected: {collisionDirection}");
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
                Console.WriteLine($"Movement blocked: {collisionDirection}");
                return;
            }

            if (isMoving)
            {
                playerX = newX;
                playerY = newY;
                movementAnimation.UpdateAnimation();
                Console.WriteLine($"Player moved to ({playerX}, {playerY}) - Frame: {movementAnimation.GetCurrentFrameName()}");
            }
            else
            {
                if (wasMovingPreviously)
                {
                    UpdateIdleAnimation();
                    Console.WriteLine("Switching to Idle Animation");
                }
                AnimateIdle();
            }

            wasMovingPreviously = isMoving;

            // Debug: Trạng thái di chuyển kết thúc
            Console.WriteLine($"Move End: CurrentDirection={currentDirection}");
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

            Console.WriteLine($"UpdateIdleAnimation: Loading Idle Animation from {idlePath}");
            idleAnimation.ClearCache(); // Xóa cache trước khi tải mới

            idleAnimation.LoadFrames(idlePath);
            idleAnimation.ResetAnimation();

            // Thêm kiểm tra để debug nếu không tải được khung hình
            if (idleAnimation.GetFrameCount() == 0)
            {
                Console.WriteLine($"Cannot load idle frames from {idlePath}");
            }
            else
            {
                Console.WriteLine($"Loaded {idleAnimation.GetFrameCount()} idle frames từ {idlePath}");
            }
        }

        // Cập nhật hoạt ảnh idle
        private void AnimateIdle()
        {
            idleAnimation.UpdateAnimation();
            // Console.WriteLine($"Animating Idle: Frame={idleAnimation.GetCurrentFrameName()}");
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

            // Reset hoạt ảnh
            string idlePath = Path.Combine("Assets", "Player", "Char_Idle", "Down");
            idleAnimation.LoadFrames(idlePath);
            idleAnimation.ResetAnimation();

            // Thêm kiểm tra để debug nếu không tải được khung hình
            if (idleAnimation.GetFrameCount() == 0)
            {
                Console.WriteLine($"Không thể tải khung hình từ {idlePath}");
            }
            else
            {
                Console.WriteLine($"Loaded {idleAnimation.GetFrameCount()} idle frames từ {idlePath}");
            }

            Console.WriteLine("Player đã được reset.");
        }

        // Thực hiện tấn công
        public void PerformAttack(List<Enemy> targets)
        {
            if (!IsAttacking)
            {
                IsAttacking = true;

                string attackPath = string.Empty;
                switch (CurrentWeapon)
                {
                    case "Sword":
                        attackPath = Path.Combine("Assets", "Player_Attack", currentDirection);
                        break;
                    case "Axe":
                        attackPath = Path.Combine("Assets", "Player", "Player_Cut", currentDirection);
                        break;
                    case "Pickaxe":
                        attackPath = Path.Combine("Assets", "Player", "Player_Dig-up", currentDirection);
                        break;
                    case "Watering-can":
                        attackPath = Path.Combine("Assets", "Player", "Player_Watering", currentDirection);
                        break;
                    default:
                        attackPath = Path.Combine("Assets", "Player_Attack", currentDirection);
                        break;
                }

                Console.WriteLine($"PerformAttack: CurrentWeapon={CurrentWeapon}, CurrentDirection={currentDirection}, AttackPath={attackPath}");

                attackAnimation.LoadFrames(attackPath);
                attackAnimation.ResetAnimation();

                // Debug checks
                if (attackAnimation.GetFrameCount() == 0)
                {
                    Console.WriteLine($"Cannot load attack frames from {attackPath}");
                }
                else
                {
                    Console.WriteLine($"Loaded {attackAnimation.GetFrameCount()} attack frames từ {attackPath}");
                }

                if (targets.Any())
                {
                    foreach (var target in targets)
                    {
                        int damage;
                        switch (CurrentWeapon)
                        {
                            case "Sword":
                                damage = 15; // Sát thương cho Sword
                                break;
                            case "Axe":
                                damage = 25;
                                break;
                            case "Pickaxe":
                                damage = 25;
                                break;
                            case "Watering-can":
                                damage = 50;
                                break;
                            default:
                                damage = 10;
                                break;
                        }
                        target.TakeDamage(damage);
                        Console.WriteLine($"Player đã gây {damage} sát thương cho {target.Name}.");
                    }
                    Console.WriteLine($"Player đã tấn công {targets.Count} kẻ địch với {CurrentWeapon}.");
                }
                else
                {
                    Console.WriteLine($"Player đã thực hiện tấn công {CurrentWeapon}, nhưng không có kẻ địch nào bị trúng.");
                }

                // Optionally, implement attack cooldowns or other logic here
            }
        }

        // Cập nhật trạng thái tấn công
        public void UpdateAttack()
        {
            if (IsAttacking)
            {
                attackAnimation.UpdateAnimation(loop: false);
                Console.WriteLine($"Updating Attack Animation: Frame={attackAnimation.GetCurrentFrameName()}");

                if (attackAnimation.IsComplete())
                {
                    IsAttacking = false;
                    Console.WriteLine("Attack Animation Complete");
                }
            }
        }

        // Lấy khung hình hiện tại để vẽ
        public virtual Image GetCurrentFrame()
        {
            if (IsAttacking)
            {
                string attackFrameName = attackAnimation.GetCurrentFrameName();
                // Console.WriteLine($"GetCurrentFrame: IsAttacking=true, CurrentWeapon={CurrentWeapon}, CurrentDirection={currentDirection}, Frame={attackFrameName}");
                return attackAnimation.GetCurrentFrame();
            }

            if (GoLeft || GoRight || GoUp || GoDown)
            {
                string moveFrameName = movementAnimation.GetCurrentFrameName();
                // Console.WriteLine($"GetCurrentFrame: Moving, CurrentDirection={currentDirection}, Frame={moveFrameName}");
                return movementAnimation.GetCurrentFrame();
            }

            string idleFrameName = idleAnimation.GetCurrentFrameName();
            // Console.WriteLine($"GetCurrentFrame: Idle, CurrentDirection={currentDirection}, Frame={idleFrameName}");
            return idleAnimation.GetCurrentFrame();
        }

        public void SetObstacles(List<GameObject> newObstacles)
        {
            this.obstacles = newObstacles;
            // Cập nhật danh sách Ores từ Obstacles nếu cần
            this.ores = newObstacles.OfType<Ore>().ToList();
        }

        public void PickupItems(List<DroppedItem> droppedItems, InventoryManager inventory)
        {
            for (int i = droppedItems.Count - 1; i >= 0; i--)
            {
                var item = droppedItems[i];
                Rectangle playerRect = new Rectangle(X, Y, Width, Height);
                Rectangle itemRect = new Rectangle(item.X, item.Y, item.Width, item.Height);

                // Debug: Kiểm tra các Rectangle
                Console.WriteLine($"Player Rect: {playerRect}, Item Rect: {itemRect}");

                if (playerRect.IntersectsWith(itemRect))
                {
                    bool added = inventory.AddItem(item.Item);
                    if (added)
                    {
                        droppedItems.RemoveAt(i);
                        Console.WriteLine($"{Name} đã nhặt được item: {item.Item.Name}");
                    }
                    else
                    {
                        Console.WriteLine("Inventory đã đầy. Không thể nhặt thêm item.");
                    }
                }
            }
        }

        // Thực hiện chặt cây
        public void Chop()
        {
            if (CurrentWeapon != "Axe")
            {
                Console.WriteLine("Only Axe can chop trees.");
                return;
            }

            if (!IsAttacking)
            {
                IsAttacking = true;

                // Đường dẫn tới hoạt ảnh Player_Cut dựa trên hướng hiện tại
                string chopPath = Path.Combine("Assets", "Player", "Player_Cut", currentDirection);
                attackAnimation.LoadFrames(chopPath);
                attackAnimation.ResetAnimation();

                Console.WriteLine($"Chop Animation Loaded from {chopPath}");

                // Kiểm tra nếu không tải được khung hình
                if (attackAnimation.GetFrameCount() == 0)
                {
                    Console.WriteLine($"Cannot load chop frames from {chopPath}");
                }
                else
                {
                    Console.WriteLine($"Loaded {attackAnimation.GetFrameCount()} chop frames từ {chopPath}");
                }

                // Thực hiện các tác động sau khi chặt cây (nếu cần)
                // Ví dụ: Giảm số lượng cây, cập nhật trạng thái cây, v.v.
            }
        }

        // Phương thức để đào quặng khi nhấn phím V
        public void MineOres(List<Ore> ores)
        {
            if (!IsAttacking && CurrentWeapon == "Pickaxe")
            {
                // Tìm quặng gần nhất trong khoảng đào
                int miningRange = 50; // Khoảng cách tối đa để đào quặng
                Ore nearestOre = null;
                double minDistance = double.MaxValue;

                foreach (var ore in ores)
                {
                    double distance = Math.Sqrt(Math.Pow(playerX - ore.X, 2) + Math.Pow(playerY - ore.Y, 2));
                    if (distance <= miningRange && distance < minDistance)
                    {
                        nearestOre = ore;
                        minDistance = distance;
                    }
                }

                if (nearestOre != null)
                {
                    Console.WriteLine($"[Info] Đang đào quặng tại ({nearestOre.X}, {nearestOre.Y}).");
                    nearestOre.Mine(InventoryManager);

                    // Kích hoạt hoạt ảnh đào quặng
                    IsAttacking = true;
                    string miningPath = Path.Combine("Assets", "Player", "Player_Dig-up", currentDirection);
                    attackAnimation.LoadFrames(miningPath);
                    attackAnimation.ResetAnimation();

                    // Kiểm tra tải khung hình thành công
                    if (attackAnimation.GetFrameCount() == 0)
                    {
                        Console.WriteLine($"Cannot load mining frames from {miningPath}");
                    }
                    else
                    {
                        Console.WriteLine($"Loaded {attackAnimation.GetFrameCount()} mining frames từ {miningPath}");
                    }
                    GameObjectManager.Instance.RemoveOre(nearestOre);

                }
                else
                {
                    Console.WriteLine("[Info] Không tìm thấy quặng nào trong phạm vi đào.");
                }
            }
            else
            {
                Console.WriteLine("[Info] Bạn cần có Pickaxe để đào quặng hoặc đang trong trạng thái tấn công.");
            }
        }
    }
}
