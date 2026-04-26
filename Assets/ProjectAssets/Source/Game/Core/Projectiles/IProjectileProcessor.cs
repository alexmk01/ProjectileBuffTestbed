using System;

namespace Game.Core.Projectiles
{
    public interface IProjectileProcessor : ICloneable
    {
        void Initialize(ProjectileEmitter emitter);
        void ModifyProjectiles(Span<Projectile> projectiles, float time, float deltaTime);
        void OnNewProjectilesLaunched(int startIndex, int endIndex, Span<Projectile> projectiles);
        void OnProjectileIndexChanged(int lastIndex, int newIndex, Span<Projectile> projectiles);
        void OnProjectileDestroyed(int projectileIndex, Span<Projectile> projectiles);
    }
}