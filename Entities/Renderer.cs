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
        // Vẽ nền
        if (bg != null)
        {
            canvas.DrawImage(bg, 0, 0, 800, 600);
        }

        // Vẽ các AnimatedObject
        foreach (var animatedObject in objectManager.AnimatedObjects)
        {
            animatedObject.Update();
            animatedObject.Draw(canvas);
        }

        // Vẽ DroppedItems
        foreach (var droppedItem in objectManager.DroppedItems)
        {
            droppedItem.Draw(canvas);
        }

        // Vẽ người chơi
        var playerFrame = player.GetCurrentFrame();
        if (playerFrame != null)
        {
            canvas.DrawImage(playerFrame, player.X, player.Y, player.Width, player.Height);
        }

        // Vẽ kẻ thù
        foreach (var en in objectManager.Enemies)
        {
            var enemyFrame = en.GetCurrentFrame();
            if (enemyFrame != null)
            {
                canvas.DrawImage(enemyFrame, en.X, en.Y, en.Width, en.Height);
            }
        }

        // Vẽ gà
        foreach (var chicken in objectManager.Chickens)
        {
            var chickenFrame = chicken.GetCurrentFrame();
            if (chickenFrame != null)
            {
                canvas.DrawImage(chickenFrame, chicken.X, chicken.Y, chicken.Width, chicken.Height);
            }
        }

        // Vẽ Kapybara
        foreach (var kapy in objectManager.Kapybaras)
        {
            var kapyFrame = kapy.GetCurrentFrame();
            if (kapyFrame != null)
            {
                canvas.DrawImage(kapyFrame, kapy.X, kapy.Y, kapy.Width, kapy.Height);
            }
        }

        // Vẽ các StaticObject
        Console.WriteLine($"[Debug] Rendering {objectManager.StaticObjects.Count} StaticObjects.");
        foreach (var staticObj in objectManager.StaticObjects)
        {
            staticObj.Draw(canvas);
        }

        // Vẽ các DroppedItem
        Console.WriteLine($"[Debug] Rendering {objectManager.DroppedItems.Count} DroppedItems.");
        foreach (var droppedItem in objectManager.DroppedItems)
        {
            droppedItem.Draw(canvas);
        }

        // Vẽ các cây
        Console.WriteLine($"[Debug] Rendering {objectManager.Trees.Count} Trees.");
        foreach (var tree in objectManager.Trees)
        {
            tree.Draw(canvas);
        }

        // Vẽ UI nếu game không kết thúc
        if (!gameOverState)
        {
            // Bạn có thể thêm các phần vẽ UI khác ở đây nếu cần
        }

        // Vẽ trạng thái Game Over
        if (gameOverState)
        {
            using (Font font = new Font("Arial", 24, FontStyle.Bold))
            {
                canvas.DrawString("Game Over", font, Brushes.Red, new PointF(300, 250));
            }
        }
    }
}
