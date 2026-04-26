using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Interaction.UI
{
    public sealed class EntityDragUIFeatureInstaller : IInstaller
    {
        private readonly GameObject entityDragHighlightViewPrefab;
        private readonly GameObject entityDragIndicatorViewPrefab;

        public EntityDragUIFeatureInstaller(GameObject entityDragHighlightViewPrefab, GameObject entityDragIndicatorViewPrefab)
        {
            this.entityDragHighlightViewPrefab = entityDragHighlightViewPrefab;
            this.entityDragIndicatorViewPrefab = entityDragIndicatorViewPrefab;
        }
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(entityDragHighlightViewPrefab.GetComponent<EntityDragHighlightView>(), Lifetime.Singleton)
                .AsImplementedInterfaces();
            builder.RegisterComponentInNewPrefab(entityDragIndicatorViewPrefab.GetComponent<EntityDragIndicatorView>(), Lifetime.Singleton)
                .AsImplementedInterfaces();
            builder.RegisterEntryPoint<EntityDragHighlightPresenter>();
            builder.RegisterEntryPoint<EntityDragIndicatorPresenter>();
        }
    }
}