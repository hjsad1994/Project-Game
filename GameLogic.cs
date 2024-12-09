﻿using Project_Game;
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
                        Console.WriteLine("Player bị kẻ địch tấn công!");
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
                // Xử lý va chạm theo trục X
                if (player.PreviousX < obstacle.X)
                {
                    // Di chuyển từ bên trái
                    player.playerX = obstacle.X - player.playerWidth;
                    player.GoLeft = false;
                    Console.WriteLine("Player không thể đi sang trái vì va chạm với obstacle.");
                }
                else if (player.PreviousX > obstacle.X)
                {
                    // Di chuyển từ bên phải
                    player.playerX = obstacle.X + obstacle.Width;
                    player.GoRight = false;
                    Console.WriteLine("Player không thể đi sang phải vì va chạm với obstacle.");
                }

                // Xử lý va chạm theo trục Y
                if (player.PreviousY < obstacle.Y)
                {
                    // Di chuyển từ bên trên
                    player.playerY = obstacle.Y - player.playerHeight;
                    player.GoUp = false;
                    Console.WriteLine("Player không thể đi lên vì va chạm với obstacle.");
                }
                else if (player.PreviousY > obstacle.Y)
                {
                    // Di chuyển từ bên dưới
                    player.playerY = obstacle.Y + obstacle.Height;
                    player.GoDown = false;
                    Console.WriteLine("Player không thể đi xuống vì va chạm với obstacle.");
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

        // Di chuyển người chơi và lưu vị trí trước khi di chuyển
        player.PreviousX = player.playerX;
        player.PreviousY = player.playerY;

        if (player.IsAttacking)
        {
            player.UpdateAttack();
        }
        else
        {
            player.Move();
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

        // Kiểm tra va chạm sau khi di chuyển
        CheckCollision(healBar);
    }
}
