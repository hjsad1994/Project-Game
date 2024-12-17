using System.Collections.Generic;

namespace Project_Game.Entities
{
    public abstract class Weapon
    {
        public string Name { get; protected set; }
        public string WeaponType { get; protected set; }

        public Weapon(string name, string weaponType)
        {
            Name = name;
            WeaponType = weaponType;
        }

        // Phương thức trừu tượng để thực hiện hành động tấn công
        public abstract void Attack(Enemy target);

        // Phương thức trừu tượng để thực hiện hành động đặc biệt (như chặt cây)
        public virtual void SpecialAction(Player player, List<Tree> trees)
        {
            // Mặc định không làm gì, các lớp kế thừa có thể override nếu cần
        }
    }
}
