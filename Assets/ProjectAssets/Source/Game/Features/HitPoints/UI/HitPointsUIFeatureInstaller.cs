using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.HitPoints.UI
{
    public sealed class HitPointsUIFeatureInstaller : IInstaller
    {
        private readonly GameObject hitPointsScreenViewPrefab;
        private readonly Transform canvasRoot;

        public HitPointsUIFeatureInstaller(GameObject hitPointsScreenViewPrefab, Transform canvasRoot)
        {
            this.hitPointsScreenViewPrefab = hitPointsScreenViewPrefab;
            this.canvasRoot = canvasRoot;
        }
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(hitPointsScreenViewPrefab.GetComponent<HitPointsScreenView>(), Lifetime.Singleton)
                .UnderTransform(canvasRoot)
                .AsImplementedInterfaces();
            builder.RegisterEntryPoint<HitPointsScreenPresenter>();
        }
    }
}