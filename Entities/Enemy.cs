using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Project_Game.Entities
{
    public class Enemy
    {
        public Image enemyImage;
        public List<string> enemyMovementsLeft = new List<string>();
        public List<string> enemyMovementsRight = new List<string>();
        public List<string> enemyMovementsUp = new List<string>();
        public List<string> enemyMovementsDown = new List<string>();
        public string enemyDirection = "Down"; // Default direction
        public int enemyX = 500;
        public int enemyY = 100;
        public int enemyHeight = 50;
        public int enemyWidth = 32;
        public int enemySpeed = 4;
        public int enemySteps = 0;
        public int enemyFrameRate = 0;  // Thêm frameRate để làm chậm animation
        private int enemySpeedX = 2;
        private int enemySpeedY = 2;

        public Enemy()
        {
            LoadEnemyImages();
        }

        public void LoadEnemyImages()
        {
            LoadEnemyDirectionImages("Left", enemyMovementsLeft);
            LoadEnemyDirectionImages("Right", enemyMovementsRight);
            LoadEnemyDirectionImages("Up", enemyMovementsUp);
            LoadEnemyDirectionImages("Down", enemyMovementsDown);

            // Mặc định chọn ảnh cho hướng "Down" khi bắt đầu
            if (enemyMovementsDown.Count > 0)
            {
                enemyImage = Image.FromFile(enemyMovementsDown[0]);
            }
        }

        private void LoadEnemyDirectionImages(string direction, List<string> enemyMovementList)
        {
            string enemyFolder = $"Char_MoveMent/{direction}";  // Lấy thư mục theo từng hướng di chuyển
            var directionImages = Directory.GetFiles(enemyFolder, "*.png").ToList();
            enemyMovementList.AddRange(directionImages);  // Thêm các ảnh vào danh sách của từng hướng
        }

        // Phương thức để chọn ảnh cho Enemy dựa trên hướng di chuyển
        public void AnimateEnemy(int start, int end)
        {
            enemyFrameRate++;  // Tăng tốc độ frame rate để điều khiển tốc độ animation
            if (enemyFrameRate >= 5)  // Điều chỉnh số frame (khoảng cách thay đổi hình ảnh)
            {
                enemySteps++;
                if (enemySteps > 5)  // Giới hạn số lượng frame từ 0 đến 6
                {
                    enemySteps = 0;
                }
                enemyFrameRate = 0;
            }

            string enemyImagePath = string.Empty;

            // Chọn hình ảnh cho enemy dựa trên hướng di chuyển
            switch (enemyDirection)
            {
                case "Left":
                    enemyImagePath = enemyMovementsLeft[enemySteps];  // Chọn khung hình theo chỉ số từ 0 đến 6
                    break;
                case "Right":
                    enemyImagePath = enemyMovementsRight[enemySteps];  // Chọn khung hình theo chỉ số từ 0 đến 6
                    break;
                case "Up":
                    enemyImagePath = enemyMovementsUp[enemySteps];  // Chọn khung hình theo chỉ số từ 0 đến 6
                    break;
                case "Down":
                    enemyImagePath = enemyMovementsDown[enemySteps];  // Chọn khung hình theo chỉ số từ 0 đến 6
                    break;
            }

            if (!string.IsNullOrEmpty(enemyImagePath))
            {
                using (Image img = Image.FromFile(enemyImagePath))
                {
                    enemyImage = new Bitmap(img);  // Tạo bản sao của hình ảnh
                }
            }
        }

        // Phương thức di chuyển của enemy (gọi MoveTowardsPlayer trong TimerEvent)
        public void Move(int playerX, int playerY, int screenWidth, int screenHeight)
        {
            // Tính toán khoảng cách giữa enemy và player
            int deltaX = playerX - enemyX;
            int deltaY = playerY - enemyY;

            // Di chuyển enemy về phía player theo từng trục
            if (Math.Abs(deltaX) > enemySpeedX)
            {
                enemyX += Math.Sign(deltaX) * enemySpeedX;
                LoadMovementImages(deltaX < 0 ? "Left" : "Right");  // Chọn hướng di chuyển (trái hoặc phải)
            }

            if (Math.Abs(deltaY) > enemySpeedY)
            {
                enemyY += Math.Sign(deltaY) * enemySpeedY;
                LoadMovementImages(deltaY < 0 ? "Up" : "Down");  // Chọn hướng di chuyển (lên hoặc xuống)
            }

            // Cập nhật animation của enemy dựa trên hướng di chuyển
            AnimateEnemy(0, 6);  // Giới hạn frame từ 0 đến 6
        }

        // Phương thức để cập nhật hướng di chuyển của enemy
        public void LoadMovementImages(string direction)
        {
            switch (direction)
            {
                case "Left":
                    enemyDirection = "Left";
                    break;
                case "Right":
                    enemyDirection = "Right";
                    break;
                case "Up":
                    enemyDirection = "Up";
                    break;
                case "Down":
                    enemyDirection = "Down";
                    break;
            }
        }
    }
}
