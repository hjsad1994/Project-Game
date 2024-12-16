using System;
using System.Drawing;

namespace Project_Game.Entities
{
    public class StaticObject
    {
        protected Image sprite;
        protected int x, y, width, height;

        public StaticObject(string imagePath, int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
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

        public virtual void Draw(Graphics g)
        {
            if (sprite != null)
            {
                g.DrawImage(sprite, x, y, width, height);
            }
        }
    }
}
