using System.Drawing;

namespace Project_Game.Entities
{
    public class DroppedItem : GameObject
    {
        public Item Item { get; private set; }

        public DroppedItem(Item item, int x, int y, int width = 32, int height = 32)
            : base(x, y, width, height, "DroppedItem", 1) // Health không quan trọng cho DroppedItem
        {
            Item = item;
        }

        public override void TakeDamage(int damage)
        {
            // DroppedItem không nhận sát thương, có thể để trống hoặc thêm logic nếu cần
        }

        public Image GetCurrentFrame()
        {
            return Item.Icon;
        }

        public void Update()
        {
            // Nếu muốn thêm hiệu ứng chuyển động cho item rơi ra, thêm logic ở đây
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(GetCurrentFrame(), X, Y, Width, Height);
        }
    }
}
