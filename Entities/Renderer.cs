using Project_Game.Entities;
using Project_Game;
using System.Drawing;
using System;

public class Renderer
{
    public void Render(
        Graphics canvas,
        Image bg,
        Player player,
        GameObjectManager objectManager,
        bool gameOverState)
    {
        // 1. Vẽ nền
        if (bg != null)
        {
            canvas.DrawImage(bg, 0, 0, 800, 600);
        }

        // 2. Vẽ StaticObjects
        foreach (var staticObj in objectManager.StaticObjects)
        {
            staticObj.Draw(canvas);
        }

        // 3. Vẽ Trees
        foreach (var tree in objectManager.Trees)
        {
            tree.Draw(canvas);
        }

        // 4. Vẽ AnimatedObjects
        foreach (var animatedObject in objectManager.AnimatedObjects)
        {
            animatedObject.Update();
            animatedObject.Draw(canvas);
        }

        // 5. Vẽ Enemies
        foreach (var en in objectManager.Enemies)
        {
            var enemyFrame = en.GetCurrentFrame();
            if (enemyFrame != null)
            {
                canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);
            }
        }

        // 6. Vẽ Chickens
        foreach (var chicken in objectManager.Chickens)
        {
            var chickenFrame = chicken.GetCurrentFrame();
            if (chickenFrame != null)
            {
                canvas.DrawImage(chickenFrame, chicken.X, chicken.Y, chicken.Width, chicken.Height);
            }
        }

        // 7. Vẽ Kapybaras
        foreach (var kapy in objectManager.Kapybaras)
        {
            kapy.Draw(canvas); // Đảm bảo rằng phương thức Draw đã được thêm vào Kapybara
        }

        // 8. Vẽ DroppedItems
        foreach (var droppedItem in objectManager.DroppedItems)
        {
            droppedItem.Draw(canvas);
        }
        var playerFrame = player.GetCurrentFrame();
        if (playerFrame != null)
        {
            canvas.DrawImage(playerFrame, player.X, player.Y, player.Width, player.Height);
        }
        // 10. Vẽ UI nếu game không kết thúc
        if (!gameOverState)
        {
            // Bạn có thể thêm các phần vẽ UI khác ở đây nếu cần
        }

        // 11. Vẽ trạng thái Game Over
        if (gameOverState)
        {
            using (Font font = new Font("Arial", 24, FontStyle.Bold))
            {
                canvas.DrawString("Game Over", font, Brushes.Red, new PointF(300, 250));
            }
        }
    }
}
