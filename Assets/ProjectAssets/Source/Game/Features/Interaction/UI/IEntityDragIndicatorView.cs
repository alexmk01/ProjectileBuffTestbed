using Game.Core.Interaction;
using UnityEngine;

namespace Game.Features.Interaction.UI
{
    public interface IEntityDragIndicatorView
    {
        bool IsVisible { get; set; }
        
        void UpdateView(Vector2 targetPosition, EntityDragResult dragResult);
    }
}