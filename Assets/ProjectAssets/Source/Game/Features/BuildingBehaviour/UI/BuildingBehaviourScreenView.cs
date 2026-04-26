using Game.Infrastructure.UI;
using UnityEngine;

namespace Game.Features.BuildingBehaviour.UI
{
    public sealed class BuildingBehaviourScreenView : MonoBehaviour, IBuildingBehaviourScreenView
    {
        public RectTransform Transform { get; private set; }

        [SerializeField]
        private GameObject buildingBehaviourEffectMessageViewPrefab;

        [SerializeField]
        private float buildingBehaviourEffectMessageLifetime = 1.5f;
        
        private void Awake() => Transform = (RectTransform)transform;

        public void ShowBuildingBehaviourEffectMessage(Vector2 localPosition, string message)
        {
            buildingBehaviourEffectMessageViewPrefab.InstantiateMessageView(localPosition, message, transform, buildingBehaviourEffectMessageLifetime);
        }
    }
}