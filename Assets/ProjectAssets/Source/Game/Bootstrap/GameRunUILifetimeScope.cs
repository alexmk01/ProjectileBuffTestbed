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
        public Canvas HitPointsScreenViewCanvas;
        public Canvas BuildingBehaviourScreenViewCanvas;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var mainCanvas = GetComponentInChildren<Canvas>();
            Canvas TakeCanvasOrDefault(Canvas canvas) => canvas != null ? canvas : mainCanvas;
            
            new BuildingConstructionUIFeatureInstaller(ConstructionModeViewPrefab, ConstructionPanelViewPrefab, mainCanvas.transform).Install(builder);
            new ProjectilesUIFeatureInstaller(ProjectilesViewPanelPrefab, mainCanvas.transform).Install(builder);
            new HitPointsUIFeatureInstaller(HitPointsScreenViewPrefab, TakeCanvasOrDefault(HitPointsScreenViewCanvas).transform).Install(builder);
            new EntityDragUIFeatureInstaller(EntityDragHighlightViewPrefab, EntityDragIndicatorViewPrefab).Install(builder);
            new BuildingBehaviourUIFeatureInstaller(BuildingBehaviourScreenViewPrefab, TakeCanvasOrDefault(BuildingBehaviourScreenViewCanvas).transform).Install(builder);
        }
    }
}
