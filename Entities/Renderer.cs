using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Project_Game.Entities
{
    public class Renderer
    {
        public void Render(Graphics canvas, Image bg, Player player, List<TestEnemy> enemies, List<Chicken> chickens, List<AnimatedObject> animatedObjects, bool gameOverState)
        {
            if (bg != null)
            {
                canvas.DrawImage(bg, 0, 0, 800, 600);
            }

            // Vẽ animated objects
            foreach (var animatedObject in animatedObjects)
            {
                animatedObject.Draw(canvas);
            }

            // Vẽ player
            var playerFrame = player.GetCurrentFrame();
            if (playerFrame != null)
            {
                canvas.DrawImage(playerFrame, player.playerX, player.playerY, player.playerWidth, player.playerHeight);
            }

            // Vẽ enemies
            foreach (var en in enemies)
            {
                var enemyFrame = en.GetCurrentFrame();
                if (enemyFrame != null)
                {
                    canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);

                    if (en is TestEnemy testEnemy)
                    {
                        using (Pen penAttack = new Pen(Color.Red, 2))
                        {
                            int centerX = testEnemy.X + testEnemy.Width / 2;
                            int centerY = testEnemy.Y + testEnemy.Height / 2;
                            canvas.DrawEllipse(penAttack, centerX - testEnemy.AttackRange, centerY - testEnemy.AttackRange, testEnemy.AttackRange * 2, testEnemy.AttackRange * 2);
                        }

                        using (Pen penDetect = new Pen(Color.Blue, 2))
                        {
                            int centerX = testEnemy.X + testEnemy.Width / 2;
                            int centerY = testEnemy.Y + testEnemy.Height / 2;
                            canvas.DrawEllipse(penDetect, centerX - testEnemy.DetectionRange, centerY - testEnemy.DetectionRange, testEnemy.DetectionRange * 2, testEnemy.DetectionRange * 2);
                        }
                    }
                }
            }

            // Vẽ chicken
            foreach (var chicken in chickens)
            {
                var chickenFrame = chicken.GetCurrentFrame();
                if (chickenFrame != null)
                {
                    canvas.DrawImage(chickenFrame, chicken.X, chicken.Y, chicken.Width, chicken.Height);
                }
            }

            // Vẽ GameOver nếu cần
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
