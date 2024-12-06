﻿using System;
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
        private TestEnemy enemy; // Sử dụng TestEnemy thay vì Enemy
        private GameLogic gameLogic;
        private GameOver gameOver;
        private bool gameOverState = false;
        private Image bg;
        private bool needsRedraw = false;
        private int currentMap = 1; // Chỉ số bản đồ hiện tại

        // Danh sách chứa các PictureBox đại diện cho vật cản
        private List<PictureBox> obstacles;

        public Form1()
        {
            InitializeComponent();

            // Tạo các PictureBox vật cản
            obstacles = new List<PictureBox> { Test1, Test2 }; // Bạn có thể thêm nhiều PictureBox ở đây

            player = new Player(obstacles); // Truyền danh sách vật cản vào constructor của Player
            enemy = new TestEnemy(); // Sử dụng TestEnemy thay vì Enemy
            gameLogic = new GameLogic(player, enemy, obstacles); // Truyền TestEnemy vào GameLogic
            gameOver = new GameOver(gameOverTimer, ResetGameAction, Invalidate);
            bg = Image.FromFile("bg.jpg"); // Bản đồ đầu tiên khi bắt đầu
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.KeyDown += KeyIsDown;
            this.KeyUp += KeyIsUp;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (!gameOverState)
            {
                if (e.KeyCode == Keys.Left) player.goLeft = true;
                if (e.KeyCode == Keys.Right) player.goRight = true;
                if (e.KeyCode == Keys.Up) player.goUp = true;
                if (e.KeyCode == Keys.Down) player.goDown = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (!gameOverState)
            {
                if (e.KeyCode == Keys.Left) player.goLeft = false;
                if (e.KeyCode == Keys.Right) player.goRight = false;
                if (e.KeyCode == Keys.Up) player.goUp = false;
                if (e.KeyCode == Keys.Down) player.goDown = false;
            }
        }

        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            // Vẽ nền với kích thước full màn hình
            if (bg != null)
            {
                canvas.DrawImage(bg, 0, 0, this.Width, this.Height);
            }

            // Vẽ hình ảnh của player và enemy
            canvas.DrawImage(player.playerImage, player.playerX, player.playerY, player.playerWidth, player.playerHeight);
            canvas.DrawImage(enemy.enemyImage, enemy.enemyX, enemy.enemyY, enemy.enemyWidth, enemy.enemyHeight);

            // Hiển thị thông báo Game Over nếu trạng thái game over
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
                // Convert obstacles from PictureBox to GameObject
                List<GameObject> gameObjects = obstacles
                    .Select(obstacle => new GameObject(obstacle.Location.X, obstacle.Location.Y, obstacle.Width, obstacle.Height, obstacle.Name))
                    .ToList();

                // Pass the gameObjects list to the Move method
                gameLogic.TimerEvent(sender, e, healBar);
                player.Move();
                enemy.Move(player.playerX, player.playerY, this.Width, this.Height, gameObjects); // Pass obstacles as gameObjects

                // Cập nhật bản đồ khi player di chuyển đến các khu vực khác nhau
                UpdateMap();

                needsRedraw = true;
            }

            if (needsRedraw)
            {
                Invalidate(); // Vẽ lại form
                needsRedraw = false;
            }
        }

        private void UpdateMap()
        {
            // Điều kiện thay đổi bản đồ khi player di chuyển đến vị trí xác định
            if (player.playerX > 400 && currentMap == 1) // Nếu player di chuyển qua x=400, thay đổi bản đồ
            {
                currentMap = 2; // Đổi sang bản đồ 2
                bg = Image.FromFile("bg2.jpg"); // Tải ảnh nền cho bản đồ 2
            }
            else if (player.playerX <= 400 && currentMap == 2) // Nếu player di chuyển trở lại x<=400, quay lại bản đồ 1
            {
                currentMap = 1; // Đổi lại bản đồ 1
                bg = Image.FromFile("bg.jpg"); // Tải ảnh nền cho bản đồ 1
            }
        }

        public void GameOverTimer_Tick(object sender, EventArgs e)
        {
            gameOverTimer.Stop();
            ResetGameAction();
            Invalidate();
        }

        private void ResetGameAction()
        {
            // Reset trạng thái của player
            player.ResetPlayer();

            // Reset trạng thái của enemy
            enemy.SetPosition(500, 100);


            // Đặt lại các cờ di chuyển
            player.goLeft = false;
            player.goRight = false;
            player.goUp = false;
            player.goDown = false;

            // Đặt lại trạng thái game over
            gameOverState = false;

            // Đặt lại thanh máu
            healBar.Value = 100;

            // Đảm bảo player có thể di chuyển trở lại
            this.KeyPreview = true;

            // Đảm bảo va chạm được kiểm tra lại sau khi reset
            gameLogic.ResetGameState();
        }

        public void EndGame(int currentHealth, ProgressBar healBar)
        {
            gameOverState = true;

            // Hiển thị thông báo Game Over
            MessageBox.Show("Game Over! Bạn đã thua cuộc!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Sau khi người chơi bấm OK, gọi phương thức reset game
            gameOver.ResetGame();
        }
    }
}
