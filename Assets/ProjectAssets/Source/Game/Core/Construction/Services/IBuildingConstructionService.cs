using System.Numerics;
using Game.Core.Buildings;

namespace Game.Core.Construction.Services
{
    public interface IBuildingConstructionService
    {
        bool TryConstructBuilding(in BuildingId buildingId, Vector2 position);
        void DestroyBuilding(IBuilding building);
    }
}