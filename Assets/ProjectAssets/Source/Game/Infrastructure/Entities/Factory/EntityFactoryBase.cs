using System;
using System.Collections.Generic;
using Common;
using Common.Tags;
using Common.Unity.Tags.Hierarchical;
using Game.Core.Entities;
using Game.Core.HitPoints;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Game.Infrastructure.Entities.Factory
{
    public abstract class EntityFactoryBase<TDescription, TEntity> 
        where TDescription : EntityDescriptionComponent
        where TEntity : EntityMainComponent
    {
        private readonly LifetimeScope parentScope;
        private readonly IInstaller[] entityScopeInstallers;
        private readonly Dictionary<Tag, TDescription> entitiesData = new(16);
        
        protected abstract int GetEntityLayer(TDescription data);

        protected EntityFactoryBase(LifetimeScope parentScope, IInstaller[] entityScopeInstallers, IReadOnlyList<TDescription> entitiesData)
        {
            this.parentScope = parentScope;
            this.entityScopeInstallers = entityScopeInstallers ?? Array.Empty<IInstaller>();

            for (int i = 0; i < entitiesData.Count; i++)
            {
                TDescription data = entitiesData[i];
                if (!data.gameObject.activeSelf) continue;
                Tag entityId = data.Id.ToTag();
                if (!entityId.HasValue()) continue;
                this.entitiesData.Add(entityId, data);
            }
        }
        
        protected TEntity CreateEntity(TDescription data, GameObject entityObject)
        {
            entityObject.layer = GetEntityLayer(data);
            var entity = entityObject.AddComponent<TEntity>();
            entity.InstanceId = entityObject.GetInstanceID();
            entity.HitPointsState = data.MaxHitPoints > 0f ? new HitPointsState(data.MaxHitPoints) : new HitPointsState(0f);
            OnEntityCreated(data, entity);
            
            using (LifetimeScope.EnqueueParent(parentScope))
            {
                var entityScope = entityObject.AddComponent<EntityLifetimeScope>();
                entityScope.EntityScopeInstallers = entityScopeInstallers;
                entityScope.Build();
            }
            
            Object.Destroy(entityObject.GetComponent<TDescription>());
            return entity;
        }
        
        protected TEntity CreateEntity(in Tag entityId, System.Numerics.Vector2 position)
        {
            if (entitiesData.TryGetValue(entityId, out TDescription data))
            {
                Assert.AreEqual(entityId, data.Id.ToTag(), entityId.ToString());
                GameObject entityObject = Object.Instantiate(data.gameObject);
                var entity = CreateEntity(data, entityObject);
                if (entity != null) ((IEntity)entity).Position = position;
                return entity;
            }
            
            DebugUtils.LogError($"Failed to create entity {entityId}");
            return null;
        }

        protected TDescription GetEntityPrototypeData(in Tag entityId)
        {
            return entitiesData.TryGetValue(entityId, out TDescription data) ? data : null;
        }

        protected virtual void OnEntityCreated(TDescription data, TEntity entity) { }
    }
}