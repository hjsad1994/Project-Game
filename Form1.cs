using Project_Game;
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

            // Chuyển PictureBox thành GameObject
            obstacles = new List<GameObject> {
                new GameObject(Test1.Location.X, Test1.Location.Y, Test1.Width, Test1.Height, "Obstacle1"),
                new GameObject(Test2.Location.X, Test2.Location.Y, Test2.Width, Test2.Height, "Obstacle2")
            };

            player = new Player(new List<PictureBox>());

            // Tạo ít quái hơn để kiểm tra
            enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 2, 500, 100);
            var enemyList = enemies.Cast<Enemy>().ToList();

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

        //private void FormPaintEvent(object sender, PaintEventArgs e)
        //{
        //    Graphics canvas = e.Graphics;

        //    if (bg != null)
        //    {
        //        canvas.DrawImage(bg, 0, 0, this.Width, this.Height);
        //    }

        //    var playerFrame = player.GetCurrentFrame();
        //    if (playerFrame != null)
        //    {
        //        canvas.DrawImage(playerFrame, player.playerX, player.playerY, player.playerWidth, player.playerHeight);
        //    }

        //    foreach (var en in enemies)
        //    {
        //        var enemyFrame = en.GetCurrentFrame();
        //        if (enemyFrame != null)
        //        {
        //            canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);
        //        }
        //    }

        //    if (gameOverState)
        //    {
        //        using (Font font = new Font("Arial", 24, FontStyle.Bold))
        //        {
        //            canvas.DrawString("Game Over", font, Brushes.Red, new PointF(300, 250));
        //        }
        //    }
        //}
        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (bg != null)
            {
                canvas.DrawImage(bg, 0, 0, this.Width, this.Height);
            }

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
                // bg = cached image if neededd
            }
        }

        private void ResetGameAction()
        {
            player.ResetPlayer();
            enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 2, 500, 100);

            gameOverState = false;
            healBar.Value = 100;

            this.KeyPreview = true;
            gameLogic.ResetGameState();

            var enemyList = enemies.Cast<Enemy>().ToList();
            gameLogic.SetEnemies(enemyList);
        }

        private void FormMouseClick(object sender, MouseEventArgs e)
        {
            if (gameOverState || player.IsAttacking) return;

            const int attackRange = 100;
            double attackRangeSquared = attackRange * attackRange;
            TestEnemy targetEnemy = null;
            double minDistance = double.MaxValue;

            foreach (var en in enemies)
            {
                double dx = e.X - en.X;
                double dy = e.Y - en.Y;
                double distSquared = dx * dx + dy * dy;
                if (distSquared <= attackRangeSquared && distSquared < minDistance && !en.IsDead())
                {
                    minDistance = distSquared;
                    targetEnemy = en;
                }
            }

            if (targetEnemy != null)
            {
                player.PerformAttack(targetEnemy);
            }

            Invalidate();
        }

        public void EndGame(int currentHealth, ProgressBar healBar)
        {
            gameOverState = true;

            MessageBox.Show("Game Over! Bạn đã thua cuộc!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            gameOver.ResetGame();
        }

        private void GameOverTimer_Tick(object sender, EventArgs e)
        {
            gameOverTimer.Stop();
            ResetGameAction();
            Invalidate();
        }
    }
}
