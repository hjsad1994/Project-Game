using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Project_Game.Entities;

namespace Project_Game
{
    public partial class Form1 : Form
    {
        private Player player;
        private TestEnemy enemy;
        private GameLogic gameLogic;
        private GameOver gameOver;
        private bool gameOverState = false;
        private Image bg;
        private bool needsRedraw = false;
        private int currentMap = 1;

        private List<PictureBox> obstacles;

        public Form1()
        {
            InitializeComponent();

            obstacles = new List<PictureBox> { Test1, Test2 };

            player = new Player(obstacles);
            enemy = new TestEnemy();
            gameLogic = new GameLogic(player, enemy, obstacles);
            gameOver = new GameOver(gameOverTimer, ResetGameAction, Invalidate);

            bg = Image.FromFile("bg.jpg");

            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.KeyDown += KeyIsDown;
            this.KeyUp += KeyIsUp;
            this.MouseClick += FormMouseClick;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (!gameOverState)
            {
                if (e.KeyCode == Keys.Left) player.GoLeft = true;
                if (e.KeyCode == Keys.Right) player.GoRight = true;
                if (e.KeyCode == Keys.Up) player.GoUp = true;
                if (e.KeyCode == Keys.Down) player.GoDown = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (!gameOverState)
            {
                if (e.KeyCode == Keys.Left) player.GoLeft = false;
                if (e.KeyCode == Keys.Right) player.GoRight = false;
                if (e.KeyCode == Keys.Up) player.GoUp = false;
                if (e.KeyCode == Keys.Down) player.GoDown = false;
            }
        }

        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (bg != null)
            {
                canvas.DrawImage(bg, 0, 0, this.Width, this.Height);
            }

            // Vẽ người chơi
            canvas.DrawImage(player.GetCurrentFrame(), player.playerX, player.playerY, player.playerWidth, player.playerHeight);

            // Chỉ vẽ kẻ địch nếu kẻ địch tồn tại và chưa chết
            if (enemy != null && !enemy.IsDead())
            {
                canvas.DrawImage(enemy.GetCurrentFrame(), enemy.enemyX, enemy.enemyY, enemy.enemyWidth, enemy.enemyHeight);
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
                // Nếu đang tấn công, cập nhật hoạt ảnh tấn công
                if (player.IsAttacking)
                {
                    player.UpdateAttack();
                }
                else
                {
                    // Nếu không tấn công, cho phép di chuyển
                    gameLogic.TimerEvent(sender, e, healBar);
                }

                // Kiểm tra nếu kẻ địch cần xóa
                if (enemy != null && enemy.IsDead())
                {
                    Console.WriteLine("Enemy defeated and removed.");
                    enemy = null; // Xóa tham chiếu đến kẻ địch
                }

                UpdateMap();
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
                bg = Image.FromFile("bg2.jpg");
            }
            else if (player.playerX <= 400 && currentMap == 2)
            {
                currentMap = 1;
                bg = Image.FromFile("bg.jpg");
            }
        }

        private void ResetGameAction()
        {
            player.ResetPlayer();
            enemy.SetPosition(500, 100);

            gameOverState = false;
            healBar.Value = 100;

            this.KeyPreview = true;
            gameLogic.ResetGameState();
        }

        private void FormMouseClick(object sender, MouseEventArgs e)
        {
            if (gameOverState || player.IsAttacking) return;

            // Tính khoảng cách giữa chuột và kẻ địch
            double distance = Math.Sqrt(Math.Pow(e.X - enemy.enemyX, 2) + Math.Pow(e.Y - enemy.enemyY, 2));

            const int attackRange = 100; // Phạm vi tấn công

            if (distance <= attackRange && !enemy.IsDead())
            {
                Console.WriteLine("Player attacks!");
                player.PerformAttack(enemy); // Tấn công kẻ địch

                if (enemy.IsDead())
                {
                    Console.WriteLine("Enemy defeated!");
                }
            }
            else
            {
                Console.WriteLine("Target is out of range or already dead!");
            }

            Invalidate(); // Cập nhật giao diện
        }

        public void EndGame(int currentHealth, ProgressBar healBar)
        {
            gameOverState = true;

            MessageBox.Show("Game Over! Bạn đã thua cuộc!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            gameOver.ResetGame();
        }
        private void GameOverTimer_Tick(object sender, EventArgs e)
        {
            // Tắt bộ đếm thời gian khi game over
            gameOverTimer.Stop();

            // Thực hiện hành động reset game
            ResetGameAction();

            // Vẽ lại giao diện
            Invalidate();
        }

        private void Test1_Click(object sender, EventArgs e)
        {

        }
    }
}
