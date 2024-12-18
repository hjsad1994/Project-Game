using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Project_Game.Entities
{
    public class TreeStage
    {
        public Image Image { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TreeStage(Image image, int width, int height)
        {
            Image = image;
            Width = width;
            Height = height;
        }
    }
}
