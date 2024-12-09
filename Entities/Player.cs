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
    public int PreviousX { get; set; }
    public int PreviousY { get; set; }
    public int playerX { get; set; } = 100;
    public int playerY { get; set; } = 100;
    public int playerWidth { get; set; } = 22;
    public int playerHeight { get; set; } = 35;
    public int playerSpeed { get; set; } = 3;

    public int Health { get; private set; } = 100;
    public int MaxHealth { get; private set; } = 100;

    public AnimationManager movementAnimation { get; private set; }
    private AnimationManager idleAnimation;
    private AnimationManager attackAnimation;
    private bool wasMovingPreviously = false;
    public bool IsAttacking { get; private set; } = false;
    private string currentDirection = "Down";

    public Player(List<PictureBox> obstacles)
    {
        movementAnimation = new AnimationManager(frameRate: 5);
        idleAnimation = new AnimationManager(frameRate: 6);
        attackAnimation = new AnimationManager(frameRate: 5);

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
        if (IsAttacking) return;

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
            movementAnimation.UpdateAnimation();
        }
        else
        {
            // Nếu frame trước đang di chuyển, còn frame này không di chuyển nữa, nghĩa là vừa dừng
            if (wasMovingPreviously)
            {
                UpdateIdleAnimation();
            }
            AnimateIdle();
        }

        // Cập nhật trạng thái wasMovingPreviously cho frame tiếp theo
        wasMovingPreviously = isMoving;
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
            attackAnimation.LoadFrames($"Player_Attack/{currentDirection}");
            attackAnimation.ResetAnimation();

            target.TakeDamage(50);
        }
    }


    public void UpdateAttack()
    {
        if (IsAttacking)
        {
            // Đảm bảo animation attack không loop
            attackAnimation.UpdateAnimation(loop: false);
            if (attackAnimation.IsComplete())
            {
                IsAttacking = false;
            }
        }
    }

    private void UpdateIdleAnimation()
    {
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
