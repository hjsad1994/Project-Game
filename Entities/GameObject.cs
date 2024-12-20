//using System;
//using System.Drawing;

//namespace Project_Game.Entities
//{
//    public class GameObject
//    {
//        public virtual int X { get; set; }
//        public virtual int Y { get; set; }
//        public int Width { get; set; }
//        public int Height { get; set; }
//        public string Name { get; set; }
//        public int Health { get; private set; }

//        public GameObject(int x, int y, int width, int height, string name, int health = 100)
//        {
//            X = x;
//            Y = y;
//            Width = width;
//            Height = height;
//            Name = name;
//            Health = health;
//        }

//        public bool CheckCollision(Rectangle rect)
//        {
//            Rectangle objectRect = new Rectangle(X, Y, Width, Height);
//            return rect.IntersectsWith(objectRect);
//        }

//        public virtual void TakeDamage(int damage)
//        {
//            Health -= damage;
//            if (Health < 0) Health = 0;
//            Console.WriteLine($"{Name} bị tấn công! Máu còn lại: {Health}");
//        }

//        public bool IsDead() => Health <= 0;
//    }
//}
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Project_Game.Entities
{
    public class GameObject
    {
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; } // Thêm thuộc tính MaxHealth
        public bool ShouldRemove { get; set; } = false;

        public GameObject(int x, int y, int width, int height, string name, int health = 100)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = name;
            Health = health;
            MaxHealth = health; // Khởi tạo MaxHealth với giá trị ban đầu của Health
        }

        public bool CheckCollision(Rectangle rect)
        {
            Rectangle objectRect = new Rectangle(X, Y, Width, Height);
            return rect.IntersectsWith(objectRect);
        }
        public virtual void Update()

        {
            // Logic cập nhật mặc định (nếu có)
        }
        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
            Console.WriteLine($"{Name} bị tấn công! Máu còn lại: {Health}/{MaxHealth}");
        }

        public bool IsDead() => Health <= 0;
        public virtual void Draw(Graphics g)
        {
        }
    }
}
