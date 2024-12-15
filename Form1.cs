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
        private UIManager uiManager;

        private const int PlayerWidth = 50;
        private const int PlayerHeight = 50;
       
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            // Gán sự kiện MouseClick trở lại
            this.MouseClick += FormMouseClick;

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

            this.DoubleBuffered = true;
            this.KeyPreview = true;

            //this.KeyDown += KeyIsDown;
            //this.KeyUp += KeyIsUp;

            inventoryManager = new InventoryManager();
            uiManager = new UIManager(this, inventoryManager);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (!gameOverState)
            {
                if (e.KeyCode == Keys.Left && !player.IsBlockedLeft) player.GoLeft = true;
                if (e.KeyCode == Keys.Right && !player.IsBlockedRight) player.GoRight = true;
                if (e.KeyCode == Keys.Up && !player.IsBlockedUp) player.GoUp = true;
                if (e.KeyCode == Keys.Down && !player.IsBlockedDown) player.GoDown = true;

                if (e.KeyCode == Keys.D1)
                {
                    uiManager.SelectedBarIndex = 0;
                    var item = inventoryManager.bar[0];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }
                if (e.KeyCode == Keys.D2)
                {
                    uiManager.SelectedBarIndex = 1; // Axe is now in slot 1
                    var item = inventoryManager.bar[1];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }
                if (e.KeyCode == Keys.D3)
                {
                    uiManager.SelectedBarIndex = 2;
                    var item = inventoryManager.bar[2];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }
                if (e.KeyCode == Keys.D4)
                {
                    uiManager.SelectedBarIndex = 3;
                    var item = inventoryManager.bar[3];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }
                if (e.KeyCode == Keys.D5)
                {
                    uiManager.SelectedBarIndex = 4;
                    var item = inventoryManager.bar[4];
                    if (item != null) player.SetCurrentWeapon(item.Name);
                }

                uiManager.OnKeyDown(e);
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

            uiManager.Draw(e.Graphics);
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

        // Giữ lại hàm FormMouseClickk
        private void FormMouseClick(object sender, MouseEventArgs e)
        {
            if (gameOverState || player.IsAttacking) return;

            // Kiểm tra xem click có phải trên UI không
            if (uiManager.IsClickOnUI(e.X, e.Y))
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
                    attackArea = new Rectangle(player.playerX - attackRange, player.playerY, attackRange, PlayerHeight);
                    break;
                case "Right":
                    attackArea = new Rectangle(player.playerX + PlayerWidth, player.playerY, attackRange, PlayerHeight);
                    break;
                case "Up":
                    attackArea = new Rectangle(player.playerX, player.playerY - attackRange, PlayerWidth, attackRange);
                    break;
                case "Down":
                    attackArea = new Rectangle(player.playerX, player.playerY + PlayerHeight, PlayerWidth, attackRange);
                    break;
                default:
                    attackArea = new Rectangle(player.playerX, player.playerY + PlayerHeight, PlayerWidth, attackRange);
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
                uiManager.OnMouseDown(e);

                // Chỉ tấn công nếu không drag item, không attacking, không click vào UI
                if (!uiManager.IsDraggingItem && !player.IsAttacking && !uiManager.IsClickOnUI(e.X, e.Y))
                {
                    PerformAttack(e);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            uiManager.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            uiManager.OnMouseUp(e);
        }

        private void PerformAttack(MouseEventArgs e)
        {
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
                    attackArea = new Rectangle(player.playerX - attackRange, player.playerY, attackRange, PlayerHeight);
                    break;
                case "Right":
                    attackArea = new Rectangle(player.playerX + PlayerWidth, player.playerY, attackRange, PlayerHeight);
                    break;
                case "Up":
                    attackArea = new Rectangle(player.playerX, player.playerY - attackRange, PlayerWidth, attackRange);
                    break;
                case "Down":
                    attackArea = new Rectangle(player.playerX, player.playerY + PlayerHeight, PlayerWidth, attackRange);
                    break;
                default:
                    attackArea = new Rectangle(player.playerX, player.playerY + PlayerHeight, PlayerWidth, attackRange);
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
