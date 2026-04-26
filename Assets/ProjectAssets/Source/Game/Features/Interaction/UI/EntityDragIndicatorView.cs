using Game.Core.Interaction;
using UnityEngine;

namespace Game.Features.Interaction.UI
{
    
    public sealed class EntityDragIndicatorView : MonoBehaviour, IEntityDragIndicatorView
    {
        public bool IsVisible
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }

        [SerializeField]
        private Sprite dragSuccessIcon;
        
        [SerializeField]
        private Color dragSuccessColor = new(0f, 1f, 0f, 0.5f);

        [SerializeField]
        private Sprite dragFailIcon;
        
        [SerializeField]
        private Color dragFailColor = new(1f, 0f, 0f, 0.5f);

        [SerializeField]
        private Sprite entityDestructionIcon;

        [SerializeField]
        private Color entityDestructionColor = new(1f, 0f, 0f, 0.5f);

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public void UpdateView(Vector2 targetPosition, EntityDragResult dragResult)
        {
            IsVisible = true;
            transform.position = targetPosition;

            spriteRenderer.sprite = dragResult switch
            {
                EntityDragResult.Success => dragSuccessIcon,
                EntityDragResult.Failed => dragFailIcon,
                EntityDragResult.WillBeDestroyed => entityDestructionIcon,
                _ => null
            };

            spriteRenderer.color = dragResult switch
            {
                EntityDragResult.Success => dragSuccessColor,
                EntityDragResult.Failed => dragFailColor,
                EntityDragResult.WillBeDestroyed => entityDestructionColor,
                _ => Color.white
            };
        }
    }
}