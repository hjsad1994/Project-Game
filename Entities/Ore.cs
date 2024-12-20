using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Project_Game.Entities
{
    public class Ore : GameObject, IRemovable
    {
        public Image Image { get; set; } // Hình ảnh của quặng

        // Danh sách các DropItem có thể rơi ra
        public List<DropItemInfo> DropItems { get; set; }

        // Trạng thái của quặng
        private bool isMined = false;
        private Random random = new Random();

        public bool ShouldRemove { get; set; } = false;

        public Ore(int x, int y, string imagePath, List<DropItemInfo> dropItems)
            : base(x, y, 40, 40, "Ore", 1) // Sử dụng kích thước tùy chỉnh
        {
            LoadOreImage(imagePath);
            DropItems = dropItems;
        }

        private void LoadOreImage(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                try
                {
                    Image = Image.FromFile(imagePath);
                    Console.WriteLine($"[Info] Đã tải hình ảnh cho quặng từ '{imagePath}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Lỗi khi tải hình ảnh quặng: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[Error] Không tìm thấy hình ảnh cho quặng tại '{imagePath}'.");
            }
        }

        /// <summary>
        /// Phương thức để đào quặng.
        /// </summary>
        /// <param name="inventory">Inventory của người chơi để thêm khoáng sản.</param>
        public void Mine(InventoryManager inventory)
        {
            if (isMined)
            {
                Console.WriteLine($"[Warning] Quặng tại ({X}, {Y}) đã được đào rồi.");
                return;
            }

            // Chọn DropItem dựa trên tỉ lệ probability
            DropItemInfo selectedDropItem = SelectDropItem();

            if (selectedDropItem != null)
            {
                string itemName = selectedDropItem.Name;
                string itemImagePath = selectedDropItem.ImagePath;

                if (File.Exists(itemImagePath))
                {
                    try
                    {
                        Image itemIcon = Image.FromFile(itemImagePath);
                        Item minedOre = new Item(itemName, itemIcon);

                        bool added = inventory.AddItem(minedOre);
                        if (added)
                        {
                            isMined = true;
                            Console.WriteLine($"[Info] Đã đào thành công {itemName} từ quặng tại ({X}, {Y}).");

                            // Tạo vật phẩm rơi xuống (DroppedItem)
                            DropOreItem(selectedDropItem);

                            // Đánh dấu quặng nên bị loại bỏ khỏi trò chơi
                            ShouldRemove = true;
                        }
                        else
                        {
                            Console.WriteLine($"[Error] Inventory đã đầy. Không thể đào thêm {itemName}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error] Lỗi khi đào quặng: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"[Error] Không tìm thấy hình ảnh cho item: {itemImagePath}");
                }
            }
            else
            {
                Console.WriteLine($"[Warning] Không có DropItem nào được chọn cho quặng tại ({X}, {Y}).");
            }
        }

        /// <summary>
        /// Chọn DropItem dựa trên tỉ lệ probability.
        /// </summary>
        /// <returns>DropItemInfo được chọn hoặc null nếu không có DropItem nào phù hợp.</returns>
        private DropItemInfo SelectDropItem()
        {
            if (DropItems == null || DropItems.Count == 0)
                return null;

            int totalProbability = DropItems.Sum(di => di.Probability);
            if (totalProbability <= 0)
                return null;

            int randValue = random.Next(1, totalProbability + 1);
            int cumulative = 0;

            foreach (var dropItem in DropItems)
            {
                cumulative += dropItem.Probability;
                if (randValue <= cumulative)
                    return dropItem;
            }

            return null;
        }

        /// <summary>
        /// Tạo vật phẩm rơi xuống sau khi đào quặng.
        /// </summary>
        /// <param name="dropItem">Thông tin về DropItem được chọn.</param>
        private void DropOreItem(DropItemInfo dropItem)
        {
            string itemName = dropItem.Name;
            string itemImagePath = dropItem.ImagePath;

            if (File.Exists(itemImagePath))
            {
                try
                {
                    Image itemIcon = Image.FromFile(itemImagePath);
                    Item droppedItem = new Item(itemName, itemIcon);

                    // Tính toán khoảng cách và hướng ngẫu nhiên
                    double angle = random.NextDouble() * 2 * Math.PI; // Góc ngẫu nhiên từ 0 đến 2π radians
                    float distance = random.Next(10, 40); // Khoảng cách ngẫu nhiên từ 10 đến 40 pixels
                    int offsetX = (int)(Math.Cos(angle) * distance);
                    int offsetY = (int)(Math.Sin(angle) * distance);

                    // Tạo DroppedItem tại vị trí offset
                    DroppedItem newDroppedItem = new DroppedItem(droppedItem, X + offsetX, Y + offsetY);

                    // Thêm DroppedItem vào GameObjectManager
                    GameObjectManager.Instance.AddDroppedItem(newDroppedItem);

                    Console.WriteLine($"[Info] Quặng tại ({X}, {Y}) đã rớt ra item: {itemName} tại ({newDroppedItem.X}, {newDroppedItem.Y}).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Lỗi khi tạo DroppedItem: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[Error] Không tìm thấy hình ảnh cho item: {itemImagePath}");
            }
        }
        public override void Draw(Graphics g)
        {
            if (isMined)
            {
                // Không vẽ quặng đã bị đào
                return;
            }

            if (Image != null)
            {
                g.DrawImage(Image, X, Y, Width, Height);
                // Bạn có thể tắt dòng Debug này để tránh spam console
                // Console.WriteLine($"[Debug] Vẽ quặng tại ({X}, {Y}) với kích thước ({Width}x{Height}).");
            }
            else
            {
                Console.WriteLine($"[Error] Quặng tại ({X}, {Y}) không có hình ảnh để vẽ.");
            }
        }
    }
}
