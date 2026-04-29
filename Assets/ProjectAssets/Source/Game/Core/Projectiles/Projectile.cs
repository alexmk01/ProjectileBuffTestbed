using System.Numerics;

namespace Game.Core.Projectiles
{
    public struct Projectile
    {
        public int RemainingHitCount
        {
            readonly get => remainingHitCount;
            set
            {
                if (remainingHitCount == value) return;
                IsHitCountDirty = value < remainingHitCount;
                remainingHitCount = value;
            }
        }
        
        public readonly float StartLifetime;
        public readonly float StartDamage;
        public readonly int Generation;
        public readonly int Seed;
        public Vector2 Position;
        public Vector2 LaunchVelocity;
        public Vector2 Velocity;
        public float Damage;
        public float RemainingLifetime;
        public bool IsHitCountDirty;
        
        private int remainingHitCount;
        
        internal void ResetState()
        {
            Velocity = LaunchVelocity;
            IsHitCountDirty = false;
        }
        
        public Projectile(Vector2 startVelocity, float startLifetime, float startDamage, int generation, int seed) : this()
        {
            LaunchVelocity = startVelocity;
            StartLifetime = startLifetime;
            StartDamage = startDamage;
            Generation = generation;
            Seed = seed;
        }
        
        public readonly bool IsNew() => RemainingLifetime == StartLifetime;
        public readonly bool IsExpired() => RemainingLifetime <= 0f || RemainingHitCount <= 0;
        public readonly float GetLifetime() => StartLifetime - RemainingLifetime;
        
        public void Destroy()
        {
            RemainingLifetime = 0f;
            RemainingHitCount = 0;
        }
    }
}