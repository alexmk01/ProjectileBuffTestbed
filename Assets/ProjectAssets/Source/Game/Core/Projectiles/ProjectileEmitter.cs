using System;
using System.Collections.Generic;
using System.Numerics;
using Common;

namespace Game.Core.Projectiles
{
    public sealed class ProjectileEmitter : IDisposable
    {
        [Serializable]
        public struct Parameters
        {
            public static readonly Parameters Default = new()
            {
                ProjectileSpeed = 10f,
                ProjectileLifetime = 5f,
                ProjectileDamage = 10f,
                MaxProjectileHitCount = 1
            };
            
            public float ProjectileSpeed;
            public float ProjectileLifetime;
            public float ProjectileDamage;
            public int MaxProjectileHitCount;
        }

        private static int lastEmitterId = 0;
        
        private static int GetEmitterId() => ++lastEmitterId;

        public int Id { get; } = GetEmitterId();
        public Span<Projectile> Projectiles => projectiles.AsSpan();
        public IProjectilesRenderer ProjectilesRenderer => renderer;
        public IReadOnlyList<IProjectilesBehaviour> ProjectilesBehaviours => behaviours;

        public Parameters EmitterParameters = Parameters.Default;

        private List<Projectile> projectiles = new(64);
        private List<Projectile> newProjectiles = new(8);
        private IProjectilesBehaviour[] behaviours;
        private IProjectilesRenderer renderer;

        public ProjectileEmitter(ProjectileEmitterConfig config)
        {
            EmitterParameters = config.EmitterParameters;
            renderer = config.ProjectilesRenderer;
            renderer?.Initialize(this);
            behaviours = config.ProjectilesBehaviours ?? Array.Empty<IProjectilesBehaviour>();

            for (int i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].Initialize(this);
            }
        }
        
        public void EmitProjectile(in ProjectileEmissionArgs args)
        {
            Vector2 velocity = args.Direction * EmitterParameters.ProjectileSpeed;
            float maxLifetime = EmitterParameters.ProjectileLifetime;
            float damage = args.DamageOverride > 0f ? args.DamageOverride : EmitterParameters.ProjectileDamage;
            int seed = Environment.TickCount;
            
            var projectile = new Projectile(velocity, maxLifetime, damage, args.ProjectileGeneration, seed)
            {
                Position = args.Position,
                Velocity = velocity,
                RemainingLifetime = maxLifetime,
                Damage = damage,
                RemainingHitCount = Math.Max(EmitterParameters.MaxProjectileHitCount, 1)
            };

            newProjectiles.Add(projectile);
        }
        
        public void Update(float time, float deltaTime)
        {
            int i;
            
            if (newProjectiles.Count != 0)
            {
                int startIndex = this.projectiles.Count;
                int endIndex = startIndex + newProjectiles.Count - 1;
                this.projectiles.AddRange(newProjectiles);
                newProjectiles.Clear();

                for (i = 0; i < behaviours.Length; i++)
                {
                    behaviours[i].OnNewProjectilesLaunched(startIndex, endIndex, this.projectiles.AsSpan());
                }
            }
            
            if (this.projectiles.Count == 0) return;
            Span<Projectile> projectiles = this.projectiles.AsSpan();

            for (i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].ModifyProjectiles(projectiles, time, deltaTime);
            }

            for (i = 0; i < projectiles.Length; i++)
            {
                ref Projectile projectile = ref projectiles[i];
                projectile.Position += projectile.Velocity * deltaTime;
            }
            
            renderer?.RenderProjectiles(projectiles);
            i = 0;
            
            while (i < projectiles.Length)
            {
                ref Projectile projectile = ref projectiles[i];
                projectile.RemainingLifetime -= deltaTime;

                if (projectile.RemainingLifetime <= 0f || projectile.RemainingHitCount <= 0)
                {
                    for (int j = 0; j < behaviours.Length; j++)
                    {
                        IProjectilesBehaviour behaviour = behaviours[j];
                        behaviour.OnProjectileDestroyed(i, projectiles);
                        //Sync additional data etc
                        behaviour.OnProjectileIndexChanged(projectiles.Length - 1, i, projectiles);
                    }

                    this.projectiles.RemoveSwapBack(i);
                    projectiles = projectiles[..^1];
                    continue;
                }
                
                i++;
            }
            
            for (i = 0; i < projectiles.Length; i++)
            {
                //Prevent behaviours effect accumulation
                projectiles[i].ResetState();
            }
        }
        
        public void Dispose()
        {
            if (projectiles == null) return;
            projectiles.Clear();
            newProjectiles.Clear();
            projectiles = null;
            newProjectiles = null;
            (renderer as IDisposable)?.Dispose();
            renderer = null;

            foreach (IProjectilesBehaviour modifier in behaviours)
            {
                (modifier as IDisposable)?.Dispose();
            }

            behaviours = null;
        }
    }
}