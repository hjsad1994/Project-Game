using System;
using System.Drawing;

namespace Project_Game.Entities
{
    public class SpriteSheet
    {
        private Image spriteSheetImage;  // Hình ảnh sprite sheet
        private int spriteWidth;         // Chiều rộng của mỗi sprite
        private int spriteHeight;        // Chiều cao của mỗi sprite

        public SpriteSheet(string filePath, int spriteWidth, int spriteHeight)
        {
            spriteSheetImage = Image.FromFile(filePath);
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
        }

        // Hàm lấy sprite từ vị trí (x, y) trong sprite sheet
        public Image GetSprite(int x, int y)
        {
            // Tạo hình ảnh mới từ một phần của sprite sheet
            Bitmap sprite = new Bitmap(spriteWidth, spriteHeight);
            Graphics g = Graphics.FromImage(sprite);
            // Cắt sprite tại (x, y) trong sprite sheet và sao chép vào ảnh mới
            g.DrawImage(spriteSheetImage, new Rectangle(0, 0, spriteWidth, spriteHeight),
                        new Rectangle(x, y, spriteWidth, spriteHeight), GraphicsUnit.Pixel);
            g.Dispose(); // Giải phóng tài nguyên của Graphics
            return sprite;
        }

        // Hàm vẽ sprite lên canvas tại toạ độ cụ thể trên màn hình
        public void DrawSprite(Graphics g, int screenX, int screenY, int spriteX, int spriteY)
        {
            // Lấy sprite từ sprite sheet với tọa độ (spriteX, spriteY)
            Image sprite = GetSprite(spriteX, spriteY);
            // Vẽ sprite lên canvas tại vị trí (screenX, screenY) trên màn hình
            g.DrawImage(sprite, screenX, screenY, spriteWidth, spriteHeight);
        }
    }
}
