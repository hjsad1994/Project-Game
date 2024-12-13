using System;
using Project_Game.Entities;

namespace Project_Game.Entities
{
    public class Attack
    {
        public int Damage { get; set; }
        public float Range { get; set; }
        public string Effect { get; set; }

        public Attack(int damage, float range, string effect = null)
        {
            Damage = damage;
            Range = range;
            Effect = effect;
        }

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

                return true;
            }
            else
            {
                Console.WriteLine($"{attacker.Name} missed! Target is out of range.");
                return false;
            }
        }

        private bool IsTargetInRange(GameObject attacker, GameObject target)
        {
            double distance = Math.Sqrt(
                Math.Pow(attacker.X - target.X, 2) +
                Math.Pow(attacker.Y - target.Y, 2)
            );
            return distance <= Range;
        }

        private void ApplyEffect(GameObject target)
        {
            switch (Effect)
            {
                case "Burn":
                    Console.WriteLine($"{target.Name} is burning!");
                    break;

                case "Stun":
                    Console.WriteLine($"{target.Name} is stunned!");
                    break;

                case "Heal":
                    Console.WriteLine($"{target.Name} is healed!");
                    target.TakeDamage(-Damage);
                    break;

                default:
                    Console.WriteLine($"Unknown effect: {Effect}");
                    break;
            }
        }
    }
}
