using Project_Game;
using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class GameLogic
{
    private Player player;
    private Enemy enemy;
    private List<PictureBox> obstacles;
    private bool isGameOver = false;

    public GameLogic(Player player, Enemy enemy, List<PictureBox> obstacles)
    {
        this.player = player;
        this.enemy = enemy;
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
        Rectangle enemyRect = new Rectangle(enemy.enemyX, enemy.enemyY, enemy.enemyWidth, enemy.enemyHeight);

        // Kiểm tra va chạm giữa người chơi và kẻ địch
        if (playerRect.IntersectsWith(enemyRect))
        {
            if (player.Health > 0)
            {
                player.TakeDamage(0); // Gây sát thương cho người chơi
                healBar.Value = player.Health;
            }
            else
            {
                isGameOver = true;
                ((Form1)Application.OpenForms["Form1"]).EndGame(player.Health, healBar);
            }
        }

        // Kiểm tra va chạm giữa người chơi và vật cản
        foreach (var obstacle in obstacles)
        {
            Rectangle obstacleRect = new Rectangle(obstacle.Location.X, obstacle.Location.Y, obstacle.Width, obstacle.Height);

            if (playerRect.IntersectsWith(obstacleRect))
            {
                if (player.GoLeft)
                {
                    player.playerX = obstacle.Right;
                    player.GoLeft = false;
                }
                if (player.GoRight)
                {
                    player.playerX = obstacle.Left - player.playerWidth;
                    player.GoRight = false;
                }
                if (player.GoUp)
                {
                    player.playerY = obstacle.Bottom;
                    player.GoUp = false;
                }
                if (player.GoDown)
                {
                    player.playerY = obstacle.Top - player.playerHeight;
                    player.GoDown = false;
                }
            }
        }
    }

    public void ResetGameState()
    {
        isGameOver = false;
    }

    public void TimerEvent(object sender, EventArgs e, ProgressBar healBar)
    {
        if (isGameOver) return;

        // Di chuyển người chơi
        if (player.GoLeft && player.playerX > 0) player.playerX -= player.playerSpeed;
        if (player.GoRight && player.playerX + player.playerWidth < 2000) player.playerX += player.playerSpeed;
        if (player.GoUp && player.playerY > 0) player.playerY -= player.playerSpeed;
        if (player.GoDown && player.playerY + player.playerHeight < 6000) player.playerY += player.playerSpeed;

        // Cập nhật hoạt ảnh của người chơi
        if (player.GoLeft || player.GoRight || player.GoUp || player.GoDown)
        {
            player.movementAnimation.UpdateAnimation();
        }
        else
        {
            player.AnimateIdle();
        }

        // Tạo danh sách GameObject từ obstacles
        List<GameObject> gameObjects = obstacles
            .Select(obstacle => new GameObject(obstacle.Location.X, obstacle.Location.Y, obstacle.Width, obstacle.Height, obstacle.Name))
            .ToList();

        // Di chuyển và cập nhật hoạt ảnh của kẻ địch
        enemy.Move(player.playerX, player.playerY, 2000, 6000, gameObjects);
        enemy.AnimateEnemy(0, 5);

        // Kiểm tra va chạm
        CheckCollision(healBar);
    }
}
