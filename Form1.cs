﻿using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Project_Game
{
    public partial class Form1 : Form
    {
        private Player player;
        private GameObjectManager objectManager;
        private MapManager mapManager;
        private Renderer renderer;
        private GameLogic gameLogic;
        private GameOver gameOver;
        private bool gameOverState = false;
        private bool needsRedraw = false;

        private InventoryManager inventoryManager;

        private Timer gameTimer;

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            // Bật double buffering để giảm hiện tượng nhấp nháy
            this.DoubleBuffered = true;

            // Tạo danh sách các đối tượng
            var obstacles = new List<GameObject>();

            var initialEnemies = new List<TestEnemy>();
            var initialChickens = new List<Chicken>
        {
        new Chicken("Chicken1", 150, 200, 100, 200),
        new Chicken("Chicken2", 400, 200, 350, 450)
        };

            var initialOres = new List<Ore>(); // Thêm danh sách Ores

            // Khởi tạo Player với danh sách Ores
            player = new Player(obstacles, initialEnemies, initialChickens, initialOres);
            player.OnHealthChanged += UpdateHealBar;

            objectManager = new GameObjectManager(player);
            objectManager.Obstacles.AddRange(obstacles);
            objectManager.Chickens.AddRange(initialChickens);
            objectManager.Ores.AddRange(initialOres); // Thêm Ores vào GameObjectManager

            var allEnemies = objectManager.Enemies.Cast<Enemy>().ToList();
            gameLogic = new GameLogic(player, allEnemies, objectManager.Obstacles);
            objectManager.SetGameLogic(gameLogic);

            mapManager = new MapManager(player, objectManager);
            renderer = new Renderer();

            objectManager.LoadMap1();
            player.SetObstacles(objectManager.Obstacles);

            this.KeyPreview = true;
            this.KeyPress += Form1_KeyPress;
            inventoryManager = new InventoryManager();

            UIManager.Initialize(this, inventoryManager, player);

            gameTimer = new Timer();
            gameTimer.Interval = 20; // khoảng 50 FPS
            gameTimer.Tick += TimerEvent;
            gameTimer.Start();
            Console.WriteLine("Game Timer started with interval 20ms.");

            this.Paint += FormPaintEvent;
            Console.WriteLine("Form Paint event handler registered.");
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'i' || e.KeyChar == 'I')
            {
                UIManager.Instance.showInventory = !UIManager.Instance.showInventory;
                Console.WriteLine($"Toggled Inventory via KeyPress: {UIManager.Instance.showInventory}");
                this.Invalidate();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameTimer.Stop();
            Console.WriteLine("Game Timer stopped.");
            Application.Exit();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            KeyIsDown(this, e);

            if (!gameOverState)
            {
                if (e.KeyCode == Keys.C)
                {
                    AttemptHarvest();
                }
                else if (e.KeyCode == Keys.X)
                {
                    AttemptChop();
                }
                else if (e.KeyCode == Keys.V)
                {
                    AttemptMineOres();
                }
            }
        }
        private void AttemptHarvest()
        {
            bool harvestedAny = false;

            // Kiểm tra xem người chơi có đứng gần bất kỳ cây nào không
            foreach (var tree in objectManager.Trees)
            {
                // Tính trung tâm của người chơi
                float playerCenterX = player.X + player.Width / 2f;
                float playerCenterY = player.Y + player.Height / 2f;

                // Tính trung tâm của cây
                float treeCenterX = tree.X + tree.Width / 2f;
                float treeCenterY = tree.Y + tree.Height / 2f;

                // Tính khoảng cách giữa trung tâm người chơi và trung tâm cây
                float deltaX = playerCenterX - treeCenterX;
                float deltaY = playerCenterY - treeCenterY;
                float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                float harvestDistance = 50f; // Khoảng cách tối đa để thu hoạch

                Console.WriteLine($"[Debug] Distance to tree at ({tree.X}, {tree.Y}): {distance}");

                if (distance <= harvestDistance)
                {
                    // Thực hiện thu hoạch
                    Item harvestedItem = tree.Harvest(inventoryManager);
                    if (harvestedItem != null)
                    {
                        Console.WriteLine($"Player đã thu hoạch {harvestedItem.Name} từ cây tại ({tree.X}, {tree.Y}).");
                        harvestedAny = true;
                    }
                    else
                    {
                        Console.WriteLine($"Không thể thu hoạch cây tại ({tree.X}, {tree.Y}). Có thể cây chưa trưởng thành hoặc inventory đã đầy.");
                    }
                }
            }

            if (!harvestedAny)
            {
                Console.WriteLine("[Info] Không có cây nào gần bạn để thu hoạch.");
            }

            Invalidate(); // Yêu cầu vẽ lại form
        }


        private void AttemptChop()
        {
            // Kiểm tra xem vũ khí hiện tại có phải là Axe hay không
            if (player.CurrentWeapon != "Axe")
            {
                Console.WriteLine("Only Axe can chop trees.");
                return;
            }

            bool choppedAny = false;

            // Kiểm tra xem người chơi có đứng gần bất kỳ cây nào không
            foreach (var tree in objectManager.Trees.ToList()) // Sử dụng ToList() để tránh lỗi khi xoá trong vòng lặp
            {
                // Tính trung tâm của người chơi
                float playerCenterX = player.X + player.Width / 2f;
                float playerCenterY = player.Y + player.Height / 2f;

                // Tính trung tâm của cây
                float treeCenterX = tree.X + tree.Width / 2f;
                float treeCenterY = tree.Y + tree.Height / 2f;

                // Tính khoảng cách giữa trung tâm người chơi và trung tâm cây
                float deltaX = playerCenterX - treeCenterX;
                float deltaY = playerCenterY - treeCenterY;
                float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                float chopDistance = 50f; // Khoảng cách tối đa để chặt

                Console.WriteLine($"[Debug] Distance to tree at ({tree.X}, {tree.Y}): {distance}");

                if (distance <= chopDistance)
                {
                    // Thực hiện chặt cây
                    List<Item> choppedItems = tree.Chop();
                    if (choppedItems != null && choppedItems.Count > 0)
                    {
                        foreach (var item in choppedItems)
                        {
                            bool added = inventoryManager.AddItem(item);
                            if (added)
                            {
                                Console.WriteLine($"Player đã nhận được {item.Name} từ việc chặt cây tại ({tree.X}, {tree.Y}).");
                            }
                            else
                            {
                                Console.WriteLine($"Inventory đã đầy. Không thể thêm {item.Name} từ việc chặt cây tại ({tree.X}, {tree.Y}).");
                            }
                        }
                        choppedAny = true;

                        // Kích hoạt hoạt ảnh chặt cây
                        player.Chop();
                    }
                    else
                    {
                        Console.WriteLine($"[Info] Không có item nào được rớt từ việc chặt cây tại ({tree.X}, {tree.Y}).");
                    }
                }
            }

            if (choppedAny)
            {
                Console.WriteLine("[Info] Đã chặt ít nhất một cây và nhận được item.");
            }
            else
            {
                Console.WriteLine("[Info] Không có cây nào gần bạn để chặt hoặc không có item nào được rớt.");
            }

            Invalidate(); // Yêu cầu vẽ lại form
        }
        private void AttemptMineOres()
        {
            // Kiểm tra xem vũ khí hiện tại có phải là Pickaxe hay không
            if (player.CurrentWeapon != "Pickaxe")
            {
                Console.WriteLine("Only Pickaxe can mine ores.");
                return;
            }

            // Thực hiện đào quặng bằng Player
            player.MineOres(objectManager.Ores);

            Invalidate(); // Yêu cầu vẽ lại form
        }



        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            KeyIsUp(this, e);
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (!gameOverState)
            {
                Console.WriteLine($"Key pressed: {e.KeyCode}");

                if (e.KeyCode == Keys.Left && !player.IsBlockedLeft)
                {
                    player.GoLeft = true;
                    player.GoRight = player.GoUp = player.GoDown = false;
                }
                else if (e.KeyCode == Keys.Right && !player.IsBlockedRight)
                {
                    player.GoRight = true;
                    player.GoLeft = player.GoUp = player.GoDown = false;
                }
                else if (e.KeyCode == Keys.Up && !player.IsBlockedUp)
                {
                    player.GoUp = true;
                    player.GoLeft = player.GoRight = player.GoDown = false;
                }
                else if (e.KeyCode == Keys.Down && !player.IsBlockedDown)
                {
                    player.GoDown = true;
                    player.GoLeft = player.GoRight = player.GoUp = false;
                }

                if (e.KeyCode == Keys.D1)
                {
                    UIManager.Instance.SelectedBarIndex = 0;
                    var item = inventoryManager.bar[0];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }
                if (e.KeyCode == Keys.D2)
                {
                    UIManager.Instance.SelectedBarIndex = 1;
                    var item = inventoryManager.bar[1];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }
                if (e.KeyCode == Keys.D3)
                {
                    UIManager.Instance.SelectedBarIndex = 2;
                    var item = inventoryManager.bar[2];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }
                if (e.KeyCode == Keys.D4)
                {
                    UIManager.Instance.SelectedBarIndex = 3;
                    var item = inventoryManager.bar[3];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }
                if (e.KeyCode == Keys.D5)
                {
                    UIManager.Instance.SelectedBarIndex = 4;
                    var item = inventoryManager.bar[4];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }

                UIManager.Instance.OnKeyDown(e);
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (!gameOverState)
            {
                if (e.KeyCode == Keys.Left)
                {
                    player.GoLeft = false;
                    player.UnblockDirection("Left");
                }
                if (e.KeyCode == Keys.Right)
                {
                    player.GoRight = false;
                    player.UnblockDirection("Right");
                }
                if (e.KeyCode == Keys.Up)
                {
                    player.GoUp = false;
                    player.UnblockDirection("Up");
                }
                if (e.KeyCode == Keys.Down)
                {
                    player.GoDown = false;
                    player.UnblockDirection("Down");
                }
            }
        }

        private void UpdateHealBar(int newHealth)
        {
            if (newHealth >= healBar.Minimum && newHealth <= healBar.Maximum)
            {
                healBar.Value = newHealth;
            }

            if (newHealth <= 0 && !gameOverState)
            {
                gameOverState = true;
                EndGame(newHealth, healBar);
            }
        }

        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            renderer.Render(
                e.Graphics,
                mapManager.CurrentBg,
                player,
                objectManager,
                gameOverState

            );
            objectManager.RenderAll(e.Graphics);
            UIManager.Instance.Draw(e.Graphics);
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            if (!gameOverState)
            {
                if (player.IsAttacking)
                {
                    player.UpdateAttack();
                }
                else
                {
                    player.Move();
                }

                gameLogic.TimerEvent(sender, e, healBar);
                objectManager.Enemies.RemoveAll(en => en.ShouldRemove);

                needsRedraw = true;
                objectManager.UpdateAll(player);

                // Người chơi nhặt item
                player.PickupItems(objectManager.DroppedItems, inventoryManager);

                // Kiểm tra khoảng cách và bắt đầu hiệu ứng bay vào người chơi
                CheckProximityForItems();

                mapManager.UpdateMap();
            }

            if (needsRedraw)
            {
                Invalidate();
                needsRedraw = false;
            }
        }




        private void CheckProximityForItems()
        {
            float proximityThreshold = 50f; // Khoảng cách để bắt đầu bay vào người chơi

            foreach (var item in objectManager.DroppedItems.ToList()) // Sử dụng ToList() để tránh lỗi khi xoá trong vòng lặp
            {
                if (!item.IsFlying)
                {
                    float deltaX = player.X - item.X;
                    float deltaY = player.Y - item.Y;
                    float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                    if (distance <= proximityThreshold)
                    {
                        item.StartFlyingTowardsPlayer(player);
                    }
                }
            }
        }



        private void ResetGameAction()
        {
            player.ResetPlayer();
            objectManager.Enemies.Clear();
            objectManager.Chickens.Clear();
            objectManager.AnimatedObjects.Clear();

            gameLogic.SetEnemies(objectManager.Enemies.Cast<Enemy>().ToList());

            gameOverState = false;
            healBar.Value = 100;

            this.KeyPreview = true;
            gameLogic.ResetGameState();

            mapManager.ResetMap();
            objectManager.LoadMap1();
            player.SetObstacles(objectManager.Obstacles);

            player.playerX = 393;
            player.playerY = 410;

            Invalidate();
        }

        public void EndGame(int currentHealth, ProgressBar healBar)
        {
            gameOverState = true;

            MessageBox.Show("Game Over! You have been defeated!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (gameOver == null)
                gameOver = new GameOver(gameOverTimer, ResetGameAction, Invalidate);
            gameOver.ResetGame();

            healBar.Value = player.MaxHealth;
            ResetGameAction();
            mapManager.SwitchMap(1, new Point(393, 410));
        }

        private void GameOverTimer_Tick(object sender, EventArgs e)
        {
            gameOverTimer.Stop();
            ResetGameAction();
            Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void FormMouseClick(object sender, MouseEventArgs e)
        {
            if (gameOverState || player.IsAttacking) return;

            if (UIManager.Instance.IsClickOnUI(e.X, e.Y))
            {
                return;
            }

            string direction = player.CurrentDirection;
            if (string.IsNullOrEmpty(direction))
            {
                direction = "Down";
            }

            int attackRange = 50;
            Rectangle attackArea = new Rectangle();
            switch (direction)
            {
                case "Left":
                    attackArea = new Rectangle(player.playerX - attackRange, player.playerY, attackRange, player.playerHeight);
                    break;
                case "Right":
                    attackArea = new Rectangle(player.playerX + player.playerWidth, player.playerY, attackRange, player.playerHeight);
                    break;
                case "Up":
                    attackArea = new Rectangle(player.playerX, player.playerY - attackRange, player.playerWidth, attackRange);
                    break;
                case "Down":
                    attackArea = new Rectangle(player.playerX, player.playerY + player.playerHeight, player.playerWidth, attackRange);
                    break;
                default:
                    attackArea = new Rectangle(player.playerX, player.playerY + player.playerHeight, player.playerWidth, attackRange);
                    break;
            }

            List<Enemy> enemiesToAttack = new List<Enemy>();
            foreach (var en in objectManager.Enemies)
            {
                if (!en.IsDead())
                {
                    Rectangle enemyRect = new Rectangle(en.X, en.Y, en.Width, en.Height);
                    if (attackArea.IntersectsWith(enemyRect))
                    {
                        enemiesToAttack.Add(en);
                    }
                }
            }
            if (enemiesToAttack.Count > 0)
            {
                List<Enemy> enemiesToAttackList = enemiesToAttack.Cast<Enemy>().ToList();
                player.PerformAttack(enemiesToAttackList);
                Console.WriteLine($"Player attacked {enemiesToAttackList.Count} enemies.");
            }
            else
            {
                player.PerformAttack(new List<Enemy>());
                Console.WriteLine("Player performed attack, but no enemies were hit.");
            }
            Invalidate();
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!gameOverState)
            {
                UIManager.Instance.OnMouseDown(e);
                if (!UIManager.Instance.IsDraggingItem && !UIManager.Instance.IsClickOnUI(e.X, e.Y))
                {
                    PerformAttack(e);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UIManager.Instance.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            UIManager.Instance.OnMouseUp(e);
        }

        private void PerformAttack(MouseEventArgs e)
        {
            string direction = player.CurrentDirection;
            if (string.IsNullOrEmpty(direction))
            {
                direction = "Down";
            }

            int attackRange = player.AttackRange;

            Rectangle attackArea = new Rectangle();
            switch (direction)
            {
                case "Left":
                    attackArea = new Rectangle(player.playerX - attackRange, player.playerY, attackRange, player.playerHeight);
                    break;
                case "Right":
                    attackArea = new Rectangle(player.playerX + player.playerWidth, player.playerY, attackRange, player.playerHeight);
                    break;
                case "Up":
                    attackArea = new Rectangle(player.playerX, player.playerY - attackRange, player.playerWidth, attackRange);
                    break;
                case "Down":
                    attackArea = new Rectangle(player.playerX, player.playerY + player.playerHeight, player.playerWidth, attackRange);
                    break;
                default:
                    attackArea = new Rectangle(player.playerX, player.playerY + player.playerHeight, player.playerWidth, attackRange);
                    break;
            }

            List<Enemy> enemiesToAttack = new List<Enemy>();
            foreach (var en in objectManager.Enemies)
            {
                if (!en.IsDead())
                {
                    Rectangle enemyRect = new Rectangle(en.X, en.Y, en.Width, en.Height);
                    if (attackArea.IntersectsWith(enemyRect))
                    {
                        enemiesToAttack.Add(en);
                    }
                }
            }

            if (enemiesToAttack.Count > 0)
            {
                List<Enemy> enemiesToAttackList = enemiesToAttack.Cast<Enemy>().ToList();
                player.PerformAttack(enemiesToAttackList);
                Console.WriteLine($"Player attacked {enemiesToAttackList.Count} enemies.");
            }
            else
            {
                player.PerformAttack(new List<Enemy>());
                Console.WriteLine("Player performed attack, but no enemies were hit.");
            }

            Invalidate();
        }
    }
}