using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Project_Game.Entities
{
    public class InventoryManager
    {
        public Item[,] inventoryGrid = new Item[5, 5];
        public Item[] bar = new Item[5];

        public Item draggingItem = null;
        public bool isDragging = false;
        public int dragSourceRow = -1, dragSourceCol = -1;
        public int dragSourceBarIndex = -1;

        public InventoryManager()
        {
            LoadOresItems();
        }

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
                    Image img = Image.FromFile(fullPath);
                    Item oreItem = new Item($"Ores_{i}", img);
                    oreItems.Add(oreItem);
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy file: {fullPath}");
                }
            }

            if (oreItems.Count > 0) inventoryGrid[0, 0] = oreItems[0];
            if (oreItems.Count > 1) inventoryGrid[0, 1] = oreItems[1];
            if (oreItems.Count > 2) bar[0] = oreItems[2];
            if (oreItems.Count > 3) bar[1] = oreItems[3];
        }

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
    }
}
