using System;
using Common;
using Common.Tags;
using Common.Unity;
using Game.Core.Entities;
using Game.Core.HitPoints.Events;
using Game.Infrastructure.VFX;
using MessagePipe;
using VContainer.Unity;

namespace Game.Features.HitPoints.VFX
{
    public sealed class EntityDesctructionEffectsSpawner : ParticleSystemEffectsSpawnerBase, IInitializable, IDisposable
    {
        private readonly IDisposable disposables;

        public EntityDesctructionEffectsSpawner(EffectPrefabData[] effectsData, ISubscriber<HitPointsExhaustedMessage> destructionSubscriber) : base(effectsData)
        {
            var disposableBuilder = DisposableBag.CreateBuilder();

            destructionSubscriber
                .Subscribe(message =>
                {
                    IEntity entity = message.Entity;
                    var entityId = (entity as IIdentifiable<Tag>)?.Id ?? Tag.Empty;
                    SpawnEffect(entityId, entity.Position.ToUnity());
                })
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