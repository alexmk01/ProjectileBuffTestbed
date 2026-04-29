using System;
using Game.Core.BuildingBehaviour;
using Game.Core.BuildingBehaviour.Messages;
using Game.Core.Buildings;
using Game.Core.Map;
using Game.Core.Projectiles.Events;
using MessagePipe;
using VContainer;

namespace Game.Features.BuildingBehaviour.Behaviours
{
    public sealed class ModifyProjectileHitCountBuildingBehaviour : BuildingBehaviourBase<ModifyProjectileHitCountBuildingBehaviour.Data>
    {
        [Serializable]
        public sealed class Data : IBuildingBehaviourData
        {
            public int HitCountModifier = 1;
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
                        int hitCountModifier = BehaviourData.HitCountModifier;
                        message.Emitter.Projectiles[message.ProjectileIndex].RemainingHitCount += BehaviourData.HitCountModifier;
                        effectAppliedMessagePublisher.Publish(new BuildingBehaviourEffectAppliedMessage(Building, this, message.Emitter, hitCountModifier));
                    }
                })
                .AddTo(disposableBuilder);
            disposables = disposableBuilder.Build();
        }

        public ModifyProjectileHitCountBuildingBehaviour(Data data, IBuilding building) : base(data, building)
        {
        }
        
        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}