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
    public Player PlayerInstance => player;

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

        // Chỉ kiểm tra va chạm với enemy nếu enemy chưa chết
        if (enemy != null && !enemy.IsDead())
        {
            Rectangle enemyRect = new Rectangle(enemy.X, enemy.Y, enemy.Width, enemy.Height);

            if (playerRect.IntersectsWith(enemyRect))
            {
                if (player.Health > 0)
                {
                    player.TakeDamage(0); // damage 0 tạm thời
                    healBar.Value = player.Health;
                }
                else
                {
                    isGameOver = true;
                    ((Form1)Application.OpenForms["Form1"]).EndGame(player.Health, healBar);
                }
            }
        }

        // Kiểm tra va chạm giữa player và obstacles
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
        // Player đã có logic di chuyển riêng trong Player.Move(), 
        // nhưng ở đây bạn có di chuyển thô, có thể bỏ nếu đã di chuyển trong Player
        // Hoặc giữ logic này nếu bạn muốn Player di chuyển bằng GameLogic
        // Ở đây có vẻ bạn đang dùng Player.Move() ở nơi khác, nên có thể bỏ đoạn này:
        // if (player.GoLeft && player.playerX > 0) player.playerX -= player.playerSpeed;
        // if (player.GoRight && player.playerX + player.playerWidth < 2000) player.playerX += player.playerSpeed;
        // if (player.GoUp && player.playerY > 0) player.playerY -= player.playerSpeed;
        // if (player.GoDown && player.playerY + player.playerHeight < 6000) player.playerY += player.playerSpeed;

        // Nếu bạn đã gọi player.Move() ở Form1 thì ở đây không cần gọi thêm.
        // Đảm bảo chỉ gọi Move() một nơi.

        // Cập nhật hoạt ảnh người chơi (nếu bạn không gọi Move() ở đây, không cần đoạn này)
        // if (player.GoLeft || player.GoRight || player.GoUp || player.GoDown)
        // {
        //     player.movementAnimation.UpdateAnimation();
        // }
        // else
        // {
        //     player.AnimateIdle();
        // }

        // Hành vi kẻ địch
        if (enemy != null && !enemy.IsDead())
        {
            if (enemy.IsAttacking)
            {
                enemy.UpdateAttack(); // Cập nhật trạng thái tấn công
            }
            else
            {
                enemy.HandleAttack(player); // Kiểm tra tấn công hoặc di chuyển tới player
            }

            enemy.Animate(); // Cập nhật hoạt ảnh
        }

        CheckCollision(healBar); // Kiểm tra va chạm
    }
}
