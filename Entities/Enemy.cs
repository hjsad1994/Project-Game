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

        public virtual bool IsAttacking { get; protected set; } = false;
        public virtual bool ShouldRemove { get; protected set; } = false;

        private List<string> MovementsLeft = new List<string>();
        private List<string> MovementsRight = new List<string>();
        private List<string> MovementsUp = new List<string>();
        private List<string> MovementsDown = new List<string>();

        private int animationIndex = 0;
        private int frameCounter = 0;
        private int frameRate = 5;
        public virtual int AttackRange { get; } = 25; // Giá trị mặc định, có thể override trong lớp kế thừaa
        public virtual int DetectionRange { get; } = 300;
        public Enemy(string name, int maxHealth = 100)
            : base(0, 0, 22, 35, name, maxHealth) { }

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

        // Thêm tham số obstacles vào phương thức HandleAttackk
        public virtual void HandleAttack(Player player, List<GameObject> obstacles) { }

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

        public virtual void PerformAttack(Player target) { }

        public virtual void UpdateAttack() { }

        public virtual Image GetCurrentFrame() { return null; }
    }
}
