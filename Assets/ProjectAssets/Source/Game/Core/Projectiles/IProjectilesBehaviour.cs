using System;

namespace Game.Core.Projectiles
{
    public interface IProjectilesBehaviour : ICloneable
    {
        void Initialize(ProjectileEmitter emitter);
        void OnNewProjectilesLaunched(int startIndex, int endIndex, Span<Projectile> projectiles);
        void OnProjectileIndexChanged(int lastIndex, int newIndex, Span<Projectile> projectiles);
        void OnProjectileDestroyed(int projectileIndex, Span<Projectile> projectiles);
        void ModifyProjectiles(Span<Projectile> projectiles, float time, float deltaTime);
    }
}