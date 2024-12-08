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
                        player.TakeDamage(0);
                        healBar.Value = player.Health;
                    }
                    else
                    {
                        isGameOver = true;
                        ((Form1)Application.OpenForms["Form1"]).EndGame(player.Health, healBar);
                        return;
                    }
                }
            }
        }

        // Check collision with obstacles for player
        foreach (var obstacle in obstacles)
        {
            Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height);
            if (playerRect.IntersectsWith(obstacleRect))
            {
                if (player.GoLeft)
                {
                    player.playerX = obstacle.X + obstacle.Width;
                    player.GoLeft = false;
                }
                if (player.GoRight)
                {
                    player.playerX = obstacle.X - player.playerWidth;
                    player.GoRight = false;
                }
                if (player.GoUp)
                {
                    player.playerY = obstacle.Y + obstacle.Height;
                    player.GoUp = false;
                }
                if (player.GoDown)
                {
                    player.playerY = obstacle.Y - player.playerHeight;
                    player.GoDown = false;
                }
            }
        }
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

        CheckCollision(healBar);
    }
}
