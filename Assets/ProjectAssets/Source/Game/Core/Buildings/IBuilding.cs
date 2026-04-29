using Game.Core.Entities;
using Game.Core.LifeCycle;

namespace Game.Core.Buildings
{
    public interface IBuilding : IEntity, IKillable
    {
        BuildingId Id { get; }
    }
}