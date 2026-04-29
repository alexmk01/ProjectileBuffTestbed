using Common.Unity.Data;
using Game.Features.BuildingBehaviour;
using Game.Features.Buildings;
using Game.Features.Construction;
using Game.Features.HitPoints;
using Game.Features.Interaction;
using Game.Features.Map;
using Game.Features.Map.Loaders;
using Game.Features.Player;
using Game.Features.Projectiles;
using Game.Infrastructure;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameRunLifetimeScope : LifetimeScope
    {
        public GameDataLocationsReference BuildingsDataLocations;
        public GameObject GameMapPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var messagePipeOptions = builder.RegisterMessagePipe();
            messagePipeOptions.EnableCaptureStackTrace = true;
            builder.RegisterBuildCallback(resolver => GlobalMessagePipe.SetProvider(resolver.AsServiceProvider()));

            var buildingScopeInstallers = new IInstaller[]
            {
                new HitPointsEntityScopeFeatureInstaller(),
                new ProjectilesEntityScopeFeatureInstaller(),
                new BuildingBehaviourEntityScopeFeatureInstaller(),
            };
            
            new GameInfrastructureInstaller().Install(builder);
            new BuildingsFeatureInstaller(BuildingsDataLocations, buildingScopeInstallers).Install(builder);
            new HitPointsFeatureInstaller(messagePipeOptions).Install(builder);
            new ProjectilesFeatureInstaller(messagePipeOptions).Install(builder);
            new GameMapFeatureInstaller(new PrefabBasedGameMapLoader(GameMapPrefab)).Install(builder);
            new ConstructionFeatureInstaller(messagePipeOptions).Install(builder);
            new BuildingBehaviourFeatureInstaller(messagePipeOptions).Install(builder);
            new PlayerFeatureInstaller().Install(builder);
            new InteractionFeatureInstaller(messagePipeOptions).Install(builder);
        }
    }
}
