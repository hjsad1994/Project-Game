using System;
using System.Drawing;

namespace Project_Game.Entities
{
    public class DroppedItem : GameObject
    {
        public Item Item { get; private set; }

        // Thuộc tính cho hiệu ứng nảy nhẩy
        private float bounceAmplitude = 5f; // Biên độ nảy (pixels)
        private float bounceFrequency = 0.05f; // Tần số nảy
        private float initialY; // Vị trí Y ban đầu khi drop
        private float bounceTimer = 0f; // Bộ đếm thời gian cho nảy

        // Thuộc tính cho hiệu ứng bay vào người chơi
        private bool isFlying = false;
        private Player targetPlayer;
        private float flySpeed = 5f; // Tốc độ bay vào người chơi

        public bool IsFlying => isFlying;

        public DroppedItem(Item item, int x, int y, int width = 32, int height = 32)
            : base(x, y, width, height, "DroppedItem", 1) // Health không quan trọng cho DroppedItem
        {
            Item = item;
            initialY = y;
        }

        public override void TakeDamage(int damage)
        {
            // DroppedItem không nhận sát thương, có thể để trống hoặc thêm logic nếu cần
        }

        public Image GetCurrentFrame()
        {
            return Item.Icon;
        }

        public void Update(Player player)
        {
            if (isFlying && targetPlayer != null)
            {
                FlyTowardsPlayer();
            }
            else
            {
                Bounce();
            }
        }

        private void Bounce()
        {
            bounceTimer += 1f;
            float offsetY = (float)(Math.Sin(bounceTimer * bounceFrequency) * bounceAmplitude);
            Y = (int)(initialY + offsetY);
        }

        private void FlyTowardsPlayer()
        {
            // Tính hướng di chuyển tới người chơi
            float deltaX = targetPlayer.X - X;
            float deltaY = targetPlayer.Y - Y;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance > 0)
            {
                // Tính vector đơn vị hướng tới người chơi
                float directionX = deltaX / distance;
                float directionY = deltaY / distance;

                // Di chuyển theo hướng đó
                X += (int)(directionX * flySpeed);
                Y += (int)(directionY * flySpeed);

                // Nếu đã đến gần người chơi, tự động thu thập
                if (distance < flySpeed * 2)
                {
                    // Thực hiện thu thập item
                    PickupItem();
                }
            }
        }

        private void PickupItem()
        {
            // Thêm item vào inventory của người chơi
            bool added = targetPlayer.InventoryManager.AddItem(Item);
            if (added)
            {
                // Xoá DroppedItem khỏi GameObjectManager
                GameObjectManager.Instance.DroppedItems.Remove(this);
                Console.WriteLine($"Item {Item.Name} đã được thu thập vào inventory.");
            }
            else
            {
                Console.WriteLine("Inventory đã đầy. Không thể thu thập thêm item.");
            }
        }

        /// <summary>
        /// Bắt đầu hiệu ứng bay vào người chơi khi người chơi gần đến.
        /// </summary>
        public void StartFlyingTowardsPlayer(Player player)
        {
            isFlying = true;
            targetPlayer = player;
            Console.WriteLine($"DroppedItem {Item.Name} bắt đầu bay về phía Player.");
        }

        public void Draw(Graphics g)
        {
            if (GetCurrentFrame() != null)
            {
                g.DrawImage(GetCurrentFrame(), X, Y, Width, Height);
                Console.WriteLine($"Đang vẽ DroppedItem: {Item.Name} tại ({X}, {Y})");
            }
            else
            {
                Console.WriteLine($"DroppedItem {Item.Name} không có hình ảnh để vẽ.");
            }
        }
    }
}
