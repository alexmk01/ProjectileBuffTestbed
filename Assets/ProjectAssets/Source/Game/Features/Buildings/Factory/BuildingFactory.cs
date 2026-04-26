using Game.Infrastructure.Entities.Factory;
using Game.Core.Buildings;
using Game.Core.Buildings.Factory;
using Game.Core.Data;
using VContainer.Unity;
using Game.Infrastructure;
using Common.Unity.Tags.Hierarchical;

namespace Game.Features.Buildings.Factory
{
    public sealed class BuildingFactory : EntityFactoryBase<BuildingDescriptionComponent, BuildingComponent>, IBuildingFactory
    {
        public int PlayerBuildingsLayer = 1;
        public int EnemyBuildingsLayer = 2;

        protected override int GetEntityLayer(BuildingDescriptionComponent data)
        {
            return data.IsEnemyBuilding ? EnemyBuildingsLayer : PlayerBuildingsLayer;
        }

        protected override void OnEntityCreated(BuildingDescriptionComponent description, BuildingComponent building)
        {
            building.Id = description.Id.ToBuildingId();
        }
        
        public BuildingFactory(GameConfig gameConfig, LifetimeScope parentScope, IInstaller[] entityScopeInstallers, BuildingDescriptionComponent[] entitiesData) : 
            base(parentScope, entityScopeInstallers, entitiesData)
        {
            PlayerBuildingsLayer = gameConfig.PlayerBuildingsLayer;
            EnemyBuildingsLayer = gameConfig.EnemyBuildingsLayer;
        }
        
        public IBuilding CreateBuilding(in BuildingFactoryArgs args) => CreateEntity(args.BuildingId.ToTag(), args.Position);
    }
}