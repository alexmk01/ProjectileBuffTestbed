using System;
using UnityEngine;

namespace Game.Features.HitPoints.UI
{
    public interface IHitPointsScreenView
    {
        Canvas Canvas { get; }
        RectTransform Transform { get; }

        void QueryHitPointsViews(int count, out ReadOnlySpan<IHitPointsView> views);
        void ShowHitPointsChangeMessage(Vector2 localPosition, float amount);
    }
}