using System;

namespace Game.Core.Projectiles
{
    public interface IProjectilesRenderer : ICloneable
    {
        void Initialize(ProjectileEmitter emitter);
        void RenderProjectiles(ReadOnlySpan<Projectile> projectiles);
    }
}