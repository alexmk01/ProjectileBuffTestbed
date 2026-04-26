using System;
using Game.Core.Projectiles;

namespace Game.Features.Projectiles.Processors
{
    public sealed class ProjectileAdditionalDataSyncProcessor<T> : IProjectileProcessor
    {
        private readonly T defaultValue;
        private T[] data;
        
        public ProjectileAdditionalDataSyncProcessor(T[] data, T defaultValue = default)
        {
            this.defaultValue = defaultValue;
            this.data = data;
        }
        
        public object Clone()
        {
            return new ProjectileAdditionalDataSyncProcessor<T>(data, defaultValue);
        }
        
        public void Initialize(ProjectileEmitter emitter)
        {
            data.AsSpan().Fill(defaultValue);
        }

        public void ModifyProjectiles(Span<Projectile> projectiles, float time, float deltaTime)
        {
        }
        
        public void OnNewProjectilesLaunched(int startIndex, int endIndex, Span<Projectile> projectiles)
        {
            if (projectiles.Length > data.Length)
            {
                Array.Resize(ref data, (int)(projectiles.Length * 1.5f));
            }
            
            for (int i = startIndex; i <= endIndex; i++)
            {
                data[i] = defaultValue;
            }
        }

        public void OnProjectileIndexChanged(int lastIndex, int newIndex, Span<Projectile> projectiles)
        {
            data[newIndex] = data[lastIndex];
        }
        
        public void OnProjectileDestroyed(int projectileIndex, Span<Projectile> projectiles)
        {
        }
    }
}