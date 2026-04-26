using System;

namespace Game.Core.Projectiles
{
    [Serializable]
    public sealed class ProjectileEmitterConfig
    {
        public ProjectileEmitter.Parameters EmitterParameters;
        public IProjectilesRenderer ProjectilesRenderer;
        public IProjectileProcessor[] ProjectileProcessors;
    }
}