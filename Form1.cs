//using Project_Game;
//using Project_Game.Entities;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Windows.Forms;

//namespace Project_Game
//{
//    public partial class Form1 : Form
//    {
//        private Player player;
//        private List<TestEnemy> enemies;
//        private GameLogic gameLogic;
//        private GameOver gameOver;
//        private bool gameOverState = false;
//        private Image bg;
//        private bool needsRedraw = false;
//        private int currentMap = 1;

//        private List<GameObject> obstacles;
//        private List<AnimatedObject> animatedObjects;  // List of AnimatedObjects

//        // Transition zones for map switching
//        // Đảm bảo các khu vực chuyển đổi nằm hoàn toàn trong phạm vi màn hình (800x600)
//        private Rectangle map1ToMap2Zone = new Rectangle(750, 275, 50, 50); // Right edge of Map1 to Map2
//        private Rectangle map2ToMap1Zone = new Rectangle(0, 275, 50, 50);   // Left edge of Map2 to Map1

//        private Rectangle map1ToMap3Zone = new Rectangle(375, 550, 50, 50); // Bottom edge of Map1 to Map3
//        private Rectangle map3ToMap1Zone = new Rectangle(500, 80, 50, 50);  // Top edge of Map3 to Map1 (Đã điều chỉnh)

//        private Rectangle map1ToMap4Zone = new Rectangle(0, 275, 50, 10);    // Left edge of Map1 for Map4 (Chiều cao giảm xuống 10)
//        private Rectangle map4ToMap1Zone = new Rectangle(750, 275, 50, 10);  // Right edge of Map4 to Map1 (Chiều cao giảm xuống 10)

//        // Flag to prevent multiple rapid transitions
//        private bool isTransitioning = false;

//        // Preloaded background images
//        private Image bgMap1;
//        private Image bgMap2;
//        private Image bgMap3;
//        private Image bgMap4;

//        // Player dimensions
//        private const int PlayerWidth = 50;
//        private const int PlayerHeight = 50;

//        public Form1()
//        {
//            InitializeComponent();
//            this.FormClosing += Form1_FormClosing;

//            // Initialize animated objects list
//            animatedObjects = new List<AnimatedObject>();

//            // Initialize obstacles based on existing PictureBox controls (Test1 and Test2)
//            obstacles = new List<GameObject>
//            {
//                new GameObject(Test1.Location.X, Test1.Location.Y, Test1.Width, Test1.Height, "Obstacle1"),
//                new GameObject(Test2.Location.X, Test2.Location.Y, Test2.Width, Test2.Height, "Obstacle2")
//            };

//            // Initialize player with obstacles
//            player = new Player(obstacles);
//            player.OnHealthChanged += UpdateHealBar;

//            // Create enemies
//            enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 0, 500, 100);
//            var enemyList = enemies.Cast<Enemy>().ToList();

//            // Initialize game logic
//            gameLogic = new GameLogic(player, enemyList, obstacles);
//            gameOver = new GameOver(gameOverTimer, ResetGameAction, Invalidate);

//            // Preload backgrounds
//            try
//            {
//                bgMap1 = Image.FromFile("base.png");
//                Console.WriteLine("Loaded base.png successfully.");
//                bgMap2 = Image.FromFile("farm2.png");
//                Console.WriteLine("Loaded farm2.png successfully.");
//                bgMap3 = Image.FromFile("dungeon.png");
//                Console.WriteLine("Loaded dungeon.png successfully.");
//                bgMap4 = Image.FromFile("farm.png");
//                Console.WriteLine("Loaded farm.png successfully.");
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading background images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                Environment.Exit(1);
//            }
//            bg = bgMap1; // Start with Map1
//            Console.WriteLine("Starting with Map1.");

//            // Set form properties
//            this.DoubleBuffered = true;
//            this.KeyPreview = true;

//            // Assign event handlers
//            this.KeyDown += KeyIsDown;
//            this.KeyUp += KeyIsUp;
//            this.Paint += FormPaintEvent;
//            this.MouseClick += FormMouseClick;

//            // Initialize and start the movement timer
//            movementTimer.Tick += TimerEvent;
//            movementTimer.Start();
//        }

//        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            // Exit the entire application when Form1 is closed
//            Application.Exit();
//        }

//        private void KeyIsDown(object sender, KeyEventArgs e)
//        {
//            if (!gameOverState)
//            {
//                if (e.KeyCode == Keys.Left && !player.IsBlockedLeft) player.GoLeft = true;
//                if (e.KeyCode == Keys.Right && !player.IsBlockedRight) player.GoRight = true;
//                if (e.KeyCode == Keys.Up && !player.IsBlockedUp) player.GoUp = true;
//                if (e.KeyCode == Keys.Down && !player.IsBlockedDown) player.GoDown = true;
//            }
//        }

