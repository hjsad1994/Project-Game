using System;
using System.Drawing;

namespace Project_Game.Entities
{
    public class StaticObject : GameObject
    {
        private Image sprite;

        public StaticObject(string imagePath, int x, int y, int width, int height)
            : base(x, y, width, height, "StaticObject", 9999) // 9999 máu giả, hoặc 100, tuỳ ý
        {
            LoadSprite(imagePath);
        }

        private void LoadSprite(string imagePath)
        {
            try
            {
                sprite = Image.FromFile(imagePath);
                Console.WriteLine($"Loaded StaticObject image: {imagePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image {imagePath}: {ex.Message}");
                sprite = null;
            }
        }

        public override void TakeDamage(int damage)
        {
            // Đối tượng tĩnh không bị phá huỷ, nên có thể bỏ trống hoặc không giảm máu.
        }

        public void Draw(Graphics g)
        {
            if (sprite != null)
            {
                g.DrawImage(sprite, X, Y, Width, Height);
            }
        }
    }
}
