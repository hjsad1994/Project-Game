using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Game.Entities
{
    public class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } 

        public GameObject(int x, int y, int width, int height, string name)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = name;
        }
        public bool CheckCollision(Rectangle playerRect)
        {
            Rectangle objectRect = new Rectangle(X, Y, Width, Height);
            return playerRect.IntersectsWith(objectRect);
        }
    }
}
