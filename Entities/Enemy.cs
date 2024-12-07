//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;

//namespace Project_Game.Entities
//{
//    public class Enemy
//    {
//        public Image enemyImage;
//        public List<string> enemyMovementsLeft = new List<string>();
//        public List<string> enemyMovementsRight = new List<string>();
//        public List<string> enemyMovementsUp = new List<string>();
//        public List<string> enemyMovementsDown = new List<string>();
//        public string enemyDirection = "Down"; // Default direction
//        public int enemyX = 500;
//        public int enemyY = 100;
//        public int enemyHeight = 50;
//        public int enemyWidth = 32;
//        public int enemySpeed = 4;
//        public int enemySteps = 0;
//        public int enemyFrameRate = 0;  // Thêm frameRate để làm chậm animation
//        private int enemySpeedX = 2;
//        private int enemySpeedY = 2;
//        // taaaaasdsadsassaa
//        // aa
//        public Enemy()
//        {
//            LoadEnemyImages();
//        }

//        public void LoadEnemyImages()
//        {
//            LoadEnemyDirectionImages("Left", enemyMovementsLeft);
//            LoadEnemyDirectionImages("Right", enemyMovementsRight);
//            LoadEnemyDirectionImages("Up", enemyMovementsUp);
//            LoadEnemyDirectionImages("Down", enemyMovementsDown);

//            // Mặc định chọn ảnh cho hướng "Down" khi bắt đầu
//            if (enemyMovementsDown.Count > 0)
//            {
//                enemyImage = Image.FromFile(enemyMovementsDown[0]);
//            }
//        }

//        private void LoadEnemyDirectionImages(string direction, List<string> enemyMovementList)
//        {
//            string enemyFolder = $"Char_MoveMent/{direction}";  // Lấy thư mục theo từng hướng di chuyển
//            var directionImages = Directory.GetFiles(enemyFolder, "*.png").ToList();
//            enemyMovementList.AddRange(directionImages);  // Thêm các ảnh vào danh sách của từng hướng
//        }

//        // Phương thức để chọn ảnh cho Enemy dựa trên hướng di chuyển
//        public void AnimateEnemy(int start, int end)
//        {
//            enemyFrameRate++;  // Tăng tốc độ frame rate để điều khiển tốc độ animation
//            if (enemyFrameRate >= 5)  // Điều chỉnh số frame (khoảng cách thay đổi hình ảnh)
//            {
//                enemySteps++;
//                if (enemySteps > 5)  // Giới hạn số lượng frame từ 0 đến 6
//                {
//                    enemySteps = 0;
//                }
//                enemyFrameRate = 0;
//            }

//            string enemyImagePath = string.Empty;

//            // Chọn hình ảnh cho enemy dựa trên hướng di chuyển
//            switch (enemyDirection)
//            {
//                case "Left":
//                    enemyImagePath = enemyMovementsLeft[enemySteps];  // Chọn khung hình theo chỉ số từ 0 đến 6
//                    break;
//                case "Right":
//                    enemyImagePath = enemyMovementsRight[enemySteps];  // Chọn khung hình theo chỉ số từ 0 đến 6
//                    break;
//                case "Up":
//                    enemyImagePath = enemyMovementsUp[enemySteps];  // Chọn khung hình theo chỉ số từ 0 đến 6
//                    break;
//                case "Down":
//                    enemyImagePath = enemyMovementsDown[enemySteps];  // Chọn khung hình theo chỉ số từ 0 đến 6
//                    break;
//            }

//            if (!string.IsNullOrEmpty(enemyImagePath))
//            {
//                using (Image img = Image.FromFile(enemyImagePath))
//                {
//                    enemyImage = new Bitmap(img);  // Tạo bản sao của hình ảnh
//                }
//            }
//        }
//        public bool CheckCollisionWithObstacles(int newX, int newY, List<GameObject> obstacles)
//        {
//            // Kiểm tra va chạm với các vật thể (obstacles)
//            Rectangle enemyRect = new Rectangle(newX, newY, enemyWidth, enemyHeight);

//            foreach (var obstacle in obstacles)
//            {
//                Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
//                if (enemyRect.IntersectsWith(obstacleRect))
//                {
//                    return true;  // Nếu va chạm, trả về true
//                }
//            }
//            return false;  // Không va chạm
//        }

//        // Phương thức di chuyển của enemy (gọi MoveTowardsPlayer trong TimerEvent)
//        public void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
//        {
//            // Tính toán khoảng cách giữa enemy và player
//            int deltaX = playerX - enemyX;
//            int deltaY = playerY - enemyY;

//            // Di chuyển enemy về phía player theo từng trục
//            if (Math.Abs(deltaX) > enemySpeedX)
//            {
//                // Kiểm tra va chạm trước khi di chuyển
//                int newX = enemyX + Math.Sign(deltaX) * enemySpeedX;
//                if (!CheckCollisionWithObstacles(newX, enemyY, obstacles))  // Nếu không va chạm, di chuyển
//                {
//                    enemyX = newX;
//                    LoadMovementImages(deltaX < 0 ? "Left" : "Right");  // Chọn hướng di chuyển
//                }
//            }

