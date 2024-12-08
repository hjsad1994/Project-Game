using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class TestEnemy : Enemy
    {
        private AnimationManager movementAnimation; // Quản lý hoạt ảnh di chuyển
        private bool isDead = false; // Trạng thái kẻ địch
        private string currentAnimationDirection = ""; // Để lưu hướng đang được tải

        public bool ShouldRemove { get; private set; } = false; // Đánh dấu kẻ địch cần xóa

        public TestEnemy() : base("TestEnemy", maxHealth: 50)
        {
            enemySpeed = 15; // Đặt tốc độ kẻ địch là 6
            movementAnimation = new AnimationManager(frameRate: 5);
            LoadEnemyImages("Char_MoveMent");
        }

        public override void LoadEnemyImages(string baseFolderPath)
        {
            movementAnimation.LoadFrames(Path.Combine(baseFolderPath, "Down")); // Load mặc định hướng xuống
        }

        public override void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            if (isDead) return; // Không di chuyển nếu đã chết

            int deltaX = playerX - enemyX; // Khoảng cách theo trục X
            int deltaY = playerY - enemyY; // Khoảng cách theo trục Y

            const int threshold = 5; // Ngưỡng tối thiểu để thay đổi hướng

            // Di chuyển theo cả hai trục nếu vượt ngưỡng
            if (Math.Abs(deltaX) > threshold || Math.Abs(deltaY) > threshold)
            {
                if (Math.Abs(deltaX) > threshold)
                {
                    // Di chuyển theo trục X
                    if (deltaX > 0)
                    {
                        enemyX += enemySpeed / 2; // Chia đôi tốc độ để cân bằng khi di chuyển chéo
                        enemyDirection = "Right";
                    }
                    else
                    {
                        enemyX -= enemySpeed / 2;
                        enemyDirection = "Left";
                    }
                }

                if (Math.Abs(deltaY) > threshold)
                {
                    // Di chuyển theo trục Y
                    if (deltaY > 0)
                    {
                        enemyY += enemySpeed / 2; // Chia đôi tốc độ để cân bằng khi di chuyển chéo
                        enemyDirection = "Down";
                    }
                    else
                    {
                        enemyY -= enemySpeed / 2;
                        enemyDirection = "Up";
                    }
                }

                // Cập nhật hoạt ảnh dựa trên hướng
                UpdateDirectionAnimation();
            }

            // Cập nhật hoạt ảnh
            movementAnimation.UpdateAnimation();
        }
        private void UpdateDirectionAnimation()
        {
            // Kiểm tra nếu hướng hiện tại khác với hướng đã được tải trước đó
            if (enemyDirection != currentAnimationDirection)
            {
                currentAnimationDirection = enemyDirection;

                switch (enemyDirection)
                {
                    case "Left":
                        movementAnimation.LoadFrames("Char_MoveMent/Left");
                        break;
                    case "Right":
                        movementAnimation.LoadFrames("Char_MoveMent/Right");
                        break;
                    case "Up":
                        movementAnimation.LoadFrames("Char_MoveMent/Up");
                        break;
                    case "Down":
                        movementAnimation.LoadFrames("Char_MoveMent/Down");
                        break;
                }
            }
        }

        public override Image GetCurrentFrame()
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
    }
}
