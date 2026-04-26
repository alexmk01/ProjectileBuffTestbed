using System;
using System.Collections.Generic;
using Common;
using Game.Infrastructure;
using Game.Infrastructure.UI;
using UnityEngine;

namespace Game.Features.HitPoints.UI
{    
    public sealed class HitPointsScreenView : MonoBehaviour, IHitPointsScreenView
    {
        public Canvas Canvas { get; private set; }
        public RectTransform Transform { get;  private set; }

        [SerializeField]
        private GameObject hitPointsViewPrefab;

        [SerializeField]
        private GameObject hitPointsChangeMessageViewPrefab;

        [SerializeField]
        private float hitPointsChangeMessageLifetime = 1.5f;
        
        private readonly List<IHitPointsView> views = new(16);

        private void Awake()
        {
            Canvas = GetComponentInParent<Canvas>();
            Transform = (RectTransform)transform;
        }

        public void QueryHitPointsViews(int count, out ReadOnlySpan<IHitPointsView> views)
        {
            if (count > this.views.Count)
            {
                for (int i = this.views.Count; i < count; i++)
                {
                    this.views.Add(Instantiate(hitPointsViewPrefab, transform, false).GetComponent<IHitPointsView>());
                }
            }
            else
            {
                for (int i = this.views.Count - 1; i >= count; i--)
                {
                    this.views[i].IsVisible = false;
                }
            }
            
            views = this.views.AsSpan()[..count];
        }
        
        public void ShowHitPointsChangeMessage(Vector2 localPosition, float amount)
        {
            hitPointsChangeMessageViewPrefab.InstantiateMessageView(localPosition, amount.FormatAmount(), Transform, hitPointsChangeMessageLifetime);
        }
    }
}