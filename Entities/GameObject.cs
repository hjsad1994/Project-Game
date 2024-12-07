﻿using System;
using System.Drawing;

namespace Project_Game.Entities
{
    public class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int Health { get; private set; } // Thuộc tính máu

        public GameObject(int x, int y, int width, int height, string name, int health = 100)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = name;
            Health = health;
        }

        // Phương thức kiểm tra va chạm
        public bool CheckCollision(Rectangle playerRect)
        {
            Rectangle objectRect = new Rectangle(X, Y, Width, Height);
            return playerRect.IntersectsWith(objectRect);
        }

        // Phương thức nhận sát thương
        public void TakeDamage(int damage)
        {
            Health -= damage; // Giảm máu theo sát thương
            if (Health < 0) Health = 0; // Đảm bảo máu không âm
            Console.WriteLine($"{Name} bị tấn công! Máu còn lại: {Health}");
        }

        // Phương thức kiểm tra nếu đối tượng đã chết
        public bool IsDead()
        {
            return Health <= 0;
        }
    }
}
