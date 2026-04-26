using System.Collections.Generic;
using Common;
using Common.Unity;
using Game.Core.Buildings;
using Game.Features.Buildings.Factory;
using Game.Infrastructure;

namespace Game.Features.Buildings
{
    public sealed class BuildingRepository : IBuildingRepository
    {
        public IReadOnlyList<IIdentifiable<BuildingId>> BuildingsData => buildingsData;

        private readonly BuildingDescriptionComponent[] buildingsData;
        
        public BuildingRepository(BuildingDescriptionComponent[] buildingsData)
        {
            this.buildingsData = buildingsData;
        }
        
        public bool IsPlayerBuildingData(IIdentifiable<BuildingId> buildingData)
        {
            return !((BuildingDescriptionComponent)buildingData).IsEnemyBuilding;
        }
        
        public bool IsPlayerBuilding(IBuilding building)
        {
            SerializableGUID buildingId = building.Id.ToGuid();
            
            for (int i = 0; i < buildingsData.Length; i++)
            {
                BuildingDescriptionComponent data = buildingsData[i];

                if (!data.IsEnemyBuilding && data.Id == buildingId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}