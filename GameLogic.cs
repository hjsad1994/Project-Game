using Project_Game.Entities;
using Project_Game;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Linq;

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

        if (e.KeyCode == Keys.Left) player.goLeft = true;
        if (e.KeyCode == Keys.Right) player.goRight = true;
        if (e.KeyCode == Keys.Up) player.goUp = true;
        if (e.KeyCode == Keys.Down) player.goDown = true;
    }

    public void KeyIsUp(object sender, KeyEventArgs e)
    {
        if (isGameOver) return;

        if (e.KeyCode == Keys.Left) player.goLeft = false;
        if (e.KeyCode == Keys.Right) player.goRight = false;
        if (e.KeyCode == Keys.Up) player.goUp = false;
        if (e.KeyCode == Keys.Down) player.goDown = false;
    }

    public void CheckCollision(ProgressBar healBar)
    {
        Rectangle playerRect = new Rectangle(player.playerX, player.playerY, player.playerWidth, player.playerHeight);
        Rectangle enemyRect = new Rectangle(enemy.enemyX, enemy.enemyY, enemy.enemyWidth, enemy.enemyHeight);

        if (playerRect.IntersectsWith(enemyRect))
        {
            if (player.currentHealth > 0)
            {
                player.currentHealth -= 1;
                healBar.Value = player.currentHealth;
            }
            else
            {
                isGameOver = true;
                ((Form1)Application.OpenForms["Form1"]).EndGame(player.currentHealth, healBar);
            }
        }

        foreach (var obstacle in obstacles)
        {
            Rectangle obstacleRect = new Rectangle(obstacle.Location, obstacle.Size);

            if (playerRect.IntersectsWith(obstacleRect))
            {
                if (player.goLeft)
                {
                    player.playerX = obstacle.Right;
                    player.goLeft = false;
                }
                if (player.goRight)
                {
                    player.playerX = obstacle.Left - player.playerWidth;
                    player.goRight = false;
                }
            }

            if (playerRect.IntersectsWith(obstacleRect))
            {
                if (player.goUp)
                {
                    player.playerY = obstacle.Bottom;
                    player.goUp = false;
                }
                if (player.goDown)
                {
                    player.playerY = obstacle.Top - player.playerHeight;
                    player.goDown = false;
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

        if (player.goLeft && player.playerX > 0) player.playerX -= player.playerSpeed;
        if (player.goRight && player.playerX + player.playerWidth < 2000) player.playerX += player.playerSpeed;
        if (player.goUp && player.playerY > 0) player.playerY -= player.playerSpeed;
        if (player.goDown && player.playerY + player.playerHeight < 6000) player.playerY += player.playerSpeed;

        if (player.goLeft || player.goRight || player.goUp || player.goDown)
            player.AnimatePlayer(0, 5);
        else
            player.AnimateIdle(0, player.idleMovements.Count - 1);

        List<GameObject> gameObjects = obstacles
            .Select(obstacle => new GameObject(obstacle.Location.X, obstacle.Location.Y, obstacle.Width, obstacle.Height, obstacle.Name))
            .ToList();

        enemy.Move(player.playerX, player.playerY, 2000, 6000, gameObjects);
        enemy.AnimateEnemy(0, 5);

        CheckCollision(healBar);
    }
}
