using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Construction.UI
{
    public sealed class BuildingConstructionUIFeatureInstaller : IInstaller
    {
        public readonly GameObject ConstructionModeViewPrefab;
        public readonly GameObject ConstructionPanelViewPrefab;
        public readonly Transform CanvasRoot;

        public BuildingConstructionUIFeatureInstaller
        (
            GameObject constructionModeViewPrefab,
            GameObject constructionPanelViewPrefab,
            Transform canvasRoot
        )
        {
            ConstructionModeViewPrefab = constructionModeViewPrefab;
            ConstructionPanelViewPrefab = constructionPanelViewPrefab;
            CanvasRoot = canvasRoot;
        }
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(ConstructionModeViewPrefab.GetComponent<BuildingConstructionModeView>(), Lifetime.Singleton)
                .UnderTransform(CanvasRoot)
                .AsImplementedInterfaces();
            builder.RegisterComponentInNewPrefab(ConstructionPanelViewPrefab.GetComponent<BuildingConstructionPanelView>(), Lifetime.Singleton)
                .UnderTransform(CanvasRoot)
                .AsImplementedInterfaces();
            builder.RegisterEntryPoint<BuildingConstructionModePresenter>();
            builder.RegisterEntryPoint<BuildingConstructionPanelPresenter>();
        }
    }
}