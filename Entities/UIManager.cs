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
        private Player player; // Thêm trường này

        private int slotSize = 32;
        private int invStartX = 400;
        private int invStartY = 400;
        private int barStartX = 630 /2 ;
        private int barStartY = 550 ;

        private Image inventoryUI;
        private Image itemBarUI;

        public bool showInventory = false;
        // public int selectedBarIndex = 0; // Xóa dòng này nếu không cần thiết
        public int SelectedBarIndex { get; set; } = 0; // Sử dụng property này

        public bool IsDraggingItem { get { return invManager.isDragging; } }

        public UIManager(Form form, InventoryManager invManager, Player player) // Sửa constructor
        {
            this.form = form;
            this.invManager = invManager;
            this.player = player; // Gán đối tượng Player
            LoadUIImages();
        }

        private void LoadUIImages()
        {
            string uiPath = Path.Combine(Application.StartupPath, "Assets", "PlayerUI");

            string invUIPath = Path.Combine(uiPath, "InventoryUI.png");
            if (File.Exists(invUIPath))
                inventoryUI = Image.FromFile(invUIPath);

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
            // Vẽ ItemBarUI background
            if (itemBarUI != null)
            {
                g.DrawImage(itemBarUI, barStartX, barStartY, slotSize * 5, slotSize);
            }

            // Vẽ items trong bar
            for (int i = 0; i < 5; i++)
            {
                var rect = GetBarSlotRect(i);
                if (invManager.bar[i] != null && invManager.bar[i].Icon != null)
                {
                    g.DrawImage(invManager.bar[i].Icon, rect);
                }
                if (i == SelectedBarIndex)
                {
                    g.DrawRectangle(Pens.Yellow, rect); // Highlight selected slot
                }
            }

            // Vẽ Inventory nếu mở
            if (showInventory && inventoryUI != null)
            {
                g.DrawImage(inventoryUI, invStartX, invStartY, slotSize * 5, slotSize * 5);
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

            // Vẽ item đang kéo thả
            if (invManager.isDragging && invManager.draggingItem != null)
            {
                g.DrawImage(invManager.draggingItem.Icon, dragX - slotSize / 2, dragY - slotSize / 2, slotSize, slotSize);
            }
        }

        public int dragX, dragY;

        public void OnMouseDown(MouseEventArgs e)
        {
            // Check bar
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

            // Check Inventory nếu mở
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

            // Thả vào inventory nếu mở
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
                showInventory = !showInventory;
                Console.WriteLine("Toggled Inventory: " + showInventory);
                form.Invalidate();
            }

            if (e.KeyCode == Keys.D1)
            {
                SelectedBarIndex = 0;
                var item = invManager.bar[0];
                if (item != null) player.SetCurrentWeapon(item.Name); // Sử dụng player đã được định nghĩa
            }
            if (e.KeyCode == Keys.D2)
            {
                SelectedBarIndex = 1; // Pickaxe is now in slot 2
                var item = invManager.bar[1];
                if (item != null) player.SetCurrentWeapon(item.Name);
            }
            if (e.KeyCode == Keys.D3)
            {
                SelectedBarIndex = 2; // Axe is now in slot 3
                var item = invManager.bar[2];
                if (item != null) player.SetCurrentWeapon(item.Name);
            }
            if (e.KeyCode == Keys.D4)
            {
                SelectedBarIndex = 3;
                var item = invManager.bar[3];
                if (item != null) player.SetCurrentWeapon(item.Name);
            }
            if (e.KeyCode == Keys.D5)
            {
                SelectedBarIndex = 4;
                var item = invManager.bar[4];
                if (item != null) player.SetCurrentWeapon(item.Name);
            }

            form.Invalidate();
        }

        // Hàm kiểm tra click có nằm trên UI (Inventory/Bar) không
        public bool IsClickOnUI(int mouseX, int mouseY)
        {
            // Vùng Item Bar
            Rectangle barRect = new Rectangle(barStartX, barStartY, slotSize * 5, slotSize);
            if (barRect.Contains(mouseX, mouseY))
                return true;

            // Vùng Inventory (nếu đang mở)
            if (showInventory)
            {
                Rectangle invRect = new Rectangle(invStartX, invStartY, slotSize * 5, slotSize * 5);
                if (invRect.Contains(mouseX, mouseY))
                    return true;
            }

            return false;
        }
    }
}
