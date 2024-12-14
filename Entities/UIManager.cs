using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Project_Game.Entities
{
    public class UIManager
    {
        private InventoryManager invManager;
        private Form form;

        // Vị trí và kích thước ô
        private int slotSize = 32;
        private int invStartX = 100;
        private int invStartY = 100;
        private int barStartX = 100;
        private int barStartY = 300;

        // UI images
        private Image inventoryUI;
        private Image itemBarUI;

        // Kiểm soát hiển thị Inventory
        public bool showInventory = false;

        // Chỉ số ô đang chọn trên bar (0 đến 4)
        public int selectedBarIndex = 0;

        // Vị trí chuột khi drag item
        public int dragX, dragY;

        public UIManager(Form form, InventoryManager invManager)
        {
            this.form = form;
            this.invManager = invManager;

            LoadUIImages();
        }
        public InventoryManager InvManager
        {
            get { return invManager; }
        }
        public bool IsDraggingItem
        {
            get { return invManager.isDragging; }
        }
        private void LoadUIImages()
        {
            string uiPath = Path.Combine(Application.StartupPath, "Assets", "PlayerUI");

            string invUIPath = Path.Combine(uiPath, "InventoryUI.png");
            if (File.Exists(invUIPath))
                inventoryUI = Image.FromFile(invUIPath);
            Console.WriteLine($"invUIPath: {invUIPath}");

            string barUIPath = Path.Combine(uiPath, "ItemBarUI.png");
            if (File.Exists(barUIPath))
                itemBarUI = Image.FromFile(barUIPath);
        }

        private Rectangle GetInventorySlotRect(int row, int col)
        {
            int x = invStartX + col * slotSize;
            int y = invStartY + row * slotSize;
            return new Rectangle(x, y, slotSize, slotSize);
        }

        private Rectangle GetBarSlotRect(int index)
        {
            int x = barStartX + index * slotSize;
            int y = barStartY;
            return new Rectangle(x, y, slotSize, slotSize);
        }

        public void Draw(Graphics g)
        {
            // Vẽ ItemBarUI luôn
            if (itemBarUI != null)
            {
                // Giả sử ItemBarUI kích thước phù hợp với 5 ô: 5*slotSize=160x32
                g.DrawImage(itemBarUI, barStartX, barStartY, slotSize * 5, slotSize);
            }

            // Vẽ item trên bar
            for (int i = 0; i < 5; i++)
            {
                var rect = GetBarSlotRect(i);
                if (invManager.bar[i] != null && invManager.bar[i].Icon != null)
                {
                    g.DrawImage(invManager.bar[i].Icon, rect);
                }
                // Highlight ô đang chọn
                if (i == selectedBarIndex)
                {
                    g.DrawRectangle(Pens.Yellow, rect);
                }
            }

            // Vẽ InventoryUI nếu showInventory = true
            if (showInventory && inventoryUI != null)
            {
                // Giả sử InventoryUI đủ lớn cho 5x5 ô (160x160)
                g.DrawImage(inventoryUI, invStartX, invStartY, slotSize * 5, slotSize * 5);

                // Vẽ inventory items
                for (int r = 0; r < 5; r++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        if (invManager.inventoryGrid[r, c] != null && invManager.inventoryGrid[r, c].Icon != null)
                        {
                            var rect = GetInventorySlotRect(r, c);
                            g.DrawImage(invManager.inventoryGrid[r, c].Icon, rect);
                        }
                    }
                }
            }

            // Vẽ item đang kéo (nếu có)
            if (invManager.isDragging && invManager.draggingItem != null)
            {
                g.DrawImage(invManager.draggingItem.Icon, dragX - slotSize / 2, dragY - slotSize / 2, slotSize, slotSize);
            }
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            // Nếu ấn chuột vào bar
            for (int i = 0; i < 5; i++)
            {
                var rect = GetBarSlotRect(i);
                if (rect.Contains(e.Location))
                {
                    if (invManager.bar[i] != null)
                    {
                        invManager.draggingItem = invManager.bar[i];
                        invManager.bar[i] = null;
                        invManager.dragSourceRow = -1;
                        invManager.dragSourceCol = -1;
                        invManager.dragSourceBarIndex = i;
                        invManager.isDragging = true;
                        form.Invalidate();
                        return;
                    }
                }
            }

            // Nếu inventory đang mở thì cho kéo thả từ inventory
            if (showInventory)
            {
                for (int r = 0; r < 5; r++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        var rect = GetInventorySlotRect(r, c);
                        if (rect.Contains(e.Location))
                        {
                            if (invManager.inventoryGrid[r, c] != null)
                            {
                                invManager.draggingItem = invManager.inventoryGrid[r, c];
                                invManager.inventoryGrid[r, c] = null;
                                invManager.dragSourceRow = r;
                                invManager.dragSourceCol = c;
                                invManager.dragSourceBarIndex = -1;
                                invManager.isDragging = true;
                                form.Invalidate();
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            if (invManager.isDragging && invManager.draggingItem != null)
            {
                dragX = e.X;
                dragY = e.Y;
                form.Invalidate();
            }
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            if (!invManager.isDragging || invManager.draggingItem == null) return;

            bool placed = false;

            // Thả vào bar
            for (int i = 0; i < 5; i++)
            {
                var rect = GetBarSlotRect(i);
                if (rect.Contains(e.Location))
                {
                    if (invManager.bar[i] == null)
                    {
                        invManager.bar[i] = invManager.draggingItem;
                    }
                    else
                    {
                        var temp = invManager.bar[i];
                        invManager.bar[i] = invManager.draggingItem;
                        invManager.PutBackItem(temp);
                    }
                    placed = true;
                    break;
                }
            }

            // Nếu inventory đang mở, thả vào inventory
            if (!placed && showInventory)
            {
                for (int r = 0; r < 5; r++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        var rect = GetInventorySlotRect(r, c);
                        if (rect.Contains(e.Location))
                        {
                            if (invManager.inventoryGrid[r, c] == null)
                            {
                                invManager.inventoryGrid[r, c] = invManager.draggingItem;
                            }
                            else
                            {
                                var temp = invManager.inventoryGrid[r, c];
                                invManager.inventoryGrid[r, c] = invManager.draggingItem;
                                invManager.PutBackItem(temp);
                            }
                            placed = true;
                            break;
                        }
                    }
                    if (placed) break;
                }
            }

            if (!placed)
            {
                // Không đặt được, trả item lại
                invManager.PutBackItem(invManager.draggingItem);
            }

            invManager.draggingItem = null;
            invManager.isDragging = false;
            invManager.dragSourceRow = -1;
            invManager.dragSourceCol = -1;
            invManager.dragSourceBarIndex = -1;

            form.Invalidate();
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I)
            {
                // Toggle inventory
                showInventory = !showInventory;
                form.Invalidate();
                Console.WriteLine("Toggled Inventory: " + showInventory);

            }

            // Chọn slot trên bar (1-5)
            if (e.KeyCode == Keys.D1) selectedBarIndex = 0;
            if (e.KeyCode == Keys.D2) selectedBarIndex = 1;
            if (e.KeyCode == Keys.D3) selectedBarIndex = 2;
            if (e.KeyCode == Keys.D4) selectedBarIndex = 3;
            if (e.KeyCode == Keys.D5) selectedBarIndex = 4;

            form.Invalidate();
        }
    }
}
