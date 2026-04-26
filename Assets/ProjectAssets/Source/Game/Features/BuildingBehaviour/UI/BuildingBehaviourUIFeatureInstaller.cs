using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.BuildingBehaviour.UI
{
    public sealed class BuildingBehaviourUIFeatureInstaller : IInstaller
    {
        private readonly GameObject buildingBehaviourScreenViewPrefab;
        private readonly Transform canvasRoot;
        
        public BuildingBehaviourUIFeatureInstaller(GameObject buildingBehaviourScreenViewPrefab, Transform canvasRoot)
        {
            this.buildingBehaviourScreenViewPrefab = buildingBehaviourScreenViewPrefab;
            this.canvasRoot = canvasRoot;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(buildingBehaviourScreenViewPrefab.GetComponent<BuildingBehaviourScreenView>(), Lifetime.Singleton)
                .UnderTransform(canvasRoot)
                .AsImplementedInterfaces();
            builder.RegisterEntryPoint<BuildingBehaviourScreenPresenter>();
        }
    }
}