//        private void KeyIsUp(object sender, KeyEventArgs e)
//        {
//            if (!gameOverState)
//            {
//                if (e.KeyCode == Keys.Left)
//                {
//                    player.GoLeft = false;
//                    player.UnblockDirection("Left");
//                }
//                if (e.KeyCode == Keys.Right)
//                {
//                    player.GoRight = false;
//                    player.UnblockDirection("Right");
//                }
//                if (e.KeyCode == Keys.Up)
//                {
//                    player.GoUp = false;
//                    player.UnblockDirection("Up");
//                }
//                if (e.KeyCode == Keys.Down)
//                {
//                    player.GoDown = false;
//                    player.UnblockDirection("Down");
//                }
//            }
//        }

//        private void UpdateHealBar(int newHealth)
//        {
//            if (newHealth >= healBar.Minimum && newHealth <= healBar.Maximum)
//            {
//                healBar.Value = newHealth;
//                Console.WriteLine($"healBar.Value updated to {newHealth}");
//            }

//            if (newHealth <= 0 && !gameOverState)
//            {
//                gameOverState = true;
//                EndGame(newHealth, healBar);
//            }
//        }

//        private void SwitchMap(int newMap, Point targetPosition)
//        {
//            // Update current map
//            currentMap = newMap;
//            Console.WriteLine($"Switching to Map {newMap}.");

//            if (currentMap == 2)
//            {
//                bg = bgMap2;  // Assign preloaded background for Map2
//                Console.WriteLine("Background set to farm2.png (Map2).");
//                animatedObjects = new List<AnimatedObject>
//                {
//                    new AnimatedObject("Flower_Grass_12_Anim_cuts", 150, 120, 30, 30, 10),
//                    new AnimatedObject("Flower_Grass_3_Anim_cuts", 500, 300, 30, 30, 10)
//                };
//                enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 2, 600, 120);  // Add enemies for Map2
//                Console.WriteLine("Enemies for Map2 created.");

//                // Set player position to the target position
//                player.playerX = targetPosition.X;
//                player.playerY = targetPosition.Y;
//                Console.WriteLine($"Player positioned at Map2 entrance: ({player.playerX}, {player.playerY})");
//            }
//            else if (currentMap == 1)
//            {
//                bg = bgMap1;  // Assign preloaded background for Map1
//                Console.WriteLine("Background set to base.png (Map1).");
//                animatedObjects = new List<AnimatedObject>
//                {
//                    new AnimatedObject("Flower_Grass_12_Anim_cuts", 100, 100, 50, 25, 8),
//                    new AnimatedObject("Flower_Grass_3_Anim_cuts", 400, 250, 50, 25, 8)
//                };
//                enemies.Clear();  // Clear enemies for Map1
//                Console.WriteLine("Enemies for Map1 cleared.");

//                // Set player position to the target position
//                player.playerX = targetPosition.X;
//                player.playerY = targetPosition.Y;
//                Console.WriteLine($"Player positioned at Map1 entrance: ({player.playerX}, {player.playerY})");
//            }
//            else if (currentMap == 3)
//            {
//                bg = bgMap3;  // Assign preloaded background for Map3 (Dungeon)
//                Console.WriteLine("Background set to dungeon.png (Map3).");
//                animatedObjects = new List<AnimatedObject>
//                {
//                    new AnimatedObject("Flower_Grass_12_Anim_cuts", 200, 200, 40, 40, 12),
//                    new AnimatedObject("Flower_Grass_3_Anim_cuts", 600, 350, 40, 40, 12)
//                };
//                // enemies = TestEnemy.CreateEnemies("Enemy/Goblin", 3, 700, 150);  // Add enemies for Map3

//                // Set player position to the target position (437, 66)
//                player.playerX = targetPosition.X;
//                player.playerY = targetPosition.Y;
//                Console.WriteLine($"Player positioned at Map3 entrance: ({player.playerX}, {player.playerY})");
//            }
//            else if (currentMap == 4)
//            {
//                bg = bgMap4;  // Assign preloaded background for Map4
//                Console.WriteLine("Background set to farm.png (Map4).");
//                animatedObjects = new List<AnimatedObject>
//                {
//                    new AnimatedObject("Flower_Grass_12_Anim_cuts", 250, 250, 35, 35, 9),
//                    new AnimatedObject("Flower_Grass_3_Anim_cuts", 650, 400, 35, 35, 9)
//                };
//                // enemies = TestEnemy.CreateEnemies("Enemy/Orc", 4, 750, 200);  // Add enemies for Map4
//                Console.WriteLine("Enemies for Map4 (Orc) not created yet.");

//                // Đặt vị trí nhân vật tại (752, 292) để đảm bảo ngoài khu vực chuyển đổi
//                player.playerX = 752;
//                player.playerY = 292;
//                Console.WriteLine($"Player positioned at Map4 entrance: ({player.playerX}, {player.playerY})");
//            }

//            // Redraw the form to reflect changes
//            Invalidate();

//            // Reset the transitioning flag after switching
//            isTransitioning = false;
//        }

//        private void UpdateMap()
//        {
//            if (isTransitioning)
//                return;

