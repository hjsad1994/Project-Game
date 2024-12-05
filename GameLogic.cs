using Project_Game.Entities;
using Project_Game;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

public class GameLogic
{
    private Player player;
    private Enemy enemy;
    private List<PictureBox> obstacles;
    private bool isGameOver = false;  // Thêm flag để kiểm tra trạng thái Game Over

    public GameLogic(Player player, Enemy enemy, List<PictureBox> obstacles)
    {
        this.player = player;
        this.enemy = enemy;
        this.obstacles = obstacles;
    }

    public void KeyIsDown(object sender, KeyEventArgs e)
    {
        if (isGameOver) return;  // Nếu game over, không nhận thêm sự kiện phím

        if (e.KeyCode == Keys.Left) player.goLeft = true;
        if (e.KeyCode == Keys.Right) player.goRight = true;
        if (e.KeyCode == Keys.Up) player.goUp = true;
        if (e.KeyCode == Keys.Down) player.goDown = true;
    }

    public void KeyIsUp(object sender, KeyEventArgs e)
    {
        if (isGameOver) return;  // Nếu game over, không nhận thêm sự kiện phím

        if (e.KeyCode == Keys.Left) player.goLeft = false;
        if (e.KeyCode == Keys.Right) player.goRight = false;
        if (e.KeyCode == Keys.Up) player.goUp = false;
        if (e.KeyCode == Keys.Down) player.goDown = false;
    }

    public void CheckCollision(ProgressBar healBar)
    {
        Rectangle playerRect = new Rectangle(player.playerX, player.playerY, player.playerWidth, player.playerHeight);
        Rectangle enemyRect = new Rectangle(enemy.enemyX, enemy.enemyY, enemy.enemyWidth, enemy.enemyHeight);

        // Kiểm tra va chạm với enemy
        if (playerRect.IntersectsWith(enemyRect))
        {
            if (player.currentHealth > 0)
            {
                player.currentHealth -= 0;  // Giảm 10 máu mỗi lần va chạm
                healBar.Value = player.currentHealth;  // Cập nhật giá trị của healBar
            }
            else
            {
                isGameOver = true;  // Đặt cờ game over
                ((Form1)Application.OpenForms["Form1"]).EndGame(player.currentHealth, healBar);  // Gọi EndGame để reset game
            }
        }

        // Kiểm tra va chạm với các vật cản
        foreach (var obstacle in obstacles)
        {
            Rectangle obstacleRect = new Rectangle(obstacle.Location, obstacle.Size);

            // Kiểm tra va chạm trên trục X (trái/phải)
            if (playerRect.IntersectsWith(obstacleRect))
            {
                if (player.goLeft)
                {
                    // Dừng di chuyển sang trái nếu va chạm với vật cản
                    player.playerX = obstacle.Right;  // Đặt player ngay phía bên phải của vật cản
                    player.goLeft = false;  // Dừng di chuyển sang trái
                }
                if (player.goRight)
                {
                    // Dừng di chuyển sang phải nếu va chạm với vật cản
                    player.playerX = obstacle.Left - player.playerWidth;  // Đặt player ngay phía bên trái của vật cản
                    player.goRight = false;  // Dừng di chuyển sang phải
                }
            }

            // Kiểm tra va chạm trên trục Y (lên/xuống)
            if (playerRect.IntersectsWith(obstacleRect))
            {
                if (player.goUp)
                {
                    // Dừng di chuyển lên nếu va chạm với vật cản
                    player.playerY = obstacle.Bottom;  // Đặt player ngay phía dưới vật cản
                    player.goUp = false;  // Dừng di chuyển lên
                }
                if (player.goDown)
                {
                    // Dừng di chuyển xuống nếu va chạm với vật cản
                    player.playerY = obstacle.Top - player.playerHeight;  // Đặt player ngay phía trên vật cản
                    player.goDown = false;  // Dừng di chuyển xuống
                }
            }
        }
    }
    public void ResetGameState()
    {
        isGameOver = false;  // Đặt lại trạng thái isGameOver
    }
    public void TimerEvent(object sender, EventArgs e, ProgressBar healBar)
    {
        if (isGameOver) return;

        // Cập nhật vị trí người chơi
        if (player.goLeft && player.playerX > 0) player.playerX -= player.playerSpeed;
        if (player.goRight && player.playerX + player.playerWidth < 2000) player.playerX += player.playerSpeed;
        if (player.goUp && player.playerY > 0) player.playerY -= player.playerSpeed;
        if (player.goDown && player.playerY + player.playerHeight < 6000) player.playerY += player.playerSpeed;

        // Cập nhật hình ảnh người chơi
        if (player.goLeft || player.goRight || player.goUp || player.goDown)
            player.AnimatePlayer(0, 5);
        else
            player.AnimateIdle(0, player.idleMovements.Count - 1);

        // Cập nhật vị trí và hình ảnh của kẻ thù, truyền tọa độ player vào và kích thước màn hình
        enemy.Move(player.playerX, player.playerY, 2000, 6000);  // Truyền kích thước màn hình vào
        enemy.AnimateEnemy(0, 5);

        // Kiểm tra va chạm giữa player và enemy
        CheckCollision(healBar);
    }
}