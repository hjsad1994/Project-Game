using Project_Game.Entities;
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
        // private UIManager uiManager; // Không cần khai báo thêm nếu sử dụng Singleton

        private const int PlayerWidth = 50;
        private const int PlayerHeight = 50;

        private Timer gameTimer;

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            // Gán sự kiện MouseClick trở lại

            var obstacles = new List<GameObject>
            {
                new GameObject(Test1.Location.X, Test1.Location.Y, Test1.Width, Test1.Height, "Obstacle1"),
            };

            var initialEnemies = new List<TestEnemy>();
            var initialChickens = new List<Chicken>
            {
                new Chicken("Chicken1", 150, 200, 100, 200),
                new Chicken("Chicken2", 400, 200, 350, 450)
            };

            player = new Player(obstacles, initialEnemies, initialChickens);
            player.OnHealthChanged += UpdateHealBar;

            objectManager = new GameObjectManager(player);
            objectManager.Obstacles.AddRange(obstacles);
            objectManager.Chickens.AddRange(initialChickens);

            var allEnemies = objectManager.Enemies.Cast<Enemy>().ToList();
            gameLogic = new GameLogic(player, allEnemies, objectManager.Obstacles);
            objectManager.SetGameLogic(gameLogic);

            mapManager = new MapManager(player, objectManager);
            renderer = new Renderer();

            objectManager.LoadMap1();

            this.KeyPreview = true;
            this.KeyPress += Form1_KeyPress;
            inventoryManager = new InventoryManager();

            // Khởi tạo UIManager Singleton
            UIManager.Initialize(this, inventoryManager, player);
            // uiManager = UIManager.Instance; // Không cần gán lại

            // Khởi tạo và khởi động Timer
            gameTimer = new Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += TimerEvent;
            gameTimer.Start();
            Console.WriteLine("Game Timer started with interval 16ms.");

            // Đăng ký sự kiện Paint
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
                // Log phím được nhấn
                Console.WriteLine($"Key pressed: {e.KeyCode}");

                // Khi nhấn một hướng di chuyển, đặt các hướng khác thành false
                if (e.KeyCode == Keys.Left && !player.IsBlockedLeft)
                {
                    player.GoLeft = true;
                    player.GoRight = player.GoUp = player.GoDown = false;
                    Console.WriteLine("GoLeft set to true, others set to false");
                }
                else if (e.KeyCode == Keys.Right && !player.IsBlockedRight)
                {
                    player.GoRight = true;
                    player.GoLeft = player.GoUp = player.GoDown = false;
                    Console.WriteLine("GoRight set to true, others set to false");
                }
                else if (e.KeyCode == Keys.Up && !player.IsBlockedUp)
                {
                    player.GoUp = true;
                    player.GoLeft = player.GoRight = player.GoDown = false;
                    Console.WriteLine("GoUp set to true, others set to false");
                }
                else if (e.KeyCode == Keys.Down && !player.IsBlockedDown)
                {
                    player.GoDown = true;
                    player.GoLeft = player.GoRight = player.GoUp = false;
                    Console.WriteLine("GoDown set to true, others set to false");
                }

                // Các xử lý phím D1-D5 khác...
                if (e.KeyCode == Keys.D1)
                {
                    UIManager.Instance.SelectedBarIndex = 0;
                    var item = inventoryManager.bar[0];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                    Console.WriteLine($"SelectedBarIndex set to {UIManager.Instance.SelectedBarIndex} with item {item?.Name}");
                }
                if (e.KeyCode == Keys.D2)
                {
                    UIManager.Instance.SelectedBarIndex = 1; // Pickaxe is now in slot 2
                    var item = inventoryManager.bar[1];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                    Console.WriteLine($"SelectedBarIndex set to {UIManager.Instance.SelectedBarIndex} with item {item?.Name}");
                }
                if (e.KeyCode == Keys.D3)
                {
                    UIManager.Instance.SelectedBarIndex = 2; // Axe is now in slot 3
                    var item = inventoryManager.bar[2];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                    Console.WriteLine($"SelectedBarIndex set to {UIManager.Instance.SelectedBarIndex} with item {item?.Name}");
                }
                if (e.KeyCode == Keys.D4)
                {
                    UIManager.Instance.SelectedBarIndex = 3;
                    var item = inventoryManager.bar[3];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                    Console.WriteLine($"SelectedBarIndex set to {UIManager.Instance.SelectedBarIndex} with item {item?.Name}");
                }
                if (e.KeyCode == Keys.D5)
                {
                    UIManager.Instance.SelectedBarIndex = 4;
                    var item = inventoryManager.bar[4];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                    Console.WriteLine($"SelectedBarIndex set to {UIManager.Instance.SelectedBarIndex} with item {item?.Name}");
                }

                // Chuyển tiếp sự kiện phím xuống UIManager
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
                    Console.WriteLine("GoLeft set to false");
                }
                if (e.KeyCode == Keys.Right)
                {
                    player.GoRight = false;
                    player.UnblockDirection("Right");
                    Console.WriteLine("GoRight set to false");
                }
                if (e.KeyCode == Keys.Up)
                {
                    player.GoUp = false;
                    player.UnblockDirection("Up");
                    Console.WriteLine("GoUp set to false");
                }
                if (e.KeyCode == Keys.Down)
                {
                    player.GoDown = false;
                    player.UnblockDirection("Down");
                    Console.WriteLine("GoDown set to false");
                }
            }
        }

        private void UpdateHealBar(int newHealth)
        {
            if (newHealth >= healBar.Minimum && newHealth <= healBar.Maximum)
            {
                healBar.Value = newHealth;
                Console.WriteLine($"healBar.Value updated to {newHealth}");
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
                objectManager.Enemies,
                objectManager.Chickens,
                objectManager.AnimatedObjects,
                objectManager.Kapybaras,
                gameOverState
            );

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

                mapManager.UpdateMap();
            }

            if (needsRedraw)
            {
                Invalidate();
                needsRedraw = false;
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

            player.playerX = 393;
            player.playerY = 410;
            Console.WriteLine($"Player reset to starting position: ({player.playerX}, {player.playerY})");

            Invalidate();
        }

        public void EndGame(int currentHealth, ProgressBar healBar)
        {
            Console.WriteLine($"EndGame called with currentHealth = {currentHealth}");
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

        private void Test1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // Giữ lại hàm FormMouseClick
        private void FormMouseClick(object sender, MouseEventArgs e)
        {
            if (gameOverState || player.IsAttacking) return;

            // Kiểm tra xem click có phải trên UI không
            if (UIManager.Instance.IsClickOnUI(e.X, e.Y))
            {
                // Click vào UI thì không tấn công
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

            List<TestEnemy> enemiesToAttack = new List<TestEnemy>();
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

                // Only perform attack if not dragging and not clicking on UI
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

            int attackRange = player.AttackRange; // Use player's attack range

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

            List<TestEnemy> enemiesToAttack = new List<TestEnemy>();
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
