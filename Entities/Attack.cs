using System;
using Project_Game.Entities;

public class Attack
{
    public int Damage { get; private set; } // Sát thương
    public float Range { get; private set; } // Phạm vi
    public string Effect { get; private set; } // Hiệu ứng tấn công

    public Attack(int damage, float range, string effect = null)
    {
        Damage = damage;
        Range = range;
        Effect = effect;
    }

    // Thực hiện tấn công
    public void PerformAttack(Player player, TestEnemy target)
    {
        if (target != null)
        {
            target.TakeDamage(Damage);
            Console.WriteLine($"Tấn công {target.Name} với {Damage} sát thương!");
        }
    }


    // Kiểm tra phạm vi
    private bool IsTargetInRange(Player player, GameObject target)
    {
        double distance = Math.Sqrt(
            Math.Pow(player.playerX - target.X, 2) +
            Math.Pow(player.playerY - target.Y, 2)
        );
        return distance <= Range;
    }
}
