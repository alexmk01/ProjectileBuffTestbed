using UnityEngine;

namespace Game.Features.BuildingBehaviour.UI
{
    public interface IBuildingBehaviourScreenView
    {
        RectTransform Transform { get; }

        void ShowBuildingBehaviourEffectMessage(Vector2 localPosition, string message);
    }
}