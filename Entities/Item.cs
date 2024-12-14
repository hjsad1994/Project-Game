using System.Drawing;

namespace Project_Game.Entities
{
    public class Item
    {
        public string Name { get; set; }
        public Image Icon { get; set; }

        public Item(string name, Image icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}
