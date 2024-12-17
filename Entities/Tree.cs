using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class Tree : GameObject
    {
        // Danh sách các hình ảnh cho từng giai đoạn phát triển của cây
        private List<Image> growthStages;
        private int currentStage;
        private int maxStage;

        // Thời gian giữa các giai đoạn (milliseconds)
        private int growthInterval;
        private DateTime lastGrowthTime;

        // Trạng thái của cây
        private bool isHarvested;

        // Chiều dài và chiều cao của cây
        public int TreeWidth { get; private set; }
        public int TreeHeight { get; private set; }

        // Tọa độ cố định
        public override int X { get; set; }
        public override int Y { get; set; }

        public Tree(int x, int y, List<Image> stages, int width, int height, int growthIntervalMilliseconds = 5000)
            : base(x, y, width, height, "Tree", 1) // Sử dụng width và height từ XML
        {
            if (stages == null || stages.Count != 3)
            {
                throw new ArgumentException("Cây cần có đúng 3 giai đoạn phát triển.");
            }

            X = x;
            Y = y;
            growthStages = stages;
            currentStage = 0;
            maxStage = growthStages.Count - 1;
            growthInterval = growthIntervalMilliseconds;
            lastGrowthTime = DateTime.Now;
            isHarvested = false;
            TreeWidth = width;
            TreeHeight = height;
        }

        public void UpdateTree()
        {
            if (isHarvested)
            {
                // Nếu cây đã được thu hoạch, không cần cập nhật thêm
                return;
            }

            var now = DateTime.Now;
            if ((now - lastGrowthTime).TotalMilliseconds >= growthInterval)
            {
                if (currentStage < maxStage)
                {
                    currentStage++;
                    lastGrowthTime = now;
                    Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã phát triển đến giai đoạn {currentStage + 1}.");
                }
                else
                {
                    Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã đạt giai đoạn tối đa.");
                }
            }
        }

        public Item Harvest(InventoryManager inventory)
        {
            if (currentStage < maxStage)
            {
                Console.WriteLine($"[Warning] Cây tại ({X}, {Y}) chưa đạt giai đoạn tối đa để thu hoạch.");
                return null;
            }

            if (isHarvested)
            {
                Console.WriteLine($"[Warning] Cây tại ({X}, {Y}) đã được thu hoạch rồi.");
                return null;
            }

            string itemName = "Fruit";
            string itemImagePath = Path.Combine("Assets", "Items", "Fruit", "fruit.png");

            if (File.Exists(itemImagePath))
            {
                try
                {
                    Image itemIcon = Image.FromFile(itemImagePath);
                    Item harvestedItem = new Item(itemName, itemIcon);

                    bool added = inventory.AddItem(harvestedItem);
                    if (added)
                    {
                        isHarvested = true;
                        Console.WriteLine($"[Info] Đã thu hoạch {itemName} từ cây tại ({X}, {Y}).");

                        // Đặt lại cây sau khi thu hoạch
                        ResetTree();

                        return harvestedItem;
                    }
                    else
                    {
                        Console.WriteLine($"[Error] Inventory đã đầy. Không thể thu hoạch {itemName}.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Lỗi khi thu hoạch item: {ex.Message}");
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"[Error] Không tìm thấy hình ảnh cho item: {itemImagePath}");
                return null;
            }
        }

        public List<Item> Chop()
        {
            if (currentStage == maxStage && !isHarvested)
            {
                // Giảm giai đoạn để biểu thị việc chặt cây
                currentStage = maxStage - 2;
                isHarvested = true;
                Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã bị chặt.");

                // Tạo các vật phẩm rơi xuống
                List<Item> droppedItems = new List<Item>();

                // Ví dụ: Rơi xuống một cây gỗ
                string itemName = "Wood";
                string itemImagePath = Path.Combine("Assets", "Items", "Wood", "wood.png");

                if (File.Exists(itemImagePath))
                {
                    try
                    {
                        Image itemIcon = Image.FromFile(itemImagePath);
                        Item wood = new Item(itemName, itemIcon);
                        droppedItems.Add(wood);
                        Console.WriteLine($"[Info] Đã tạo vật phẩm: {itemName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error] Lỗi khi tải hình ảnh vật phẩm: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"[Error] Không tìm thấy hình ảnh cho vật phẩm: {itemImagePath}");
                }

                return droppedItems;
            }
            else
            {
                Console.WriteLine($"[Warning] Cây tại ({X}, {Y}) không thể chặt.");
                return new List<Item>();
            }
        }

        private void ResetTree()
        {
            currentStage = 0;
            isHarvested = false;
            lastGrowthTime = DateTime.Now;
            Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã được tái sinh và bắt đầu phát triển lại.");
        }


        public void Draw(Graphics g)
        {
            if (currentStage < 0 || currentStage >= growthStages.Count)
            {
                Console.WriteLine($"[Error] currentStage {currentStage} is out of range for Tree tại ({X}, {Y}).");
                return; // Hoặc vẽ một hình ảnh mặc định
            }

            Image currentImage = growthStages[currentStage];
            if (currentImage != null)
            {
                g.DrawImage(currentImage, X, Y, TreeWidth, TreeHeight); // Sử dụng TreeWidth và TreeHeight
                Console.WriteLine($"[Debug] Vẽ cây tại ({X}, {Y}) ở giai đoạn {currentStage + 1} với kích thước ({TreeWidth}x{TreeHeight}).");
            }
            else
            {
                Console.WriteLine($"[Error] Cây tại ({X}, {Y}) không có hình ảnh để vẽ.");
            }
        }
    }
}
