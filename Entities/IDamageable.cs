// File: IDamageable.cs
namespace Project_Game.Entities
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        bool IsDead { get; }
    }
}
