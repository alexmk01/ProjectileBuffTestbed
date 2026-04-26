using System;
using Common.Unity;
using Game.Core.Projectiles;
using UnityEngine;

namespace Game.Features.Projectiles.Renderers
{
    [Serializable]
    public sealed class ParticleSystemProjectilesRenderer : IProjectilesRenderer, IDisposable
    {
        public GameObject ParticleSystemPrefab;
        
        private ParticleSystem particleSystem;
        private ParticleSystem.Particle[] particles;
        private int muzzleFlashSubEmitterIndex;
        private int hitEffectSubEmitterIndex;
        
        public void Initialize(ProjectileEmitter emitter)
        {
            void PrepareSubEmitter(ParticleSystemSubEmitterType targetEmitterType, ref int index)
            {
                index = -1;
                ParticleSystem.SubEmittersModule subEmitters = particleSystem.subEmitters;
                
                for (int i = 0; i < subEmitters.subEmittersCount; i++)
                {
                    ParticleSystemSubEmitterType emitterType = subEmitters.GetSubEmitterType(i);
                    
                    if (emitterType == targetEmitterType)
                    {
                        subEmitters.SetSubEmitterType(i, ParticleSystemSubEmitterType.Manual);
                        ParticleSystem subEmitter = subEmitters.GetSubEmitterSystem(i);
                        //TODO: use view bounds to suppress particle system culling
                        subEmitter.GetComponent<Renderer>().bounds = new Bounds(new Vector3(), new Vector3(1000000f, 10000f, 1000000f));
                        index = i;
                        break;
                    }
                }
            }

            particleSystem = UnityEngine.Object.Instantiate(ParticleSystemPrefab).GetComponent<ParticleSystem>();
            particleSystem.Stop();
            particleSystem.transform.position = new Vector3();
            ParticleSystem.MainModule main = particleSystem.main;
            main.playOnAwake = false;
            main.loop = false;
            PrepareSubEmitter(ParticleSystemSubEmitterType.Birth, ref muzzleFlashSubEmitterIndex);
            PrepareSubEmitter(ParticleSystemSubEmitterType.Death, ref hitEffectSubEmitterIndex);
        }
        
        public void RenderProjectiles(ReadOnlySpan<Projectile> projectiles)
        {
            if (particles == null)
            {
                particles = new ParticleSystem.Particle[Mathf.Max(projectiles.Length, 32)];
                particleSystem.Stop();
                ParticleSystem.MainModule mainModule = particleSystem.main;
                mainModule.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
                mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
                mainModule.stopAction = ParticleSystemStopAction.None;
            }
            
            int projectilesCount = projectiles.Length;
            int particlesCountDifference = projectilesCount - particleSystem.particleCount;

            if (particlesCountDifference > 0)
            {
                particleSystem.Emit(particlesCountDifference);
            }
            
            if (particles.Length < projectilesCount)
            {
                Array.Resize(ref particles, (int)(projectilesCount * 1.5f));
            }
            
            particleSystem.GetParticles(particles);

            void TriggerSubEmitter(ref ParticleSystem.Particle particle, int subEmitterIndex)
            {
                if (subEmitterIndex >= 0)
                {
                    particleSystem.TriggerSubEmitter(subEmitterIndex, ref particle);
                }
            }
            
            for (int i = 0; i < projectilesCount; i++)
            {
                ref readonly Projectile projectile = ref projectiles[i];
                ref ParticleSystem.Particle particle = ref particles[i];
                particle.position = projectile.Position.ToVector3();
                particle.velocity = projectile.Velocity.ToVector3();
                particle.startLifetime = projectile.StartLifetime;
                
                if (projectile.IsExpired())
                {
                    particle.remainingLifetime = 0f;
                    TriggerSubEmitter(ref particle, hitEffectSubEmitterIndex);
                }
                else
                {
                    particle.remainingLifetime = projectile.RemainingLifetime;
                    if (projectile.Generation <= 1 && projectile.IsNew()) TriggerSubEmitter(ref particle, muzzleFlashSubEmitterIndex);
                    if (projectile.IsHitCountDirty) TriggerSubEmitter(ref particle, hitEffectSubEmitterIndex);
                }
            }
            
            particleSystem.SetParticles(particles, projectilesCount);
        }

        public ParticleSystemProjectilesRenderer Clone()
        {
            return new ParticleSystemProjectilesRenderer
            {
                ParticleSystemPrefab = ParticleSystemPrefab
            };
        }

        object ICloneable.Clone() => Clone();

        public void Dispose()
        {
            if (particleSystem != null)
            {
                UnityEngine.Object.Destroy(particleSystem.gameObject);
                particleSystem = null;
            }
        }
    }
}