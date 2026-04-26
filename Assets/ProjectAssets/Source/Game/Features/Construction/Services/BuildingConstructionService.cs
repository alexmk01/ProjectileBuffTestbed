using System.Numerics;
using Game.Core.Buildings;
using Game.Core.Construction;
using Game.Core.Construction.Services;
using Game.Core.Map.Services;
using Game.Infrastructure.Entities;

namespace Game.Features.Construction.Services
{
    public class BuildingConstructionService : IBuildingConstructionService
    {
        private readonly IEntityRegistry entityRegistry;
        private readonly IGameMapService mapService;
        private readonly IBuildingSpawner spawner;
        
        public BuildingConstructionService(IEntityRegistry entityRegistry, IGameMapService mapService, IBuildingSpawner spawner)
        {
            this.entityRegistry = entityRegistry;
            this.mapService = mapService;
            this.spawner = spawner;
        }

        public bool TryConstructBuilding(in BuildingId buildingId, Vector2 position)
        {
            if (mapService.IsEntityPlacementAllowed(position))
            {
                position = mapService.GetEntityPlacementPosition(position);
                IBuilding building = spawner.SpawnBuilding(buildingId, position);

                if (building != null)
                {
                    mapService.AddEntityToGrid(building);
                    return true;
                }
            }

            return false;
        }
        
        public void DestroyBuilding(IBuilding building)
        {
            mapService.RemoveEntityFromGrid(building);
            entityRegistry.RemoveEntity(building);
            building.Kill();
        }
    }
}