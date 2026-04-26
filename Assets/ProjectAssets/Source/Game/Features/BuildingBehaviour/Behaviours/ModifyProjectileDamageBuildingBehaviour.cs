using System;
using Game.Core.BuildingBehaviour.Messages;
using Game.Core.Buildings;
using Game.Core.Map.Services;
using Game.Core.Projectiles;
using Game.Core.Projectiles.Events;
using MessagePipe;
using VContainer;

namespace Game.Features.BuildingBehaviour.Behaviours
{
    public sealed class ModifyProjectileDamageBuildingBehaviour : BuildingBehaviourBase<ModifyProjectileDamageBuildingBehaviour.Data>, IDisposable
    {
        [Serializable]
        public sealed class Data : IBuildingBehaviourData
        {
            public float BaseDamageMultiplier = 0.5f;
        }
        
        private IDisposable disposables;

        [Inject]
        private void Construct
        (
            IGameMapService mapService,
            ISubscriber<ProjectileCellChangedMessage> cellChangedMessageSubscriber,
            IPublisher<BuildingBehaviourEffectAppliedMessage> effectAppliedMessagePublisher
        )
        {
            var disposableBuilder = DisposableBag.CreateBuilder();

            cellChangedMessageSubscriber
                .Subscribe(message =>
                {
                    if (IsActive && message.CellIndex == mapService.GetCellIndex(Building.Position))
                    {
                        ref Projectile projectile = ref message.Emitter.Projectiles[message.ProjectileIndex];
                        float damageAddition = projectile.StartDamage * BehaviourData.BaseDamageMultiplier;
                        projectile.Damage += damageAddition;
                        effectAppliedMessagePublisher.Publish(new BuildingBehaviourEffectAppliedMessage(Building, this, message.Emitter, damageAddition));
                    }
                })
                .AddTo(disposableBuilder);
            disposables = disposableBuilder.Build();
        }
        
        public ModifyProjectileDamageBuildingBehaviour(Data data, IBuilding building) : base(data, building)
        {
        }
        
        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}