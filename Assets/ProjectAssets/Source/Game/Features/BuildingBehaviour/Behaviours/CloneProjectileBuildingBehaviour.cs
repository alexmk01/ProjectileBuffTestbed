using System;
using System.Numerics;
using Game.Core.BuildingBehaviour;
using Game.Core.BuildingBehaviour.Messages;
using Game.Core.Buildings;
using Game.Core.Map;
using Game.Core.Projectiles;
using Game.Core.Projectiles.Events;
using Game.Infrastructure;
using MessagePipe;
using VContainer;

namespace Game.Features.BuildingBehaviour.Behaviours
{
    public sealed class CloneProjectileBuildingBehaviour : BuildingBehaviourBase<CloneProjectileBuildingBehaviour.Data>, IDisposable
    {
        [Serializable]
        public sealed class Data : IBuildingBehaviourData
        {
            public float ClonedProjectileDamageMultiplier = 0.5f;
        }
        
        private IDisposable disposables;

        [Inject]
        private void Construct
        (
            IGameMap gameMap,
            ISubscriber<ProjectileCellChangedMessage> cellChangedMessageSubscriber,
            IPublisher<BuildingBehaviourEffectAppliedMessage> effectAppliedMessagePublisher
        )
        {
            var disposableBuilder = DisposableBag.CreateBuilder();

            cellChangedMessageSubscriber
                .Subscribe(message =>
                {
                    if (IsActive && message.CellIndex == gameMap.GetCellIndex(Building.Position))
                    {
                        ref readonly Projectile projectile = ref message.Emitter.Projectiles[message.ProjectileIndex];

                        if (projectile.Generation <= 1)
                        {
                            if (gameMap.TryGetEntityPlacementArea(Building, out GameMapArea currentArea))
                            {
                                var args = new ProjectileEmissionArgs
                                {
                                    Position = currentArea.Min + new Vector2(0f, currentArea.Size.Y * 0.5f),
                                    Direction = Vector2.Normalize(projectile.LaunchVelocity),
                                    DamageOverride = projectile.Damage * BehaviourData.ClonedProjectileDamageMultiplier,
                                    ProjectileGeneration = Math.Max(projectile.Generation, 1) + 1
                                };

                                message.Emitter.EmitProjectile(args);
                                effectAppliedMessagePublisher.Publish(new BuildingBehaviourEffectAppliedMessage(Building, this, message.Emitter, 0f));
                            }
                            else
                            {
                                Common.DebugUtils.LogError($"Failed to get entity placement area for building {Building.Id.ToTag()}");
                            }
                        }
                    }
                })
                .AddTo(disposableBuilder);
            disposables = disposableBuilder.Build();
        }

        public CloneProjectileBuildingBehaviour(Data data, IBuilding building) : base(data, building)
        {
        }
        
        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}