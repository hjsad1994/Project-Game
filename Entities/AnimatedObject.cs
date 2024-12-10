using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Game.Entities
{
    public class AnimatedObject
    {
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