//            // Create player's bounding rectangle
//            Rectangle playerRect = new Rectangle(player.playerX, player.playerY, player.playerWidth, player.playerHeight);
//            Console.WriteLine($"Player Rect: {playerRect}");

//            // Check transitions from Map1 to Map2
//            if (currentMap == 1 && map1ToMap2Zone.IntersectsWith(playerRect))
//            {
//                Console.WriteLine("Player entered map1ToMap2Zone.");
//                isTransitioning = true;
//                // Position the player just inside Map2, outside the transition zone
//                Point targetPosition = new Point(map2ToMap1Zone.Right, player.playerY);
//                SwitchMap(2, targetPosition);
//            }
//            // Check transitions from Map2 to Map1
//            else if (currentMap == 2 && map2ToMap1Zone.IntersectsWith(playerRect))
//            {
//                Console.WriteLine("Player entered map2ToMap1Zone.");
//                isTransitioning = true;
//                // Position the player just inside Map1, outside the transition zone
//                Point targetPosition = new Point(map1ToMap2Zone.X - PlayerWidth, player.playerY); // 750 - 50 = 700
//                SwitchMap(1, targetPosition);
//            }
//            // Check transitions from Map1 to Map3 (Dungeon)
//            else if (currentMap == 1 && map1ToMap3Zone.IntersectsWith(playerRect))
//            {
//                Console.WriteLine("Player entered map1ToMap3Zone.");
//                isTransitioning = true;
//                // Position the player just inside Map3, tại (437, 66)
//                Point targetPosition = new Point(437, 66);
//                SwitchMap(3, targetPosition);
//            }
//            // Check transitions from Map3 to Map1 (Dungeon to Base)
//            else if (currentMap == 3 && map3ToMap1Zone.IntersectsWith(playerRect))
//            {
//                Console.WriteLine("Player entered map3ToMap1Zone.");
//                isTransitioning = true;
//                // Đặt vị trí nhân vật vào Map1 tại (100, 100) để xuất hiện gần vị trí vừa vào dungeon
//                Point targetPosition = new Point(100, 100);
//                SwitchMap(1, targetPosition);
//            }
//            // Check transitions from Map1 to Map4
//            else if (currentMap == 1 && map1ToMap4Zone.IntersectsWith(playerRect))
//            {
//                Console.WriteLine("Player entered map1ToMap4Zone.");
//                isTransitioning = true;
//                // Đặt vị trí nhân vật vào Map4 tại (752, 292) để tránh nằm trong khu vực chuyển đổi
//                Point targetPosition = new Point(752, 292);
//                SwitchMap(4, targetPosition);
//            }
//            // Check transitions from Map4 to Map1
//            else if (currentMap == 4 && map4ToMap1Zone.IntersectsWith(playerRect))
//            {
//                Console.WriteLine("Player entered map4ToMap1Zone.");
//                isTransitioning = true;
//                // Đặt vị trí nhân vật vào Map1 tại (50, 292) để xuất hiện gần vị trí vừa vào farm
//                Point targetPosition = new Point(50, 292);
//                SwitchMap(1, targetPosition);
//            }
//        }

//        private void FormPaintEvent(object sender, PaintEventArgs e)
//        {
//            Graphics canvas = e.Graphics;

//            // Draw background
//            if (bg != null)
//            {
//                canvas.DrawImage(bg, 0, 0, 800, 600);
//                Console.WriteLine("Background image drawn.");
//            }
//            else
//            {
//                Console.WriteLine("Background image is null.");
//            }

//            // Draw animated objects
//            foreach (var animatedObject in animatedObjects)
//            {
//                animatedObject.Update();  // Update animation
//                animatedObject.Draw(canvas);  // Draw object
//            }

//            // **Visualize Transition Zones (Development Only)**

//            using (Brush brushMap1ToMap2 = new SolidBrush(Color.FromArgb(100, Color.Red))) // Semi-transparent red
//            {
//                canvas.FillRectangle(brushMap1ToMap2, map1ToMap2Zone);
//            }
//            using (Brush brushMap2ToMap1 = new SolidBrush(Color.FromArgb(100, Color.Blue))) // Semi-transparent blue
//            {
//                canvas.FillRectangle(brushMap2ToMap1, map2ToMap1Zone);
//            }
//            using (Brush brushMap1ToMap3 = new SolidBrush(Color.FromArgb(100, Color.Green))) // Semi-transparent green
//            {
//                canvas.FillRectangle(brushMap1ToMap3, map1ToMap3Zone);
//            }
//            using (Brush brushMap3ToMap1 = new SolidBrush(Color.FromArgb(100, Color.Yellow))) // Semi-transparent yellow
//            {
//                canvas.FillRectangle(brushMap3ToMap1, map3ToMap1Zone);
//            }
//            using (Brush brushMap1ToMap4 = new SolidBrush(Color.FromArgb(100, Color.Purple))) // Semi-transparent purple
//            {
//                canvas.FillRectangle(brushMap1ToMap4, map1ToMap4Zone);
//            }
//            using (Brush brushMap4ToMap1 = new SolidBrush(Color.FromArgb(100, Color.Orange))) // Semi-transparent orange
//            {
//                canvas.FillRectangle(brushMap4ToMap1, map4ToMap1Zone);
//            }


