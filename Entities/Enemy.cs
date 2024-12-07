using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class Enemy
    {
        public string Name { get; protected set; } // Tên của Enemy
        public int Health { get; protected set; } // Máu của Enemy
        public int MaxHealth { get; protected set; } // Máu tối đa

        public int enemyX { get; protected set; } = 0;
        public int enemyY { get; protected set; } = 0;
        public int enemySpeed { get; protected set; } = 4;

        public int enemyWidth { get; protected set; } = 32;
        public int enemyHeight { get; protected set; } = 50;

        public string enemyDirection { get; protected set; } = "Down";

        public Image EnemyImage { get; protected set; }

        protected List<string> EnemyMovementsLeft = new List<string>();
        protected List<string> EnemyMovementsRight = new List<string>();
        protected List<string> EnemyMovementsUp = new List<string>();
        protected List<string> EnemyMovementsDown = new List<string>();

        private int enemySteps = 0;
        private int enemyFrameRate = 0;

        public Enemy(string name, int maxHealth = 100)
        {
            Name = name;
            Health = maxHealth;
            MaxHealth = maxHealth;
        }

        public virtual void LoadEnemyImages(string baseFolderPath = "Char_MoveMent")
        {
            LoadEnemyDirectionImages("Left", EnemyMovementsLeft, baseFolderPath);
            LoadEnemyDirectionImages("Right", EnemyMovementsRight, baseFolderPath);
            LoadEnemyDirectionImages("Up", EnemyMovementsUp, baseFolderPath);
            LoadEnemyDirectionImages("Down", EnemyMovementsDown, baseFolderPath);

            if (EnemyMovementsDown.Count > 0)
            {
                EnemyImage = Image.FromFile(EnemyMovementsDown[0]);
            }
        }

        protected void LoadEnemyDirectionImages(string direction, List<string> movementList, string baseFolderPath)
        {
            string folderPath = Path.Combine(baseFolderPath, direction);
            if (Directory.Exists(folderPath))
            {
                movementList.AddRange(Directory.GetFiles(folderPath, "*.png"));
            }
        }

        public virtual void AnimateEnemy(int start, int end)
        {
            enemyFrameRate++;
            if (enemyFrameRate >= 5)
            {
                enemySteps = (enemySteps + 1) % (end - start + 1);
                enemyFrameRate = 0;
            }

            string imagePath = null;

            if (enemyDirection == "Left" && EnemyMovementsLeft.Count > 0)
            {
                imagePath = EnemyMovementsLeft[enemySteps % EnemyMovementsLeft.Count];
            }
            else if (enemyDirection == "Right" && EnemyMovementsRight.Count > 0)
            {
                imagePath = EnemyMovementsRight[enemySteps % EnemyMovementsRight.Count];
            }
            else if (enemyDirection == "Up" && EnemyMovementsUp.Count > 0)
            {
                imagePath = EnemyMovementsUp[enemySteps % EnemyMovementsUp.Count];
            }
            else if (enemyDirection == "Down" && EnemyMovementsDown.Count > 0)
            {
                imagePath = EnemyMovementsDown[enemySteps % EnemyMovementsDown.Count];
            }
            else
            {
                Console.WriteLine("No images available for current direction.");
                return; // Thoát nếu không có hình ảnh
            }

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                using (Image img = Image.FromFile(imagePath))
                {
                    EnemyImage = new Bitmap(img);
                }
            }
        }


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

        protected bool CheckCollisionWithObstacles(int newX, int newY, List<GameObject> obstacles)
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

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }
        public bool IsDead()
        {
            return Health <= 0;
        }
    }
}
