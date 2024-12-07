using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Project_Game.Entities;

public class Player
{
    public bool GoLeft { get; set; }
    public bool GoRight { get; set; }
    public bool GoUp { get; set; }
    public bool GoDown { get; set; }

    public int playerX { get; set; } = 100;
    public int playerY { get; set; } = 100;
    public int playerWidth { get; set; } = 32;
    public int playerHeight { get; set; } = 50;
    public int playerSpeed { get; set; } = 6;

    public int Health { get; private set; } = 100;
    public int MaxHealth { get; private set; } = 100;

    public AnimationManager movementAnimation { get; private set; } // Cho phép truy cập nhưng chỉ lớp Player có thể set giá trị.

    private AnimationManager idleAnimation;

    public Player(List<PictureBox> obstacles)
    {
        movementAnimation = new AnimationManager(frameRate: 5);
        idleAnimation = new AnimationManager(frameRate: 8);

        movementAnimation.LoadFrames("Char_MoveMent/MoveDown");
        idleAnimation.LoadFrames("Char_Idle/Down");
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0; // Đảm bảo máu không âm
    }
    public void ResetHealth()
    {
        Health = MaxHealth; // Khôi phục máu tối đa
    }
    public void Move()
    {
        if (GoLeft)
        {
            if (movementAnimation.CurrentDirection != "Left")
            {
                movementAnimation.LoadFrames("Char_MoveMent/MoveLeft");
                movementAnimation.CurrentDirection = "Left";
            }
            playerX -= playerSpeed;
            movementAnimation.UpdateAnimation();
        }
        else if (GoRight)
        {
            if (movementAnimation.CurrentDirection != "Right")
            {
                movementAnimation.LoadFrames("Char_MoveMent/MoveRight");
                movementAnimation.CurrentDirection = "Right";
            }
            playerX += playerSpeed;
            movementAnimation.UpdateAnimation();
        }
        else if (GoUp)
        {
            if (movementAnimation.CurrentDirection != "Up")
            {
                movementAnimation.LoadFrames("Char_MoveMent/MoveUp");
                movementAnimation.CurrentDirection = "Up";
            }
            playerY -= playerSpeed;
            movementAnimation.UpdateAnimation();
        }
        else if (GoDown)
        {
            if (movementAnimation.CurrentDirection != "Down")
            {
                movementAnimation.LoadFrames("Char_MoveMent/MoveDown");
                movementAnimation.CurrentDirection = "Down";
            }
            playerY += playerSpeed;
            movementAnimation.UpdateAnimation();
        }
        else
        {
            AnimateIdle();
        }
    }


    public void AnimateIdle()
    {
        idleAnimation.UpdateAnimation();
    }

    public void ResetPlayer()
    {
        playerX = 100;
        playerY = 100;
        Health = MaxHealth;
        GoLeft = GoRight = GoUp = GoDown = false;
    }

    public void PerformAttack(Enemy target)
    {
        // Giả sử tấn công gây 10 sát thương
        target.TakeDamage(10);
    }

    public Image GetCurrentFrame()
    {
        return GoLeft || GoRight || GoUp || GoDown ? movementAnimation.CurrentFrame : idleAnimation.CurrentFrame;
    }
}
