using System.Collections.Generic;
using Common;

namespace Game.Core.Buildings
{
    public interface IBuildingRepository
    {
        IReadOnlyList<IIdentifiable<BuildingId>> BuildingsData { get; }
        
        bool IsPlayerBuildingData(IIdentifiable<BuildingId> buildingData);
        bool IsPlayerBuilding(IBuilding building);
    }
}