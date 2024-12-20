using Project_Game.Entities;
using System.Drawing;
using System.IO;
using System;

public class Bullet : GameObject
{
    private AnimationManager animation;
    private string direction;
    private float speed = 1.5f;
    private int damage = 5;
    private int maxDistance = 200;
    private int startXPos;
    private int startYPos;

    public Bullet(string direction, int startX, int startY, string baseFolderPath)
        : base(startX, startY, 32, 32, "Bullet", 1)
    {
        this.direction = direction;

        animation = new AnimationManager(frameRate: 15);
        string bulletDirPath = Path.Combine(baseFolderPath, direction);
        animation.LoadFrames(bulletDirPath);
    }

    public void SetMaxDistance(int distance)
    {
        maxDistance = distance;
    }

    public void SetStartPosition(int x, int y)
    {
        startXPos = x;
        startYPos = y;
    }

    public override void Update()
    {
        base.Update();
        // Di chuyển bullet theo direction
        switch (direction)
        {
            case "Left":
                X -= (int)speed;
                break;
            case "Right":
                X += (int)speed;
                break;
            case "Up":
                Y -= (int)speed;
                break;
            case "Down":
                Y += (int)speed;
                break;
        }

        // Update animation
        animation.UpdateAnimation();

        // Tính khoảng cách đã bay
        int distX = X - startXPos;
        int distY = Y - startYPos;
        double distanceTravelled = Math.Sqrt(distX * distX + distY * distY);

        if (distanceTravelled >= maxDistance)
        {
            ShouldRemove = true;
        }
    }

    public void CheckHit(Player player)
    {
        Rectangle bulletRect = new Rectangle(X, Y, Width, Height);
        Rectangle playerRect = new Rectangle(player.X, player.Y, player.Width, player.Height);

        if (bulletRect.IntersectsWith(playerRect))
        {
            player.TakeDamage(damage);
            ShouldRemove = true;
            Console.WriteLine("[Info] Player bị trúng đạn từ Skeleton Mage!");
        }
    }

    public Image GetCurrentFrame()
    {
        return animation.GetCurrentFrame();
    }

    public override void Draw(Graphics g)
    {
        Image frame = GetCurrentFrame();
        if (frame != null)
        {
            g.DrawImage(frame, X, Y, Width, Height);
        }
    }
}