//            // Draw player
//            var playerFrame = player.GetCurrentFrame();
//            if (playerFrame != null)
//            {
//                canvas.DrawImage(playerFrame, player.playerX, player.playerY, player.playerWidth, player.playerHeight);
//            }

//            // Draw enemies
//            foreach (var en in enemies)
//            {
//                var enemyFrame = en.GetCurrentFrame();
//                if (enemyFrame != null)
//                {
//                    canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);

//                    // If enemy is TestEnemy, draw AttackRange and DetectionRange
//                    if (en is TestEnemy testEnemy)
//                    {
//                        // Draw AttackRange (Red circle)
//                        using (Pen penAttack = new Pen(Color.Red, 2))
//                        {
//                            int centerX = testEnemy.X + testEnemy.Width / 2;
//                            int centerY = testEnemy.Y + testEnemy.Height / 2;
//                            canvas.DrawEllipse(penAttack, centerX - testEnemy.AttackRange, centerY - testEnemy.AttackRange, testEnemy.AttackRange * 2, testEnemy.AttackRange * 2);
//                        }

//                        // Draw DetectionRange (Blue circle)
//                        using (Pen penDetect = new Pen(Color.Blue, 2))
//                        {
//                            int centerX = testEnemy.X + testEnemy.Width / 2;
//                            int centerY = testEnemy.Y + testEnemy.Height / 2;
//                            canvas.DrawEllipse(penDetect, centerX - testEnemy.DetectionRange, centerY - testEnemy.DetectionRange, testEnemy.DetectionRange * 2, testEnemy.DetectionRange * 2);
//                        }
//                    }
//                }
//            }

//            // Check for game over state
//            if (gameOverState)
//            {
//                using (Font font = new Font("Arial", 24, FontStyle.Bold))
//                {
//                    canvas.DrawString("Game Over", font, Brushes.Red, new PointF(300, 250));
//                }
//            }
//        }

//        private void TimerEvent(object sender, EventArgs e)
//        {
//            if (!gameOverState)
//            {
//                if (player.IsAttacking)
//                {
//                    player.UpdateAttack();
//                }
//                else
//                {
//                    player.Move();
//                }

//                gameLogic.TimerEvent(sender, e, healBar);

//                enemies.RemoveAll(en => en.ShouldRemove);

//                needsRedraw = true;

//                // Update map position
//                UpdateMap();  // Check if map switch is needed
//            }

//            if (needsRedraw)
//            {
//                Invalidate();
//                needsRedraw = false;
//            }
//        }

//        private void ResetGameAction()
//        {
//            player.ResetPlayer();
//            enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 2, 500, 100);
//            var enemyList = enemies.Cast<Enemy>().ToList();
//            gameLogic.SetEnemies(enemyList);

//            gameOverState = false;
//            healBar.Value = 100;

//            this.KeyPreview = true;
//            gameLogic.ResetGameState();

//            // Reset background and other map-specific settings if necessary
//            if (currentMap == 2)
//            {
//                bg = bgMap2;
//                Console.WriteLine("Background reset to farm2.png (Map2).");
//            }
//            else if (currentMap == 3)
//            {
//                bg = bgMap3;
//                Console.WriteLine("Background reset to dungeon.png (Map3).");
//            }
//            else if (currentMap == 4)
//            {
//                bg = bgMap4;
//                Console.WriteLine("Background reset to farm.png (Map4).");
//            }
//            else
//            {
//                bg = bgMap1;
//                Console.WriteLine("Background reset to base.png (Map1).");
//            }

//            // Redraw the form
//            Invalidate();
//        }

//        private void FormMouseClick(object sender, MouseEventArgs e)
//        {
//            if (gameOverState || player.IsAttacking) return;

//            // Determine the current direction of the player
//            string direction = player.CurrentDirection; // Ensure you have this property in Player.cs
//            if (string.IsNullOrEmpty(direction))
//            {
//                direction = "Down"; // Default direction if not set
//            }

//            // Define attack range
//            int attackRange = 50; // Adjust as needed

//            // Determine attack area based on direction
//            Rectangle attackArea = new Rectangle();
//            switch (direction)
//            {
//                case "Left":
//                    attackArea = new Rectangle(player.playerX - attackRange, player.playerY, attackRange, PlayerHeight);
//                    break;
//                case "Right":
//                    attackArea = new Rectangle(player.playerX + PlayerWidth, player.playerY, attackRange, PlayerHeight);
//                    break;
//                case "Up":
//                    attackArea = new Rectangle(player.playerX, player.playerY - attackRange, PlayerWidth, attackRange);
//                    break;
//                case "Down":
//                    attackArea = new Rectangle(player.playerX, player.playerY + PlayerHeight, PlayerWidth, attackRange);
//                    break;
//                default:
//                    // Default attack direction if not determined
//                    attackArea = new Rectangle(player.playerX, player.playerY + PlayerHeight, PlayerWidth, attackRange);
//                    break;
//            }

