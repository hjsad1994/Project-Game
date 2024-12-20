using Project_Game.Entities;
using System.Drawing;
using System;

namespace Project_Game.Entities // Đảm bảo AnimatedObject thuộc về cùng namespace
{
    public class AnimatedObject 
    {
        private static readonly Random rand = new Random(); // Nguồn Random chung
        private AnimationManager animationManager;
        private int x, y, width, height;

        public AnimatedObject(string spriteSheetFolderPath, int x, int y, int width, int height, int frameRate = 5)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            animationManager = new AnimationManager(frameRate);
            animationManager.LoadFrames(spriteSheetFolderPath); // Tải các frame từ thư mục

            // Thiết lập frame bắt đầu ngẫu nhiên nếu có
            if (animationManager.GetFrameCount() > 0)
            {
                int initialFrame = rand.Next(0, animationManager.GetFrameCount());
                animationManager.SetCurrentFrame(initialFrame);
                Console.WriteLine($"AnimatedObject: Khởi tạo với frame {initialFrame}");

                // Thiết lập frameRateCounter ngẫu nhiên để phá đồng bộ hóa
                int randomCounter = rand.Next(0, animationManager.FrameRateLimit);
                animationManager.SetFrameRateCounter(randomCounter);
                Console.WriteLine($"AnimatedObject: Khởi tạo với frameRateCounter {randomCounter}");
            }
        }

        public void Update()
        {
            animationManager.UpdateAnimation();  // Cập nhật hoạt ảnh
        }

        public void Draw(Graphics g)
        {
            var currentFrame = animationManager.GetCurrentFrame();
            if (currentFrame != null)
            {
                g.DrawImage(currentFrame, x, y, width, height);  // Vẽ frame
            }
        }

        public void ResetAnimation()
        {
            animationManager.ResetAnimation();
        }

        public bool IsComplete()
        {
            return animationManager.IsComplete();
        }
    }
}

