using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class Tree : GameObject
    {
        // Danh sách các giai đoạn phát triển của cây
        private List<TreeStage> growthStages;
        private int currentStage;
        private int maxStage;
        public Image Image { get; set; } // Thêm thuộc tính Image

        // Thời gian giữa các giai đoạn (milliseconds)
        private int growthInterval;
        private DateTime lastGrowthTime;

        // Thời gian hồi phục sau khi chặt (milliseconds)
        private int recoveryInterval = 10000; // 10 giây
        private bool isChopped = false;
        private DateTime chopTime;

        // Trạng thái của cây
        private bool isHarvested;

        // Tọa độ cố định
        public override int X { get; set; }
        public override int Y { get; set; }

        public Tree(int x, int y, List<TreeStage> stages, int growthIntervalMilliseconds = 5000)
            : base(x, y, stages[0].Width, stages[0].Height, "Tree", 1) // Sử dụng width và height từ giai đoạn đầu tiên
        {
            if (stages == null || stages.Count == 0)
            {
                throw new ArgumentException("Cây cần có ít nhất 1 giai đoạn phát triển.");
            }

            X = x;
            Y = y;
            growthStages = stages;
            currentStage = 0;
            maxStage = growthStages.Count - 1;
            growthInterval = growthIntervalMilliseconds;
            lastGrowthTime = DateTime.Now;
            isHarvested = false;
        }

        public void UpdateTree()
        {
            if (isHarvested)
            {
                // Nếu cây đã bị chặt, kiểm tra thời gian hồi phục
                if (isChopped)
                {
                    var now = DateTime.Now;
                    if ((now - chopTime).TotalMilliseconds >= recoveryInterval)
                    {
                        ResetTree();
                        Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã hồi phục lại.");
                    }
                }
                else
                {
                    // Nếu cây chỉ được thu hoạch nhưng chưa bị chặt, không cần hồi phục
                    return;
                }
            }
            else
            {
                var now = DateTime.Now;
                if ((now - lastGrowthTime).TotalMilliseconds >= growthInterval)
                {
                    if (currentStage < maxStage)
                    {
                        currentStage++;
                        lastGrowthTime = now;
                        UpdateStage();
                        Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã phát triển đến giai đoạn {currentStage + 1}.");
                    }
                    else
                    {
                        Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã đạt giai đoạn tối đa.");
                    }
                }
            }
        }

        private void UpdateStage()
        {
            if (currentStage >= 0 && currentStage < growthStages.Count)
            {
                TreeStage current = growthStages[currentStage];
                this.Width = current.Width;
                this.Height = current.Height;
                this.Image = current.Image;
                Console.WriteLine($"[Info] Cập nhật cây tại ({X}, {Y}) sang giai đoạn {currentStage + 1} với kích thước ({Width}x{Height}).");
            }
            else
            {
                Console.WriteLine($"[Error] Giai đoạn {currentStage} không hợp lệ cho cây tại ({X}, {Y}).");
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
            if (currentStage == maxStage && !isHarvested && !isChopped)
            {
                // Giảm giai đoạn để biểu thị việc chặt cây
                currentStage = maxStage - 2 >= 0 ? maxStage - 2 : 0;
                isHarvested = true;
                isChopped = true;
                chopTime = DateTime.Now;
                UpdateStage();
                Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã bị chặt.");

                // Tạo các vật phẩm rơi xuống
                List<Item> droppedItems = new List<Item>();

                // Ví dụ: Rơi xuống một cây gỗ
                string itemName = "Fruit";
                string itemImagePath = Path.Combine("Assets", "Items", "Fruit", "fruit.png");

                if (File.Exists(itemImagePath))
                {
                    try
                    {
                        Image itemIcon = Image.FromFile(itemImagePath);
                        Item fruit = new Item(itemName, itemIcon);
                        droppedItems.Add(fruit);
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
            isChopped = false;
            lastGrowthTime = DateTime.Now;
            chopTime = DateTime.MinValue;
            UpdateStage();
            Console.WriteLine($"[Info] Cây tại ({X}, {Y}) đã được tái sinh và bắt đầu phát triển lại.");
        }

        public void Draw(Graphics g)
        {
            if (currentStage < 0 || currentStage >= growthStages.Count)
            {
                Console.WriteLine($"[Error] currentStage {currentStage} is out of range cho Tree tại ({X}, {Y}).");
                return; // Hoặc vẽ một hình ảnh mặc định
            }

            Image currentImage = growthStages[currentStage].Image;
            if (currentImage != null)
            {
                g.DrawImage(currentImage, X, Y, Width, Height); // Sử dụng Width và Height được cập nhật theo giai đoạn
                Console.WriteLine($"[Debug] Vẽ cây tại ({X}, {Y}) ở giai đoạn {currentStage + 1} với kích thước ({Width}x{Height}).");
            }
            else
            {
                Console.WriteLine($"[Error] Cây tại ({X}, {Y}) không có hình ảnh để vẽ.");
            }
        }
    }
}
