using System.Numerics;
using Game.Core.Buildings;
using Game.Core.Buildings.Factory;
using Game.Core.Construction;
using Game.Infrastructure.Entities;

namespace Game.Features.Construction
{
    public sealed class BuildingSpawner : IBuildingSpawner
    {
        private readonly IEntityRegistry entityRegistry;
        private readonly IBuildingFactory buildingFactory;

        public BuildingSpawner(IEntityRegistry entityRegistry, IBuildingFactory buildingFactory)
        {
            this.entityRegistry = entityRegistry;
            this.buildingFactory = buildingFactory;
        }
        
        public IBuilding SpawnBuilding(in BuildingId buildingId, Vector2 position)
        {
            if (buildingFactory.CreateBuilding(new BuildingFactoryArgs(buildingId, position)) is { } building)
            {
                entityRegistry.AddEntity(building);
                return building;
            }

            return null;
        }
    }
}