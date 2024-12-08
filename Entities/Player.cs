using System.Collections.Generic;
using System.Drawing;
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
    public int playerSpeed { get; set; } = 10;

    public int Health { get; private set; } = 100;
    public int MaxHealth { get; private set; } = 100;

    public AnimationManager movementAnimation { get; private set; }
    private AnimationManager idleAnimation;
    private AnimationManager attackAnimation;

    public bool IsAttacking { get; private set; } = false;
    private string currentDirection = "Down";

    public Player(List<PictureBox> obstacles)
    {
        movementAnimation = new AnimationManager(frameRate: 5);
        idleAnimation = new AnimationManager(frameRate: 8);
        attackAnimation = new AnimationManager(frameRate: 4);

        // Load default animations
        movementAnimation.LoadFrames("Char_MoveMent/MoveDown");
        idleAnimation.LoadFrames("Char_Idle/Down");
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
    }

    public void ResetHealth()
    {
        Health = MaxHealth;
    }

    public void Move()
    {
        if (IsAttacking) return; // Không di chuyển khi đang tấn công

        bool isMoving = false;

        if (GoLeft)
        {
            if (currentDirection != "Left")
            {
                movementAnimation.LoadFrames("Char_MoveMent/MoveLeft");
                currentDirection = "Left";
            }
            playerX -= playerSpeed;
            isMoving = true;
        }
        else if (GoRight)
        {
            if (currentDirection != "Right")
            {
                movementAnimation.LoadFrames("Char_MoveMent/MoveRight");
                currentDirection = "Right";
            }
            playerX += playerSpeed;
            isMoving = true;
        }
        else if (GoUp)
        {
            if (currentDirection != "Up")
            {
                movementAnimation.LoadFrames("Char_MoveMent/MoveUp");
                currentDirection = "Up";
            }
            playerY -= playerSpeed;
            isMoving = true;
        }
        else if (GoDown)
        {
            if (currentDirection != "Down")
            {
                movementAnimation.LoadFrames("Char_MoveMent/MoveDown");
                currentDirection = "Down";
            }
            playerY += playerSpeed;
            isMoving = true;
        }

        if (isMoving)
        {
            movementAnimation.UpdateAnimation(); // Cập nhật khung hình di chuyển
        }
        else
        {
            UpdateIdleAnimation();
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
        IsAttacking = false;
    }

    public void PerformAttack(Enemy target)
    {
        if (!IsAttacking)
        {
            IsAttacking = true;

            // Gây sát thương cho kẻ địch
            target.TakeDamage(10);

            // Hiển thị hoạt ảnh tấn công
            attackAnimation.LoadFrames($"Player_Attack/{currentDirection}");
            attackAnimation.ResetAnimation(); // Reset để hoạt ảnh bắt đầu từ đầu
        }
    }

    public void UpdateAttack()
    {
        if (IsAttacking)
        {
            attackAnimation.UpdateAnimation();
            if (attackAnimation.IsComplete())
            {
                IsAttacking = false;
            }
        }
    }

    private void UpdateIdleAnimation()
    {
        // Đảm bảo hoạt ảnh Idle khớp với hướng di chuyển cuối cùng
        switch (currentDirection)
        {
            case "Left":
                idleAnimation.LoadFrames("Char_Idle/Left");
                break;
            case "Right":
                idleAnimation.LoadFrames("Char_Idle/Right");
                break;
            case "Up":
                idleAnimation.LoadFrames("Char_Idle/Up");
                break;
            case "Down":
                idleAnimation.LoadFrames("Char_Idle/Down");
                break;
        }
        idleAnimation.ResetAnimation();
    }

    public Image GetCurrentFrame()
    {
        if (IsAttacking)
        {
            return attackAnimation.CurrentFrame;
        }
        return GoLeft || GoRight || GoUp || GoDown ? movementAnimation.CurrentFrame : idleAnimation.CurrentFrame;
    }
}
