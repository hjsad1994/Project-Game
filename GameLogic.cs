using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Project_Game
{
    public class GameLogic
    {
        private Player player;
        private List<Enemy> enemies;
        private List<GameObject> obstacles;
        private bool isGameOver = false;
        public Player PlayerInstance => player;

        public GameLogic(Player player, List<Enemy> enemies, List<GameObject> obstacles)
        {
            this.player = player;
            this.enemies = enemies;
            this.obstacles = obstacles;
        }

        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (isGameOver) return;

            if (e.KeyCode == Keys.Left) player.GoLeft = true;
            if (e.KeyCode == Keys.Right) player.GoRight = true;
            if (e.KeyCode == Keys.Up) player.GoUp = true;
            if (e.KeyCode == Keys.Down) player.GoDown = true;
        }

        public void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (isGameOver) return;

            if (e.KeyCode == Keys.Left) player.GoLeft = false;
            if (e.KeyCode == Keys.Right) player.GoRight = false;
            if (e.KeyCode == Keys.Up) player.GoUp = false;
            if (e.KeyCode == Keys.Down) player.GoDown = false;
        }

        public void CheckCollision(ProgressBar healBar)
        {
            Rectangle playerRect = new Rectangle(player.playerX, player.playerY, player.playerWidth, player.playerHeight);

            // Check collision with enemies
            foreach (var enemy in enemies)
            {
                if (!enemy.IsDead())
                {
                    Rectangle enemyRect = new Rectangle(enemy.X, enemy.Y, enemy.Width, enemy.Height);
                    if (playerRect.IntersectsWith(enemyRect))
                    {
                        if (player.Health > 0)
                        {
                            // Xử lý tấn công từ enemy đã được thực hiện trong TestEnemy.HandleAttack()
                            Console.WriteLine("Player bị kẻ địch tấn công!");
                        }
                        else
                        {
                            Console.WriteLine("Player Health <=0, EndGame được gọi.");
                            isGameOver = true;
                            ((Form1)Application.OpenForms["Form1"]).EndGame(player.Health, healBar);
                            return;
                        }
                    }
                }
            }

            // Loại bỏ phần xử lý va chạm với obstacles
        }

        public void ResetGameState()
        {
            isGameOver = false;
        }

        public void SetEnemies(List<Enemy> newEnemies)
        {
            this.enemies = newEnemies;
        }

        public void TimerEvent(object sender, EventArgs e, ProgressBar healBar)
        {
            if (isGameOver) return;

            // Di chuyển người chơi và lưu vị trí trước khi di chuyển
            player.PreviousX = player.playerX;
            player.PreviousY = player.playerY;

            if (player.IsAttacking)
            {
                player.UpdateAttack();
            }
            else
            {
                player.Move(); // Truyền danh sách enemies
            }

            // Di chuyển và xử lý va chạm của kẻ địch
            foreach (var enemy in enemies)
            {
                if (!enemy.IsDead())
                {
                    if (enemy.IsAttacking)
                    {
                        enemy.UpdateAttack();
                    }
                    else
                    {
                        // Truyền obstacles vào HandleAttack
                        enemy.HandleAttack(player, obstacles);
                    }

                    enemy.Animate();
                }
            }

            // Kiểm tra va chạm sau khi di chuyển (chỉ để xử lý Game Over nếu cần)
            CheckCollision(healBar);

            // Cập nhật healBar.Value dựa trên player.Health
            if (player.Health >= healBar.Minimum && player.Health <= healBar.Maximum)
            {
                healBar.Value = player.Health;
                //Console.WriteLine($"healBar.Value được cập nhật thành {player.Health}");
            }
            else
            {
                Console.WriteLine($"player.Health = {player.Health} không hợp lệ cho healBar.Value");
            }

            // Kiểm tra Game Over
            if (player.Health <= 0 && !isGameOver)
            {
                isGameOver = true;
                ((Form1)Application.OpenForms["Form1"]).EndGame(player.Health, healBar);
            }
        }
    }
}
