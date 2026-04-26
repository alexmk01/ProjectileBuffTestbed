using Game.Core.Entities;
using UnityEngine;
using Outline = OutlineFx.OutlineFx;

namespace Game.Features.Interaction.UI
{
    public sealed class EntityDragHighlightView : MonoBehaviour, IEntityDragHighlightView
    {
        public Color OutlineColor = Color.orange;
        
        public void SetEntityHighlighted(IEntity entity, bool isHighlighted)
        {
            var mainComponent = (Component)entity;

            if (!mainComponent.TryGetComponent(out Outline outlineComponent))
            {
                outlineComponent = mainComponent.gameObject.AddComponent<Outline>();
            }
            
            outlineComponent.Color = OutlineColor;
            outlineComponent.enabled = isHighlighted;
        }
    }
}