using System;
using System.Collections.Generic;
using System.Drawing;

namespace Project_Game.Entities
{
    public class TestEnemy : Enemy
    {
        private AnimationManager movementAnimation; // Quản lý hoạt ảnh di chuyển

        public TestEnemy() : base("TestEnemy", maxHealth: 50)
        {
            movementAnimation = new AnimationManager(frameRate: 5);
            movementAnimation.LoadFrames("Char_MoveMent/Down");
        }

        public override void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            if (enemyX < playerX)
            {
                enemyX += enemySpeed;
                movementAnimation.LoadFrames("Char_MoveMent/Right");
            }
            else if (enemyX > playerX)
            {
                enemyX -= enemySpeed;
                movementAnimation.LoadFrames("Char_MoveMent/Left");
            }

            if (enemyY < playerY)
            {
                enemyY += enemySpeed;
                movementAnimation.LoadFrames("Char_MoveMent/Down");
            }
            else if (enemyY > playerY)
            {
                enemyY -= enemySpeed;
                movementAnimation.LoadFrames("Char_MoveMent/Up");
            }

            movementAnimation.UpdateAnimation();
        }

        public Image GetCurrentFrame()
        {
            return movementAnimation.CurrentFrame;
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            Console.WriteLine($"{Name} bị tấn công! Máu còn lại: {Health}");
        }
    }
}
