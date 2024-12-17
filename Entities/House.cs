using System;
using System.Drawing;

namespace Project_Game.Entities
{
    public class House : StaticObject
    {
        public House(string imagePath, int x, int y, int width, int height)
            : base(imagePath, x, y, width, height)
        {
            // Bạn có thể thêm các logic bổ sung nếu cần
            Console.WriteLine($"[Info] House tại ({x}, {y}) đã được tạo với hình ảnh {imagePath}");
        }
    }
}
