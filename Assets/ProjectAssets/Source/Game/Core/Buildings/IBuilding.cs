using Game.Core.Entities;

namespace Game.Core.Buildings
{
    public interface IBuilding : IEntity
    {
        BuildingId Id { get; }
    }
}