//            // Find all enemies within the attack area
//            List<TestEnemy> enemiesToAttack = new List<TestEnemy>();
//            foreach (var en in enemies)
//            {
//                if (!en.IsDead())
//                {
//                    Rectangle enemyRect = new Rectangle(en.X, en.Y, en.Width, en.Height);
//                    if (attackArea.IntersectsWith(enemyRect))
//                    {
//                        enemiesToAttack.Add(en);
//                    }
//                }
//            }

//            // Perform attack animation regardless of whether enemies are hit
//            if (enemiesToAttack.Count > 0)
//            {
//                // Convert List<TestEnemy> to List<Enemy>
//                List<Enemy> enemiesToAttackList = enemiesToAttack.Cast<Enemy>().ToList();
//                player.PerformAttack(enemiesToAttackList);
//                Console.WriteLine($"Player attacked {enemiesToAttackList.Count} enemies.");
//            }
//            else
//            {
//                // No enemies hit, still perform attack animation
//                player.PerformAttack(new List<Enemy>());
//                Console.WriteLine("Player performed attack, but no enemies were hit.");
//            }

//            // Redraw the form to reflect attack
//            Invalidate();
//        }

//        public void EndGame(int currentHealth, ProgressBar healBar)
//        {
//            Console.WriteLine($"EndGame called with currentHealth = {currentHealth}");
//            gameOverState = true;

//            // Display Game Over message
//            MessageBox.Show("Game Over! You have been defeated!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

//            // Wait for the user to close the MessageBox before resetting the game
//            gameOver.ResetGame();

//            // Optional: Reset healBar.Value if needed
//            healBar.Value = player.MaxHealth;
//        }

//        private void GameOverTimer_Tick(object sender, EventArgs e)
//        {
//            gameOverTimer.Stop();
//            ResetGameAction();
//            Invalidate();
//        }

//        private void Test1_Click(object sender, EventArgs e)
//        {
//            // You can handle Test1 PictureBox click events here if needed
//        }

