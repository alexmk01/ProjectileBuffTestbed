using UnityEngine;

namespace Game.Features.Construction.UI
{    
    public sealed class BuildingConstructionPanelView : MonoBehaviour, IBuildingConstructionPanelView
    {
        public GameObject BuildingButtonPrefab;
        public Transform ButtonsLayoutRoot;

        public IBuildingButtonView CreateBuildingButton()
        {
            return Instantiate(BuildingButtonPrefab, ButtonsLayoutRoot, false).GetComponent<IBuildingButtonView>();
        }
    }
}