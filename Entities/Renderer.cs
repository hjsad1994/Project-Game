using Project_Game.Entities;
using System.Drawing;
using System.Collections.Generic;

namespace Project_Game.Entities
{
    public class Renderer
    {
        public void Render(
            Graphics canvas,
            Image bg,
            Player player,
            GameObjectManager objectManager, // Thêm parameter này
            bool gameOverState)
        {
            if (bg != null)
            {
                canvas.DrawImage(bg, 0, 0, 800, 600);
            }

            foreach (var animatedObject in objectManager.AnimatedObjects)
            {
                animatedObject.Update();
                animatedObject.Draw(canvas);
            }

            var playerFrame = player.GetCurrentFrame();
            if (playerFrame != null)
            {
                canvas.DrawImage(playerFrame, player.playerX, player.playerY, player.playerWidth, player.playerHeight);
            }

            foreach (var en in objectManager.Enemies)
            {
                var enemyFrame = en.GetCurrentFrame();
                if (enemyFrame != null)
                {
                    canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);
                }
            }

            foreach (var chicken in objectManager.Chickens)
            {
                var chickenFrame = chicken.GetCurrentFrame();
                if (chickenFrame != null)
                {
                    canvas.DrawImage(chickenFrame, chicken.X, chicken.Y, chicken.Width, chicken.Height);
                }
            }

            foreach (var kapy in objectManager.Kapybaras)
            {
                var kapyFrame = kapy.GetCurrentFrame();
                if (kapyFrame != null)
                {
                    canvas.DrawImage(kapyFrame, kapy.X, kapy.Y, kapy.Width, kapy.Height);
                }
            }

            // Vẽ các StaticObject
            foreach (var staticObj in objectManager.StaticObjects)
            {
                staticObj.Draw(canvas);
            }

            foreach (var en in objectManager.Enemies)
            {
                var enemyFrame = en.GetCurrentFrame();
                if (enemyFrame != null)
                {
                    canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);
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
