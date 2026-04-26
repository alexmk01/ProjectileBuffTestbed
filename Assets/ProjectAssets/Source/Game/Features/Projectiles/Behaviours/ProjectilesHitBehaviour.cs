using System;
using Common.Unity;
using Game.Core.Entities;
using Game.Core.HitPoints.Commands;
using Game.Core.Projectiles;
using MessagePipe;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Game.Features.Projectiles.Behaviours
{
    [Serializable]
    public sealed class ProjectilesHitBehaviour : IProjectilesBehaviour
    {
        public LayerMask DamageableLayers = -1;
        
        private int[] lastHitEntityIds;
        private IPublisher<ChangeHitPointsCommand> changeHitPointsCommandPublisher;

        [Inject]
        private void Construct(IPublisher<ChangeHitPointsCommand> changeHitPointsCommandPublisher)
        {
            this.changeHitPointsCommandPublisher = changeHitPointsCommandPublisher;
        }
        
        public object Clone()
        {
            return new ProjectilesHitBehaviour
            {
                DamageableLayers = DamageableLayers
            };
        }

        public void Initialize(ProjectileEmitter emitter)
        {
            lastHitEntityIds = new int[64];
        }
        
        public void ModifyProjectiles(Span<Projectile> projectiles, float time, float deltaTime)
        {
            var contactFilter = new ContactFilter2D { useTriggers = false };
            contactFilter.SetLayerMask(DamageableLayers);
            
            using (ListPool<Collider2D>.Get(out var colliders))
            {
                for (int i = 0; i < projectiles.Length; i++)
                {
                    ref Projectile projectile = ref projectiles[i];
                    Vector2 position = projectile.Position.ToUnity();
                    colliders.Clear();
                    Physics2D.OverlapPoint(position, contactFilter, colliders);

                    for (int j = 0; j < colliders.Count; j++)
                    {
                        if (colliders[j].TryGetComponent(out IEntity entity) && lastHitEntityIds[i] != entity.InstanceId)
                        {
                            changeHitPointsCommandPublisher.Publish(new ChangeHitPointsCommand(entity.InstanceId, -projectile.Damage));
                            projectile.RemainingHitCount--;
                            lastHitEntityIds[i] = entity.InstanceId;
                            break;
                        }
                    }
                }
            }
        }
        
        public void OnNewProjectilesLaunched(int startIndex, int endIndex, Span<Projectile> projectiles)
        {
            if (projectiles.Length > lastHitEntityIds.Length)
            {
                Array.Resize(ref lastHitEntityIds, (int)(projectiles.Length * 1.5f));
            }
            
            for (int i = startIndex; i <= endIndex; i++)
            {
                lastHitEntityIds[i] = 0;
            }
        }

        public void OnProjectileIndexChanged(int lastIndex, int newIndex, Span<Projectile> projectiles)
        {
            lastHitEntityIds[newIndex] = lastHitEntityIds[lastIndex];
        }

        public void OnProjectileDestroyed(int projectileIndex, Span<Projectile> projectiles)
        {
        }
    }
}
    