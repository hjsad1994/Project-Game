using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Project_Game.Entities
{
    public class InventoryManager
    {
        public Item[,] inventoryGrid = new Item[5, 5];
        public Item[] bar = new Item[5]; // 5 slot cho ItemBarUI

        public Item draggingItem = null;
        public bool isDragging = false;
        public int dragSourceRow = -1, dragSourceCol = -1;
        public int dragSourceBarIndex = -1;

        public InventoryManager()
        {
            LoadOresItems();
            LoadToolItems();
        }

        /// <summary>
        /// Tải các item ores từ thư mục Assets\Items\Ores vào inventoryGrid.
        /// </summary>
        private void LoadOresItems()
        {
            List<Item> oreItems = new List<Item>();
            string oresPath = Path.Combine(Application.StartupPath, "Assets", "Items", "Ores");
            for (int i = 1; i <= 8; i++)
            {
                string fileName = $"ores_{i}.png";
                string fullPath = Path.Combine(oresPath, fileName);
                if (File.Exists(fullPath))
                {
                    try
                    {
                        Image img = Image.FromFile(fullPath);
                        Item oreItem = new Item($"Ores_{i}", img);
                        oreItems.Add(oreItem);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi tải hình ảnh {fullPath}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy file: {fullPath}");
                }
            }

            // Gán các ore items vào inventoryGrid
            if (oreItems.Count > 0) inventoryGrid[0, 0] = oreItems[0];
            if (oreItems.Count > 1) inventoryGrid[0, 1] = oreItems[1];
            if (oreItems.Count > 2) inventoryGrid[0, 2] = oreItems[2];
            if (oreItems.Count > 3) inventoryGrid[0, 3] = oreItems[3];
            if (oreItems.Count > 4) inventoryGrid[0, 4] = oreItems[4];
            if (oreItems.Count > 5) inventoryGrid[1, 0] = oreItems[5];
            if (oreItems.Count > 6) inventoryGrid[1, 1] = oreItems[6];
            if (oreItems.Count > 7) inventoryGrid[1, 2] = oreItems[7];
            // Bạn có thể tiếp tục gán các item khác nếu có
        }

        /// <summary>
        /// Tải các tool items từ thư mục Assets\Items\Tool_Icons_Outline_cuts vào các slot cụ thể trong bar.
        /// </summary>
        private void LoadToolItems()
        {
            string toolPath = Path.Combine(Application.StartupPath, "Assets", "Items", "Tool_Icons_Outline_cuts");

            try
            {
                // Load Sword.png into slot 0 (Slot 1)
                string swordPath = Path.Combine(toolPath, "Sword.png");
                if (File.Exists(swordPath))
                {
                    Image swordImg = Image.FromFile(swordPath);
                    bar[0] = new Item("Sword", swordImg);
                }
                else
                {
                    Console.WriteLine($"File not found: {swordPath}");
                }

                // Load Pickaxe.png into slot 1 (Slot 2)
                string pickaxePath = Path.Combine(toolPath, "Pickaxe.png");
                if (File.Exists(pickaxePath))
                {
                    Image pickaxeImg = Image.FromFile(pickaxePath);
                    bar[1] = new Item("Pickaxe", pickaxeImg); // Assign Pickaxe to slot 2
                }
                else
                {
                    Console.WriteLine($"File not found: {pickaxePath}");
                }

                // Load Axe.png into slot 2 (Slot 3)
                string axePath = Path.Combine(toolPath, "Axe.png");
                if (File.Exists(axePath))
                {
                    Image axeImg = Image.FromFile(axePath);
                    bar[2] = new Item("Axe", axeImg); // Assign Axe to slot 3
                }
                else
                {
                    Console.WriteLine($"File not found: {axePath}");
                }

                // Load Watering-can.png into slot 3 (Slot 4)
                string wateringCanPath = Path.Combine(toolPath, "Watering-can.png");
                if (File.Exists(wateringCanPath))
                {
                    Image wateringCanImg = Image.FromFile(wateringCanPath);
                    bar[3] = new Item("Watering-can", wateringCanImg);
                }
                else
                {
                    Console.WriteLine($"File not found: {wateringCanPath}");
                }

                // Slot 4 (Slot 5) can remain empty or load another item as needed
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tool items: {ex.Message}");
            }
        }

        /// <summary>
        /// Đặt lại item vào vị trí ban đầu nếu việc kéo thả không thành công.
        /// </summary>
        /// <param name="item">Item cần đặt lại.</param>
        public void PutBackItem(Item item)
        {
            if (dragSourceRow != -1 && dragSourceCol != -1)
            {
                inventoryGrid[dragSourceRow, dragSourceCol] = item;
            }
            else if (dragSourceBarIndex != -1)
            {
                bar[dragSourceBarIndex] = item;
            }
        }
public bool AddItem(Item item)
{
    // Thêm vào inventoryGrid
    for (int r = 0; r < inventoryGrid.GetLength(0); r++)
    {
        for (int c = 0; c < inventoryGrid.GetLength(1); c++)
        {
            if (inventoryGrid[r, c] == null)
            {
                inventoryGrid[r, c] = item;
                Console.WriteLine($"Item {item.Name} thêm vào inventory tại ({r}, {c})");
                return true;
            }
        }
    }

    // Nếu inventoryGrid đầy, có thể thêm vào bar nếu cần
    for (int i = 0; i < bar.Length; i++)
    {
        if (bar[i] == null)
        {
            bar[i] = item;
            Console.WriteLine($"Item {item.Name} thêm vào bar tại slot {i}");
            return true;
        }
    }

    // Nếu không có chỗ trống, trả về false
    Console.WriteLine($"Inventory đầy. Không thể thêm item: {item.Name}");
    return false;
}
    }
}
