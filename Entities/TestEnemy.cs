using System;
using System.Collections.Generic;
using System.Drawing;

namespace Project_Game.Entities
{
    public class TestEnemy : Enemy
    {
        private AnimationManager movementAnimation; // Quản lý hoạt ảnh di chuyển
        private bool isDead = false; // Trạng thái kẻ địch

        public bool ShouldRemove { get; private set; } = false; // Đánh dấu kẻ địch cần xóa

        public TestEnemy() : base("TestEnemy", maxHealth: 50)
        {
            movementAnimation = new AnimationManager(frameRate: 5);
            movementAnimation.LoadFrames("Char_MoveMent/Down");
        }

        public override void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            if (isDead) return; // Không di chuyển nếu đã chết

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
            if (isDead) return;

            base.TakeDamage(damage);
            Console.WriteLine($"{Name} bị tấn công! Máu còn lại: {Health}");

            if (Health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            ShouldRemove = true; // Đánh dấu kẻ địch cần xóa khỏi giao diện
            Console.WriteLine($"{Name} đã chết!");
        }

        public bool IsDead()
        {
            return isDead;
        }
    }
}