//            if (Math.Abs(deltaY) > enemySpeedY)
//            {
//                // Kiểm tra va chạm trước khi di chuyển
//                int newY = enemyY + Math.Sign(deltaY) * enemySpeedY;
//                if (!CheckCollisionWithObstacles(enemyX, newY, obstacles))  // Nếu không va chạm, di chuyển
//                {
//                    enemyY = newY;
//                    LoadMovementImages(deltaY < 0 ? "Up" : "Down");  // Chọn hướng di chuyển
//                }
//            }

//            // Cập nhật animation của enemy dựa trên hướng di chuyển
//            AnimateEnemy(0, 6);  // Giới hạn frame từ 0 đến 6
//        }


//        // Phương thức để cập nhật hướng di chuyển của enemy
//        public void LoadMovementImages(string direction)
//        {
//            switch (direction)
//            {
//                case "Left":
//                    enemyDirection = "Left";
//                    break;
//                case "Right":
//                    enemyDirection = "Right";
//                    break;
//                case "Up":
//                    enemyDirection = "Up";
//                    break;
//                case "Down":
//                    enemyDirection = "Down";
//                    break;
//            }
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Project_Game.Entities
{
    public class Enemy
    {
        // Thuộc tính chung cho tất cả các enemy
        public Image enemyImage { get; protected set; }
        public int enemyX { get; protected set; }
        public int enemyY { get; protected set; }
        public int enemyHeight { get; protected set; } = 50;
        public int enemyWidth { get; protected set; } = 32;
        public int enemySpeed { get; protected set; } = 4;
        public string enemyDirection { get; protected set; } = "Down";

        // Danh sách ảnh di chuyển theo hướng
        protected List<string> enemyMovementsLeft = new List<string>();
        protected List<string> enemyMovementsRight = new List<string>();
        protected List<string> enemyMovementsUp = new List<string>();
        protected List<string> enemyMovementsDown = new List<string>();

        private int enemySteps = 0;
        private int enemyFrameRate = 0;

        // Constructor mặc định
        public Enemy(string baseFolderPath = "Char_MoveMent")
        {
            LoadEnemyImages(baseFolderPath);
        }

        // Load hình ảnh cho từng hướng di chuyển
        protected virtual void LoadEnemyImages(string baseFolderPath)
        {
            LoadEnemyDirectionImages("Left", enemyMovementsLeft, baseFolderPath);
            LoadEnemyDirectionImages("Right", enemyMovementsRight, baseFolderPath);
            LoadEnemyDirectionImages("Up", enemyMovementsUp, baseFolderPath);
            LoadEnemyDirectionImages("Down", enemyMovementsDown, baseFolderPath);

            if (enemyMovementsDown.Count > 0)
            {
                enemyImage = Image.FromFile(enemyMovementsDown[0]);
            }
        }

        protected void LoadEnemyDirectionImages(string direction, List<string> movementList, string baseFolderPath)
        {
            string folderPath = Path.Combine(baseFolderPath, direction);
            if (Directory.Exists(folderPath))
            {
                movementList.AddRange(Directory.GetFiles(folderPath, "*.png").ToList());
            }
        }

        // Hoạt ảnh di chuyển (có thể ghi đè)
        public virtual void AnimateEnemy(int start, int end)
        {
            enemyFrameRate++;
            if (enemyFrameRate >= 5)
            {
                enemySteps = (enemySteps + 1) % (end - start + 1);
                enemyFrameRate = 0;
            }

            string imagePath = null;

            if (enemyDirection == "Left")
            {
                imagePath = enemyMovementsLeft.ElementAtOrDefault(enemySteps);
            }
            else if (enemyDirection == "Right")
            {
                imagePath = enemyMovementsRight.ElementAtOrDefault(enemySteps);
            }
            else if (enemyDirection == "Up")
            {
                imagePath = enemyMovementsUp.ElementAtOrDefault(enemySteps);
            }
            else if (enemyDirection == "Down")
            {
                imagePath = enemyMovementsDown.ElementAtOrDefault(enemySteps);
            }

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                using (Image img = Image.FromFile(imagePath))
                {
                    enemyImage = new Bitmap(img);
                }
            }
        }

        // Di chuyển về phía người chơi (có thể ghi đè)
        public virtual void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            int deltaX = playerX - enemyX;
            int deltaY = playerY - enemyY;

            if (Math.Abs(deltaX) > enemySpeed)
            {
                int newX = enemyX + Math.Sign(deltaX) * enemySpeed;
                if (!CheckCollisionWithObstacles(newX, enemyY, obstacles))
                {
                    enemyX = newX;
                    enemyDirection = deltaX < 0 ? "Left" : "Right";
                }
            }

            if (Math.Abs(deltaY) > enemySpeed)
            {
                int newY = enemyY + Math.Sign(deltaY) * enemySpeed;
                if (!CheckCollisionWithObstacles(enemyX, newY, obstacles))
                {
                    enemyY = newY;
                    enemyDirection = deltaY < 0 ? "Up" : "Down";
                }
            }
        }
        public void SetPosition(int x, int y)
        {
            enemyX = x;
            enemyY = y;
        }
        // Kiểm tra va chạm với vật cản
        protected virtual bool CheckCollisionWithObstacles(int newX, int newY, List<GameObject> obstacles)
        {
            Rectangle enemyRect = new Rectangle(newX, newY, enemyWidth, enemyHeight);

            foreach (var obstacle in obstacles)
            {
                Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
                if (enemyRect.IntersectsWith(obstacleRect))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