//        private void Form1_Load(object sender, EventArgs e)
//        {
//            // You can initialize additional settings here if needed
//        }
//    }
//}
using Project_Game;
using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Project_Game
{
    public partial class Form1 : Form
    {
        private Player player;
        private List<TestEnemy> enemies;
        private GameLogic gameLogic;
        private GameOver gameOver;
        private bool gameOverState = false;
        private Image bg;
        private bool needsRedraw = false;
        private int currentMap = 1;

        private List<GameObject> obstacles;
        private List<AnimatedObject> animatedObjects;  // List of AnimatedObjects

        // Transition zones for map switching
        // Đảm bảo các khu vực chuyển đổi nằm hoàn toàn trong phạm vi màn hình (800x600)
        private Rectangle map1ToMap2Zone = new Rectangle(750, 275, 50, 50); // Right edge of Map1 to Map2
        private Rectangle map2ToMap1Zone = new Rectangle(0, 275, 50, 50);   // Left edge of Map2 to Map1

        private Rectangle map1ToMap3Zone = new Rectangle(375, 550, 50, 50); // Bottom edge of Map1 to Map3
        private Rectangle map3ToMap1Zone = new Rectangle(500, 80, 50, 50);  // Top edge of Map3 to Map1 (Đã điều chỉnh)

        private Rectangle map1ToMap4Zone = new Rectangle(0, 275, 50, 10);    // Left edge of Map1 for Map4 (Chiều cao giảm xuống 10)
        private Rectangle map4ToMap1Zone = new Rectangle(750, 275, 50, 10);  // Right edge of Map4 to Map1 (Chiều cao giảm xuống 10)

        // Flag to prevent multiple rapid transitions
        private bool isTransitioning = false;

        // Preloaded background images
        private Image bgMap1;
        private Image bgMap2;
        private Image bgMap3;
        private Image bgMap4;

        // Player dimensions
        private const int PlayerWidth = 50;
        private const int PlayerHeight = 50;

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            // Initialize animated objects list
            animatedObjects = new List<AnimatedObject>();

            // Initialize obstacles based on existing PictureBox controls (Test1 and Test2)
            obstacles = new List<GameObject>
            {
                new GameObject(Test1.Location.X, Test1.Location.Y, Test1.Width, Test1.Height, "Obstacle1"),
                new GameObject(Test2.Location.X, Test2.Location.Y, Test2.Width, Test2.Height, "Obstacle2")
            };

            // Initialize player with obstacles
            player = new Player(obstacles);
            player.OnHealthChanged += UpdateHealBar;

            // Create enemies
            enemies = new List<TestEnemy>(); // Danh sách quái ban đầu trống
            var enemyList = enemies.Cast<Enemy>().ToList();

            // Initialize game logic
            gameLogic = new GameLogic(player, enemyList, obstacles);
            gameOver = new GameOver(gameOverTimer, ResetGameAction, Invalidate);

            // Preload backgrounds
            try
            {
                bgMap1 = Image.FromFile("base.png");
                Console.WriteLine("Loaded base.png successfully.");
                bgMap2 = Image.FromFile("farm2.png");
                Console.WriteLine("Loaded farm2.png successfully.");
                bgMap3 = Image.FromFile("dungeon.png");
                Console.WriteLine("Loaded dungeon.png successfully.");
                bgMap4 = Image.FromFile("farm.png");
                Console.WriteLine("Loaded farm.png successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading background images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            bg = bgMap1; // Start with Map1
            Console.WriteLine("Starting with Map1.");

            // Set form properties
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            // Assign event handlers
            this.KeyDown += KeyIsDown;
            this.KeyUp += KeyIsUp;
            this.Paint += FormPaintEvent;
            this.MouseClick += FormMouseClick;

            // Initialize and start the movement timer
            movementTimer.Tick += TimerEvent;
            movementTimer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Exit the entire application when Form1 is closed
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

        private void SwitchMap(int newMap, Point targetPosition)
        {
            // Update current map
            currentMap = newMap;
            Console.WriteLine($"Switching to Map {newMap}.");

            if (currentMap == 2)
            {
                bg = bgMap2;  // Assign preloaded background for Map2
                Console.WriteLine("Background set to farm2.png (Map2).");
                animatedObjects = new List<AnimatedObject>
                {
                    new AnimatedObject("Flower_Grass_12_Anim_cuts", 150, 120, 30, 30, 10),
                    new AnimatedObject("Flower_Grass_3_Anim_cuts", 500, 300, 30, 30, 10)
                };
                enemies.Clear();  // Clear enemies for Map2
                Console.WriteLine("Enemies for Map2 cleared.");

                // Set player position to the target position
                player.playerX = targetPosition.X;
                player.playerY = targetPosition.Y;
                Console.WriteLine($"Player positioned at Map2 entrance: ({player.playerX}, {player.playerY})");
            }
            else if (currentMap == 1)
            {
                bg = bgMap1;  // Assign preloaded background for Map1
                Console.WriteLine("Background set to base.png (Map1).");
                animatedObjects = new List<AnimatedObject>
                {
                    new AnimatedObject("Flower_Grass_12_Anim_cuts", 100, 100, 50, 25, 8),
                    new AnimatedObject("Flower_Grass_3_Anim_cuts", 400, 250, 50, 25, 8)
                };
                enemies.Clear();  // Clear enemies for Map1
                Console.WriteLine("Enemies for Map1 cleared.");

                // Set player position to the target position
                player.playerX = targetPosition.X;
                player.playerY = targetPosition.Y;
                Console.WriteLine($"Player positioned at Map1 entrance: ({player.playerX}, {player.playerY})");
            }
            else if (currentMap == 3)
            {
                bg = bgMap3;  // Assign preloaded background for Map3 (Dungeon)
                Console.WriteLine("Background set to dungeon.png (Map3).");
                animatedObjects = new List<AnimatedObject>
                {
                    new AnimatedObject("Flower_Grass_12_Anim_cuts", 200, 200, 40, 40, 12),
                    new AnimatedObject("Flower_Grass_3_Anim_cuts", 600, 350, 40, 40, 12)
                };
                enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 3, 700, 150);  // Add enemies for Map3
                Console.WriteLine("Enemies for Map3 (Goblin) created.");

                // Set player position to the target position (437, 66)
                player.playerX = targetPosition.X;
                player.playerY = targetPosition.Y;
                Console.WriteLine($"Player positioned at Map3 entrance: ({player.playerX}, {player.playerY})");
            }
            else if (currentMap == 4)
            {
                bg = bgMap4;  // Assign preloaded background for Map4
                Console.WriteLine("Background set to farm.png (Map4).");
                animatedObjects = new List<AnimatedObject>
                {
                    new AnimatedObject("Flower_Grass_12_Anim_cuts", 250, 250, 35, 35, 9),
                    new AnimatedObject("Flower_Grass_3_Anim_cuts", 650, 400, 35, 35, 9)
                };
                enemies.Clear();  // Clear enemies for Map4
                Console.WriteLine("Enemies for Map4 cleared.");

                // Set player position to the target position
                player.playerX = 752;
                player.playerY = 292;
                Console.WriteLine($"Player positioned at Map4 entrance: ({player.playerX}, {player.playerY})");
            }

            // Redraw the form to reflect changes
            Invalidate();

            // Reset the transitioning flag after switching
            isTransitioning = false;

            // Update GameLogic with the new enemies list
            gameLogic.SetEnemies(enemies.Cast<Enemy>().ToList());
        }

        private void UpdateMap()
        {
            if (isTransitioning)
                return;

            // Create player's bounding rectangle
            Rectangle playerRect = new Rectangle(player.playerX, player.playerY, player.playerWidth, player.playerHeight);
            Console.WriteLine($"Player Rect: {playerRect}");

            // Check transitions from Map1 to Map2
            if (currentMap == 1 && map1ToMap2Zone.IntersectsWith(playerRect))
            {
                Console.WriteLine("Player entered map1ToMap2Zone.");
                isTransitioning = true;
                // Position the player just inside Map2, outside the transition zone
                Point targetPosition = new Point(map2ToMap1Zone.Right, player.playerY);
                SwitchMap(2, targetPosition);
            }
            // Check transitions from Map2 to Map1
            else if (currentMap == 2 && map2ToMap1Zone.IntersectsWith(playerRect))
            {
                Console.WriteLine("Player entered map2ToMap1Zone.");
                isTransitioning = true;
                // Position the player just inside Map1, outside the transition zone
                Point targetPosition = new Point(map1ToMap2Zone.X - PlayerWidth, player.playerY); // 750 - 50 = 700
                SwitchMap(1, targetPosition);
            }
            // Check transitions from Map1 to Map3 (Dungeon)
            else if (currentMap == 1 && map1ToMap3Zone.IntersectsWith(playerRect))
            {
                Console.WriteLine("Player entered map1ToMap3Zone.");
                isTransitioning = true;
                // Position the player just inside Map3, tại (437, 66)
                Point targetPosition = new Point(437, 66);
                SwitchMap(3, targetPosition);
            }
            // Check transitions from Map3 to Map1 (Dungeon to Base)
            else if (currentMap == 3 && map3ToMap1Zone.IntersectsWith(playerRect))
            {
                Console.WriteLine("Player entered map3ToMap1Zone.");
                isTransitioning = true;
                // Đặt vị trí nhân vật vào Map1 tại (100, 100) để xuất hiện gần vị trí vừa vào dungeon
                Point targetPosition = new Point(100, 100);
                SwitchMap(1, targetPosition);
            }
            // Check transitions from Map1 to Map4
            else if (currentMap == 1 && map1ToMap4Zone.IntersectsWith(playerRect))
            {
                Console.WriteLine("Player entered map1ToMap4Zone.");
                isTransitioning = true;
                // Đặt vị trí nhân vật vào Map4 tại (752, 292) để tránh nằm trong khu vực chuyển đổi
                Point targetPosition = new Point(752, 292);
                SwitchMap(4, targetPosition);
            }
            // Check transitions from Map4 to Map1
            else if (currentMap == 4 && map4ToMap1Zone.IntersectsWith(playerRect))
            {
                Console.WriteLine("Player entered map4ToMap1Zone.");
                isTransitioning = true;
                // Đặt vị trí nhân vật vào Map1 tại (50, 292) để xuất hiện gần vị trí vừa vào farm
                Point targetPosition = new Point(50, 292);
                SwitchMap(1, targetPosition);
            }
        }

        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            // Draw background
            if (bg != null)
            {
                canvas.DrawImage(bg, 0, 0, 800, 600);
                Console.WriteLine("Background image drawn.");
            }
            else
            {
                Console.WriteLine("Background image is null.");
            }

            // Draw animated objects
            foreach (var animatedObject in animatedObjects)
            {
                animatedObject.Update();  // Update animation
                animatedObject.Draw(canvas);  // Draw object
            }

            // **Visualize Transition Zones (Development Only)**
            using (Brush brushMap1ToMap2 = new SolidBrush(Color.FromArgb(100, Color.Red))) // Semi-transparent red
            {
                canvas.FillRectangle(brushMap1ToMap2, map1ToMap2Zone);
            }
            using (Brush brushMap2ToMap1 = new SolidBrush(Color.FromArgb(100, Color.Blue))) // Semi-transparent blue
            {
                canvas.FillRectangle(brushMap2ToMap1, map2ToMap1Zone);
            }
            using (Brush brushMap1ToMap3 = new SolidBrush(Color.FromArgb(100, Color.Green))) // Semi-transparent green
            {
                canvas.FillRectangle(brushMap1ToMap3, map1ToMap3Zone);
            }
            using (Brush brushMap3ToMap1 = new SolidBrush(Color.FromArgb(100, Color.Yellow))) // Semi-transparent yellow
            {
                canvas.FillRectangle(brushMap3ToMap1, map3ToMap1Zone);
            }
            using (Brush brushMap1ToMap4 = new SolidBrush(Color.FromArgb(100, Color.Purple))) // Semi-transparent purple
            {
                canvas.FillRectangle(brushMap1ToMap4, map1ToMap4Zone);
            }
            using (Brush brushMap4ToMap1 = new SolidBrush(Color.FromArgb(100, Color.Orange))) // Semi-transparent orange
            {
                canvas.FillRectangle(brushMap4ToMap1, map4ToMap1Zone);
            }

            // Draw player
            var playerFrame = player.GetCurrentFrame();
            if (playerFrame != null)
            {
                canvas.DrawImage(playerFrame, player.playerX, player.playerY, player.playerWidth, player.playerHeight);
            }

            // Draw enemies
            foreach (var en in enemies)
            {
                var enemyFrame = en.GetCurrentFrame();
                if (enemyFrame != null)
                {
                    canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);

                    // If enemy is TestEnemy, draw AttackRange and DetectionRange
                    if (en is TestEnemy testEnemy)
                    {
                        // Draw AttackRange (Red circle)
                        using (Pen penAttack = new Pen(Color.Red, 2))
                        {
                            int centerX = testEnemy.X + testEnemy.Width / 2;
                            int centerY = testEnemy.Y + testEnemy.Height / 2;
                            canvas.DrawEllipse(penAttack, centerX - testEnemy.AttackRange, centerY - testEnemy.AttackRange, testEnemy.AttackRange * 2, testEnemy.AttackRange * 2);
                        }

                        // Draw DetectionRange (Blue circle)
                        using (Pen penDetect = new Pen(Color.Blue, 2))
                        {
                            int centerX = testEnemy.X + testEnemy.Width / 2;
                            int centerY = testEnemy.Y + testEnemy.Height / 2;
                            canvas.DrawEllipse(penDetect, centerX - testEnemy.DetectionRange, centerY - testEnemy.DetectionRange, testEnemy.DetectionRange * 2, testEnemy.DetectionRange * 2);
                        }
                    }
                }
            }

            // Check for game over state
            if (gameOverState)
            {
                using (Font font = new Font("Arial", 24, FontStyle.Bold))
                {
                    canvas.DrawString("Game Over", font, Brushes.Red, new PointF(300, 250));
                }
            }
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

                enemies.RemoveAll(en => en.ShouldRemove);

                needsRedraw = true;

                // Update map position
                UpdateMap();  // Check if map switch is needed
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
            enemies = new List<TestEnemy>(); // Reset enemies to trống
            gameLogic.SetEnemies(enemies.Cast<Enemy>().ToList());

            gameOverState = false;
            healBar.Value = 100;

            this.KeyPreview = true;
            gameLogic.ResetGameState();

            // Reset background and other map-specific settings if necessary
            if (currentMap == 2)
            {
                bg = bgMap2;
                Console.WriteLine("Background reset to farm2.png (Map2).");
            }
            else if (currentMap == 3)
            {
                bg = bgMap3;
                Console.WriteLine("Background reset to dungeon.png (Map3).");
            }
            else if (currentMap == 4)
            {
                bg = bgMap4;
                Console.WriteLine("Background reset to farm.png (Map4).");
            }
            else
            {
                bg = bgMap1;
                Console.WriteLine("Background reset to base.png (Map1).");
            }

            // Redraw the form
            Invalidate();
        }

        private void FormMouseClick(object sender, MouseEventArgs e)
        {
            if (gameOverState || player.IsAttacking) return;

            // Determine the current direction of the player
            string direction = player.CurrentDirection; // Ensure you have this property in Player.cs
            if (string.IsNullOrEmpty(direction))
            {
                direction = "Down"; // Default direction if not set
            }

            // Define attack range
            int attackRange = 50; // Adjust as needed

            // Determine attack area based on direction
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
                    // Default attack direction if not determined
                    attackArea = new Rectangle(player.playerX, player.playerY + PlayerHeight, PlayerWidth, attackRange);
                    break;
            }

            // Find all enemies within the attack area
            List<TestEnemy> enemiesToAttack = new List<TestEnemy>();
            foreach (var en in enemies)
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

            // Perform attack animation regardless of whether enemies are hit
            if (enemiesToAttack.Count > 0)
            {
                // Convert List<TestEnemy> to List<Enemy>
                List<Enemy> enemiesToAttackList = enemiesToAttack.Cast<Enemy>().ToList();
                player.PerformAttack(enemiesToAttackList);
                Console.WriteLine($"Player attacked {enemiesToAttackList.Count} enemies.");
            }
            else
            {
                // No enemies hit, still perform attack animation
                player.PerformAttack(new List<Enemy>());
                Console.WriteLine("Player performed attack, but no enemies were hit.");
            }

            // Redraw the form to reflect attack
            Invalidate();
        }

        public void EndGame(int currentHealth, ProgressBar healBar)
        {
            Console.WriteLine($"EndGame called with currentHealth = {currentHealth}");
            gameOverState = true;

            // Display Game Over message
            MessageBox.Show("Game Over! You have been defeated!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Wait for the user to close the MessageBox before resetting the game
            gameOver.ResetGame();

            // Optional: Reset healBar.Value if needed
            healBar.Value = player.MaxHealth;
        }

        private void GameOverTimer_Tick(object sender, EventArgs e)
        {
            gameOverTimer.Stop();
            ResetGameAction();
            Invalidate();
        }

        private void Test1_Click(object sender, EventArgs e)
        {
            // You can handle Test1 PictureBox click events here if needed
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // You can initialize additional settings here if needed
        }
    }
}
