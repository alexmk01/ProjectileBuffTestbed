using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Projectiles.UI
{
    public sealed class ProjectilesUIFeatureInstaller : IInstaller
    {
        public readonly GameObject ProjectilesTimePanelViewPrefab;
        public readonly Transform CanvasRoot;
        
        public ProjectilesUIFeatureInstaller(GameObject projectilesTimePanelViewPrefab, Transform canvasRoot)
        {
            ProjectilesTimePanelViewPrefab = projectilesTimePanelViewPrefab;
            CanvasRoot = canvasRoot;
        }
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(ProjectilesTimePanelViewPrefab.GetComponent<ProjectilesTimePanelView>(), Lifetime.Singleton)
                .UnderTransform(CanvasRoot)
                .AsImplementedInterfaces();
            
            builder.RegisterEntryPoint<ProjectilesTimePanelPresenter>();
        }
    }
}