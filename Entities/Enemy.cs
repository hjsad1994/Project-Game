using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class Enemy : GameObject
    {
        public string Direction { get; protected set; } = "Down";
        public int Speed { get; protected set; } = 4;

        // Cho phép override
        public virtual bool IsAttacking { get; protected set; } = false;
        public virtual bool ShouldRemove { get; protected set; } = false;

        private List<string> MovementsLeft = new List<string>();
        private List<string> MovementsRight = new List<string>();
        private List<string> MovementsUp = new List<string>();
        private List<string> MovementsDown = new List<string>();

        private int animationIndex = 0;
        private int frameCounter = 0;
        private int frameRate = 5;

        public Enemy(string name, int maxHealth = 100)
            : base(0, 0, 32, 50, name, maxHealth) { }

        public virtual void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public virtual void LoadEnemyImages(string baseFolderPath)
        {
            LoadDirectionImages("Left", MovementsLeft, baseFolderPath);
            LoadDirectionImages("Right", MovementsRight, baseFolderPath);
            LoadDirectionImages("Up", MovementsUp, baseFolderPath);
            LoadDirectionImages("Down", MovementsDown, baseFolderPath);
        }

        private void LoadDirectionImages(string direction, List<string> movements, string baseFolderPath)
        {
            string path = Path.Combine(baseFolderPath, direction);
            if (Directory.Exists(path))
                movements.AddRange(Directory.GetFiles(path, "*.png"));
        }

        public void Animate()
        {
            frameCounter++;
            if (frameCounter >= frameRate)
            {
                frameCounter = 0;
                List<string> currentMovements = GetCurrentMovementList();
                if (currentMovements.Count > 0)
                {
                    animationIndex = (animationIndex + 1) % currentMovements.Count;
                }
            }
        }

        private List<string> GetCurrentMovementList()
        {
            if (Direction == "Left") return MovementsLeft;
            if (Direction == "Right") return MovementsRight;
            if (Direction == "Up") return MovementsUp;
            if (Direction == "Down") return MovementsDown;
            return new List<string>();
        }

        public virtual void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            int deltaX = playerX - X;
            int deltaY = playerY - Y;

            if (Math.Abs(deltaX) > Speed)
            {
                int newX = X + Math.Sign(deltaX) * Speed;
                if (!CheckCollisionWithObstacles(newX, Y, obstacles))
                {
                    X = newX;
                    Direction = deltaX < 0 ? "Left" : "Right";
                }
            }

            if (Math.Abs(deltaY) > Speed)
            {
                int newY = Y + Math.Sign(deltaY) * Speed;
                if (!CheckCollisionWithObstacles(X, newY, obstacles))
                {
                    Y = newY;
                    Direction = deltaY < 0 ? "Up" : "Down";
                }
            }
        }

        protected bool CheckCollisionWithObstacles(int newX, int newY, List<GameObject> obstacles)
        {
            Rectangle rect = new Rectangle(newX, newY, Width, Height);
            foreach (var obj in obstacles)
            {
                Rectangle objRect = new Rectangle(obj.X, obj.Y, obj.Width, obj.Height);
                if (rect.IntersectsWith(objRect))
                    return true;
            }
            return false;
        }

        public virtual void PerformAttack(Player target)
        {
            // Default: không có hành vi tấn công
        }

        public virtual void UpdateAttack()
        {
            // Default: không có cập nhật tấn công
        }

        public virtual void HandleAttack(Player player)
        {
            // Sẽ được ghi đè trong TestEnemy
        }

        public virtual Image GetCurrentFrame()
        {
            return null; // Lớp con sẽ override
        }
    }
}
