using Project_Game.Entities;
using System.Drawing;
using System.Collections.Generic;

namespace Project_Game.Entities
{
    public class Renderer
    {
        public void Render(Graphics canvas, Image bg, Player player, List<TestEnemy> enemies, List<Chicken> chickens, List<AnimatedObject> animatedObjects, List<Kapybara> kapybaras, bool gameOverState)
        {
            if (bg != null)
            {
                canvas.DrawImage(bg, 0, 0, 800, 600);
            }

            foreach (var animatedObject in animatedObjects)
            {
                animatedObject.Update();
                animatedObject.Draw(canvas);
            }

            var playerFrame = player.GetCurrentFrame();
            if (playerFrame != null)
            {
                // Vẽ player theo kích thước playerWidth, playerHeight
                canvas.DrawImage(playerFrame, player.playerX, player.playerY, player.playerWidth, player.playerHeight);
            }

            foreach (var en in enemies)
            {
                var enemyFrame = en.GetCurrentFrame();
                if (enemyFrame != null)
                {
                    // Vẽ enemy theo Width, Height
                    canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);
                }
            }

            foreach (var chicken in chickens)
            {
                var chickenFrame = chicken.GetCurrentFrame();
                if (chickenFrame != null)
                {
                    // Gà vẫn 20x20, bạn có thể thay đổi kích thước gà tương tự
                    canvas.DrawImage(chickenFrame, chicken.X, chicken.Y, chicken.Width, chicken.Height);
                }
            }

            foreach (var kapy in kapybaras)
            {
                var kapyFrame = kapy.GetCurrentFrame();
                if (kapyFrame != null)
                {
                    // Kapybara có thể thay đổi Width, Height tương tự
                    canvas.DrawImage(kapyFrame, kapy.X, kapy.Y, kapy.Width, kapy.Height);
                }
            }

            if (gameOverState)
            {
                using (Font font = new Font("Arial", 24, FontStyle.Bold))
                {
                    canvas.DrawString("Game Over", font, Brushes.Red, new PointF(300, 250));
                }
            }
        }
    }
}
