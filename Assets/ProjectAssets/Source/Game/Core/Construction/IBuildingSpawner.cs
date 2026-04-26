using Game.Core.Buildings;
using System.Numerics;

namespace Game.Core.Construction
{
    public interface IBuildingSpawner
    {
        IBuilding SpawnBuilding(in BuildingId buildingId, Vector2 position);
    }
}