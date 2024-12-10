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

        public Form1()
        {
            InitializeComponent();

            // Chuyển PictureBox thành GameObjectt
            obstacles = new List<GameObject> {

                new GameObject(Test1.Location.X, Test1.Location.Y, Test1.Width, Test1.Height, "Obstacle1"),
                new GameObject(Test2.Location.X, Test2.Location.Y, Test2.Width, Test2.Height, "Obstacle2")
            };

            // Truyền danh sách obstacles vào Player
            player = new Player(obstacles);
            player.OnHealthChanged += UpdateHealBar;
            // Tạo enemies và truyền Player vào nếu cần
            enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 1, 500, 100);
            var enemyList = enemies.Cast<Enemy>().ToList();

            // Khởi tạo GameLogic với Player, enemies và obstacles
            gameLogic = new GameLogic(player, enemyList, obstacles);
            gameOver = new GameOver(gameOverTimer, ResetGameAction, Invalidate);

            bg = Image.FromFile("bg.jpg");

            this.DoubleBuffered = true;
            this.KeyPreview = true;


            this.KeyDown += KeyIsDown;
            this.KeyUp += KeyIsUp;
            this.Paint += FormPaintEvent;
            this.MouseClick += FormMouseClick;

            // Bắt đầu Timer
            movementTimer.Tick += TimerEvent;
            movementTimer.Start();
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
                Console.WriteLine($"healBar.Value được cập nhật thành {newHealth}");
            }

            if (newHealth <= 0 && !gameOverState)
            {
                gameOverState = true;
                EndGame(newHealth, healBar);
            }
        }
        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (bg != null)
            {
                canvas.DrawImage(bg, 0, 0, this.Width, this.Height);
            }
            // ahhihih
            string imagePath = @"C:\Users\trant\Source\Repos\Project-Game\Resources\House_4_4.png";
            // ahhihihj
            Image houseImage = Image.FromFile(imagePath);

            // Vẽ Test1 tại vị trí (100, 100)
            Rectangle rectTest1 = new Rectangle(100, 100, 100, 100);
            canvas.DrawImage(houseImage, rectTest1);

            // Vẽ Test2 chồng lên Test1 khoảng 50%
            Rectangle rectTest2 = new Rectangle(100 + 50, 100 + 50, 100, 100);
            canvas.DrawImage(houseImage, rectTest2);



            var playerFrame = player.GetCurrentFrame();
            if (playerFrame != null)
            {
                canvas.DrawImage(playerFrame, player.playerX, player.playerY, player.playerWidth, player.playerHeight);
            }

            foreach (var en in enemies)
            {
                var enemyFrame = en.GetCurrentFrame();
                if (enemyFrame != null)
                {
                    canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);

                    // Kiểm tra xem enemy có phải là TestEnemy hay không để truy cập AttackRange và DetectionRange
                    if (en is TestEnemy testEnemy)
                    {
                        // Vẽ AttackRange (Vòng tròn đỏ)
                        using (Pen penAttack = new Pen(Color.Red, 2))
                        {
                            // Tính toán vị trí để vẽ vòng tròn
                            int centerX = testEnemy.X + testEnemy.Width / 2;
                            int centerY = testEnemy.Y + testEnemy.Height / 2;
                            canvas.DrawEllipse(penAttack, centerX - testEnemy.AttackRange, centerY - testEnemy.AttackRange, testEnemy.AttackRange * 2, testEnemy.AttackRange * 2);
                        }

                        // Vẽ DetectionRange (Vòng tròn xanh)
                        using (Pen penDetect = new Pen(Color.Blue, 2))
                        {
                            // Tính toán vị trí để vẽ vòng tròn
                            int centerX = testEnemy.X + testEnemy.Width / 2;
                            int centerY = testEnemy.Y + testEnemy.Height / 2;
                            canvas.DrawEllipse(penDetect, centerX - testEnemy.DetectionRange, centerY - testEnemy.DetectionRange, testEnemy.DetectionRange * 2, testEnemy.DetectionRange * 2);
                        }
                    }
                }
            }

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
            }

            if (needsRedraw)
            {
                Invalidate();
                needsRedraw = false;
            }
        }


        private void UpdateMap()
        {
            if (player.playerX > 400 && currentMap == 1)
            {
                currentMap = 2;
                // bg = cached image if needed
            }
            else if (player.playerX <= 400 && currentMap == 2)
            {
                currentMap = 1;
                // bg = cached image if needed
            }
        }

        private void ResetGameAction()
        {
            player.ResetPlayer();
            enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 2, 500, 100);
            var enemyList = enemies.Cast<Enemy>().ToList();
            gameLogic.SetEnemies(enemyList);

            gameOverState = false;
            healBar.Value = 100;

            this.KeyPreview = true;
            gameLogic.ResetGameState();
        }

        private void FormMouseClick(object sender, MouseEventArgs e)
        {
            if (gameOverState || player.IsAttacking) return;

            // Xác định hướng hiện tại của người chơi
            string direction = player.CurrentDirection; // Đảm bảo rằng bạn đã thêm thuộc tính này trong Player.cs
            if (string.IsNullOrEmpty(direction))
            {
                direction = "Down"; // Mặc định nếu chưa có hướng
            }

            // Định nghĩa khoảng cách tấn công
            int attackRange = 50; // Bạn có thể điều chỉnh giá trị này theo ý muốn

            // Xác định vùng tấn công dựa trên hướng
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
                    // Mặc định tấn công xuống dưới nếu không xác định được hướng
                    attackArea = new Rectangle(player.playerX, player.playerY + player.playerHeight, player.playerWidth, attackRange);
                    break;
            }

            // Tìm tất cả kẻ địch trong vùng tấn công
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

            // Dù không có kẻ địch trong vùng tấn công, vẫn thực hiện hoạt ảnh tấn công
            if (enemiesToAttack.Count > 0)
            {
                // Chuyển danh sách TestEnemy thành List<Enemy>
                List<Enemy> enemiesToAttackList = enemiesToAttack.Cast<Enemy>().ToList();
                player.PerformAttack(enemiesToAttackList);
            }
            else
            {
                // Không có kẻ địch, nhưng vẫn thực hiện hoạt ảnh tấn công
                player.PerformAttack(new List<Enemy>());
            }

            // Cập nhật lại giao diện
            Invalidate();
        }


        public void EndGame(int currentHealth, ProgressBar healBar)
        {
            Console.WriteLine($"EndGame được gọi với currentHealth = {currentHealth}");
            gameOverState = true;

            // Hiển thị thông báo Game Over
            MessageBox.Show("Game Over! Bạn đã thua cuộc!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Chờ cho đến khi người dùng đóng MessageBox trước khi reset game
            gameOver.ResetGame();

            // Optional: Đặt lại `healBar.Value` nếu cần
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

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}