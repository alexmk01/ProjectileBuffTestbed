using System;
using Game.Core.Projectiles;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Game.Features.Projectiles.Processors
{
    [Serializable]
    public sealed class ProjectileSinusoidalTrajectoryProcessor : IProjectileProcessor
    {
        public float Amplitude = 15f;
        public float Frequency = 10f;

        public object Clone()
        {
            return new ProjectileSinusoidalTrajectoryProcessor
            {
                Amplitude = Amplitude,
                Frequency = Frequency
            };
        }

        public void Initialize(ProjectileEmitter emitter)
        {
        }
        
        public void ModifyProjectiles(Span<Projectile> projectiles, float time, float deltaTime)
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                ref Projectile projectile = ref projectiles[i];
                Vector2 velocity = projectile.Velocity;
                float angle = Mathf.Sin(time * Frequency) * Amplitude * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                projectile.Velocity = new Vector2(velocity.X * cos - velocity.Y * sin, velocity.X * sin + velocity.Y * cos);
            }
        }
        
        public void OnNewProjectilesLaunched(int startIndex, int endIndex, Span<Projectile> projectiles)
        {
        }

        public void OnProjectileDestroyed(int projectileIndex, Span<Projectile> projectiles)
        {
        }
        
        public void OnProjectileIndexChanged(int lastIndex, int newIndex, Span<Projectile> projectiles)
        {
        }
    }
}