using System.Numerics;
using Game.Core.Buildings;
using Game.Core.Construction;
using Game.Core.Construction.Services;
using Game.Core.Map;

namespace Game.Features.Construction.Services
{
    public class BuildingConstructionService : IBuildingConstructionService
    {
        private readonly IGameMap gameMap;
        private readonly IBuildingSpawner spawner;

        public BuildingConstructionService(IGameMap gameMap, IBuildingSpawner spawner)
        {
            this.gameMap = gameMap;
            this.spawner = spawner;
        }
        
        public bool IsBuildingConstructionAllowed(Vector2 position)
        {
            return gameMap.IsEntityPlacementAllowed(position);
        }

        public bool TryConstructBuilding(in BuildingId buildingId, Vector2 position)
        {
            if (IsBuildingConstructionAllowed(position))
            {
                position = gameMap.GetCellPosition(position);
                IBuilding building = spawner.SpawnBuilding(buildingId, position);

                if (building != null)
                {
                    gameMap.AddEntityToGrid(building);
                    return true;
                }
            }

            return false;
        }
        
        public void DestroyBuilding(IBuilding building)
        {
            gameMap.RemoveEntityFromGrid(building);
            building.Kill();
        }
    }
}