using System.IO;

namespace Project_Game.Entities
{
    public class House : StaticObject
    {
        public House(string imageName, int x, int y, int width, int height)
            : base(Path.Combine("Assets", "House", imageName), x, y, width, height)
        {
        }
    }
}
