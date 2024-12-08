using System;
using Project_Game.Entities;

public class Attack
{
    public int Damage { get; set; } // Adjustable damage for this attack
    public float Range { get; set; } // Adjustable range for this attack
    public string Effect { get; set; } // Optional effect for the attack

    public Attack(int damage, float range, string effect = null)
    {
        Damage = damage;
        Range = range;
        Effect = effect;
    }

    // Perform an attack on a target
    public bool PerformAttack(GameObject attacker, GameObject target)
    {
        if (target != null && IsTargetInRange(attacker, target))
        {
            target.TakeDamage(Damage);

            Console.WriteLine($"{attacker.Name} attacked {target.Name}, dealing {Damage} damage!");

            if (Effect != null)
            {
                ApplyEffect(target);
            }

            if (target.IsDead())
            {
                Console.WriteLine($"{target.Name} has been defeated!");
            }

            return true; // Attack successful
        }
        else
        {
            Console.WriteLine($"{attacker.Name} missed! Target is out of range.");
            return false; // Attack failed
        }
    }

    // Check if the target is within attack range
    private bool IsTargetInRange(GameObject attacker, GameObject target)
    {
        double distance = Math.Sqrt(
            Math.Pow(attacker.X - target.X, 2) +
            Math.Pow(attacker.Y - target.Y, 2)
        );
        return distance <= Range;
    }

    // Apply special effects to the target
    private void ApplyEffect(GameObject target)
    {
        switch (Effect)
        {
            case "Burn":
                Console.WriteLine($"{target.Name} is burning! Additional damage over time.");
                // Add burn logic here
                break;

            case "Stun":
                Console.WriteLine($"{target.Name} is stunned! Temporarily unable to move.");
                // Add stun logic here
                break;

            case "Heal":
                Console.WriteLine($"{target.Name} is healed!");
                target.TakeDamage(-Damage); // Negative damage to heal
                break;

            default:
                Console.WriteLine($"Unknown effect: {Effect}");
                break;
        }
    }
}
