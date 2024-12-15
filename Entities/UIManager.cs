using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Project_Game.Entities
{
    public class UIManager
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("UIManager chưa được khởi tạo. Vui lòng gọi UIManager.Initialize() trước.");
                return _instance;
            }
        }

        private InventoryManager invManager;
        private Form form;
        private Player player;

        private int slotSize = 32;
        private int invStartX = 100;
        private int invStartY = 100;
        private int barStartX = 315; // 630 / 2
        private int barStartY = 550;

        private Image inventoryUI;
        private Image itemBarUI;

        public bool showInventory = false;
        public int SelectedBarIndex { get; set; } = 0;

        public bool IsDraggingItem { get { return invManager.isDragging; } }

        // Private constructor để ngăn chặn tạo instance từ bên ngoài
        private UIManager() { }

        /// <summary>
        /// Phương thức để khởi tạo singleton instance của UIManager.
        /// Chỉ nên được gọi một lần từ Form1.
        /// </summary>
        public static void Initialize(Form form, InventoryManager invManager, Player player)
        {
            if (_instance != null)
                throw new Exception("UIManager đã được khởi tạo.");

            _instance = new UIManager();
            _instance.form = form;
            _instance.invManager = invManager;
            _instance.player = player;
            _instance.LoadUIImages();
        }

        private void LoadUIImages()
        {
            string uiPath = Path.Combine(Application.StartupPath, "Assets", "PlayerUI");

            string invUIPath = Path.Combine(uiPath, "InventoryUI.png");
            if (File.Exists(invUIPath))
            {
                try
                {
                    inventoryUI = Image.FromFile(invUIPath);
                    Console.WriteLine("InventoryUI loaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading InventoryUI: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"InventoryUI not found at path: {invUIPath}");
            }

            string barUIPath = Path.Combine(uiPath, "ItemBarUI.png");
            if (File.Exists(barUIPath))
            {
                try
                {
                    itemBarUI = Image.FromFile(barUIPath);
                    Console.WriteLine("ItemBarUI loaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading ItemBarUI: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"ItemBarUI not found at path: {barUIPath}");
            }
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
            Console.WriteLine("UIManager.Draw called.");

            // Vẽ ItemBarUI background
            if (itemBarUI != null)
            {
                g.DrawImage(itemBarUI, barStartX, barStartY, slotSize * 5, slotSize);
                Console.WriteLine("Drawn ItemBarUI.");
            }
            else
            {
                Console.WriteLine("ItemBarUI is null, cannot draw.");
            }

            // Vẽ items trong bar
            for (int i = 0; i < 5; i++)
            {
                var rect = GetBarSlotRect(i);
                if (invManager.bar[i] != null && invManager.bar[i].Icon != null)
                {
                    g.DrawImage(invManager.bar[i].Icon, rect);
                    Console.WriteLine($"Drawn bar item: {invManager.bar[i].Name} at slot {i}");
                }
                if (i == SelectedBarIndex)
                {
                    g.DrawRectangle(Pens.Yellow, rect); // Highlight selected slot
                    Console.WriteLine($"Highlighted selected bar slot: {i}");
                }
            }

            // Log giá trị showInventory
            Console.WriteLine($"showInventory is {showInventory}");

            // Vẽ Inventory nếu mở
            if (showInventory)
            {
                Console.WriteLine("showInventory condition is True.");
                if (inventoryUI != null)
                {
                    g.DrawImage(inventoryUI, invStartX, invStartY, slotSize * 5, slotSize * 5);
                    Console.WriteLine("Drawn InventoryUI.");

                    for (int r = 0; r < 5; r++)
                    {
                        for (int c = 0; c < 5; c++)
                        {
                            if (invManager.inventoryGrid[r, c] != null && invManager.inventoryGrid[r, c].Icon != null)
                            {
                                var rect = GetInventorySlotRect(r, c);
                                g.DrawImage(invManager.inventoryGrid[r, c].Icon, rect);
                                Console.WriteLine($"Drawn inventory item: {invManager.inventoryGrid[r, c].Name} at ({r}, {c})");
                            }
                        }
                    }
                }
                else
                {
                    // Thử vẽ hình chữ nhật màu nếu InventoryUI không được tải
                    g.FillRectangle(Brushes.LightBlue, invStartX, invStartY, slotSize * 5, slotSize * 5);
                    g.DrawRectangle(Pens.Black, invStartX, invStartY, slotSize * 5, slotSize * 5);
                    Console.WriteLine("Drawn Inventory Rectangle.");
                }
            }
            else
            {
                Console.WriteLine("showInventory is false. Inventory not drawn.");
            }

            // Vẽ item đang kéo thả
            if (invManager.isDragging && invManager.draggingItem != null)
            {
                g.DrawImage(invManager.draggingItem.Icon, dragX - slotSize / 2, dragY - slotSize / 2, slotSize, slotSize);
                Console.WriteLine($"Drawing dragging item: {invManager.draggingItem.Name} at ({dragX}, {dragY})");
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
                Console.WriteLine($"Toggled Inventory: {showInventory}");
                form.Invalidate(); // Yêu cầu vẽ lại form
            }

            if (e.KeyCode == Keys.D1)
            {
                SelectedBarIndex = 0;
                var item = invManager.bar[0];
                if (item != null) player.SetCurrentWeapon(item.Name);
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

            Console.WriteLine($"After KeyDown - showInventory: {showInventory}, SelectedBarIndex: {SelectedBarIndex}");
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
