using UnityEngine;

namespace Game.Features.HitPoints.UI
{
    public interface IHitPointsView
    {
        bool IsVisible { get; set; }

        void UpdateView(Vector2 localPosition, float currentHP, float maxHP);
    }
}