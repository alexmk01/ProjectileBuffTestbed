using Game.Features.Buildings.Factory;
using Game.Core.Buildings.Factory;
using VContainer;
using VContainer.Unity;
using Common.Unity.Data;
using System.Linq;

namespace Game.Features.Buildings
{
    public sealed class BuildingsFeatureInstaller : IInstaller
    {
        private readonly GameDataLocationsReference buildingsDataLocations;
        private readonly IInstaller[] buildingScopeInstallers;

        public BuildingsFeatureInstaller(GameDataLocationsReference buildingsDataLocations, IInstaller[] buildingScopeInstallers)
        {
            this.buildingsDataLocations = buildingsDataLocations;
            this.buildingScopeInstallers = buildingScopeInstallers;
        }
        
        public void Install(IContainerBuilder builder)
        {
            var parentScope = (LifetimeScope)builder.ApplicationOrigin;
            var buildingsData = buildingsDataLocations.LoadComponents<BuildingDescriptionComponent>().ToArray();
            builder.Register<BuildingRepository>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(buildingsData);
            builder.Register<BuildingFactory>(Lifetime.Singleton).As<IBuildingFactory>()
                .WithParameter(parentScope)
                .WithParameter(buildingScopeInstallers)
                .WithParameter(buildingsData);
        }
    }
}