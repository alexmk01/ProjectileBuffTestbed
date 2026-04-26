using System;
using System.Collections.Generic;
using Common;
using Game.Core.Entities;
using Game.Core.Entities.Messages;
using MessagePipe;
using UnityEngine.Assertions;

namespace Game.Infrastructure.Entities
{
    public sealed class EntityRegistry : IEntityRegistry, IDisposable
    {
        public IReadOnlyList<IEntity> Entities => entities;

        private readonly List<IEntity> entities = new(64);
        private readonly Dictionary<int, IEntity> entitiesById = new(64);
        private readonly IDisposable disposables;
        
        private void RemoveEntityFromList(IEntity entity)
        {
            int entityIndex = entities.IndexOf(entity);
            if (entityIndex >= 0) entities.RemoveSwapBack(entityIndex);
        }
        
        public EntityRegistry(ISubscriber<EntityKilledMessage> killedMessageSubscriber)
        {
            var disposablesBuilder = DisposableBag.CreateBuilder();
            killedMessageSubscriber.Subscribe(message =>
            {
                if (entitiesById.TryGetValue(message.Entity.InstanceId, out IEntity entity))
                {
                    RemoveEntityFromList(entity);
                }
            })
            .AddTo(disposablesBuilder);
            disposables = disposablesBuilder.Build();
        }
        
        public IEntity GetEntity(int entityId)
        {
            return entitiesById.TryGetValue(entityId, out var entity) ? entity : null;
        }

        public void AddEntity(IEntity entity)
        {
            Assert.AreNotEqual(entity.InstanceId, 0, "Entity id can't be zero");
            entities.Add(entity);
            entitiesById.Add(entity.InstanceId, entity);
        }
        
        public void RemoveEntity(IEntity entity)
        {
            RemoveEntityFromList(entity);
            entitiesById.Remove(entity.InstanceId);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}