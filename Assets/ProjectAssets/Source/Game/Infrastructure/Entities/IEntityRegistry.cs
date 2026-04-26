using System.Collections.Generic;
using Game.Core.Entities;

namespace Game.Infrastructure.Entities
{
    public interface IEntityRegistry
    {
        IReadOnlyList<IEntity> Entities { get; }
        IEntity GetEntity(int entityId);
        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);
    }
}