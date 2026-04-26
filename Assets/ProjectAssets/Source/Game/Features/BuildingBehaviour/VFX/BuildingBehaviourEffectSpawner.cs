using System;
using Common.Unity;
using Game.Core.BuildingBehaviour.Messages;
using Game.Infrastructure;
using Game.Infrastructure.VFX;
using MessagePipe;
using VContainer.Unity;

namespace Game.Features.BuildingBehaviour.VFX
{
    public sealed class BuildingBehaviourEffectSpawner : ParticleSystemEffectsSpawnerBase, IInitializable, IDisposable
    {
        private readonly IDisposable disposables;

        public BuildingBehaviourEffectSpawner
        (
            EffectPrefabData[] effectsData,
            ISubscriber<BuildingBehaviourEffectAppliedMessage> behaviourEffectSubscriber
        ) 
            : base(effectsData)
        {
            var disposableBuilder = DisposableBag.CreateBuilder();
            
            behaviourEffectSubscriber
                .Subscribe(message => SpawnEffect(message.Building.Id.ToTag(), message.Building.Position.ToUnity()))
                .AddTo(disposableBuilder);
            
            disposables = disposableBuilder.Build();
        }

        public void Initialize()
        {
        }
        
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}