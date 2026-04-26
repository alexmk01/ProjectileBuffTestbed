using TMPro;
using UnityEngine;

namespace Game.Features.HitPoints.UI
{    
    public sealed class HitPointsView : MonoBehaviour, IHitPointsView
    {
        public bool IsVisible
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }
        
        [SerializeField]
        private TMP_Text hitPointsLabel;
        
        public void UpdateView(Vector2 localPosition, float currentHP, float maxHP)
        {
            localPosition = new(localPosition.x, localPosition.y + 110f);
            ((RectTransform)hitPointsLabel.transform).anchoredPosition = localPosition;
            hitPointsLabel.text = $"{currentHP} / {maxHP}";
            IsVisible = true;
        }
    }
}