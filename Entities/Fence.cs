using System.IO;

namespace Project_Game.Entities
{
    public class Fence : StaticObject
    {
        public Fence(string imageName, int x, int y, int width, int height)
            : base(Path.Combine("Assets", "Fence", imageName), x, y, width, height)
        {
        }
    }
}
