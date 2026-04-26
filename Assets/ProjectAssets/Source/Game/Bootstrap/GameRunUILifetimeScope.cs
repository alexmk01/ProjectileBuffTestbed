using Game.Features.BuildingBehaviour.UI;
using Game.Features.Construction.UI;
using Game.Features.HitPoints.UI;
using Game.Features.Interaction.UI;
using Game.Features.Projectiles.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameRunUILifetimeScope : LifetimeScope
    {
        public GameObject ConstructionModeViewPrefab;
        public GameObject ConstructionPanelViewPrefab;
        public GameObject ProjectilesViewPanelPrefab;
        public GameObject HitPointsScreenViewPrefab;
        public GameObject EntityDragHighlightViewPrefab;
        public GameObject EntityDragIndicatorViewPrefab;
        public GameObject BuildingBehaviourScreenViewPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var canvas = GetComponentInChildren<Canvas>();
            new BuildingConstructionUIFeatureInstaller(ConstructionModeViewPrefab, ConstructionPanelViewPrefab, canvas.transform).Install(builder);
            new ProjectilesUIFeatureInstaller(ProjectilesViewPanelPrefab, canvas.transform).Install(builder);
            new HitPointsUIFeatureInstaller(HitPointsScreenViewPrefab, canvas.transform).Install(builder);
            new EntityDragUIFeatureInstaller(EntityDragHighlightViewPrefab, EntityDragIndicatorViewPrefab).Install(builder);
            new BuildingBehaviourUIFeatureInstaller(BuildingBehaviourScreenViewPrefab, canvas.transform).Install(builder);
        }
    }
}
