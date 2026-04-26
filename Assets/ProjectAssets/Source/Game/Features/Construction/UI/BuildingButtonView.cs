using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Features.Construction.UI
{
    public sealed class BuildingButtonView : MonoBehaviour, IBuildingButtonView, IPointerEnterHandler, IPointerExitHandler
    {
        public Button ButtonComponent => buttonComponent;
        public Sprite BuildingImage { set => buildingImage.sprite = value; }
        
        public string TooltipText
        {
            set
            {
                if (tooltipObject == null) return;
                tooltipObject.GetComponentInChildren<TMP_Text>().text = value;
            }
        }

        [SerializeField]
        private Button buttonComponent;

        [SerializeField]
        private Image buildingImage;

        [SerializeField]
        private GameObject tooltipObject;

        private void SetTooltipActive(bool isActive)
        {
            if (tooltipObject == null) return;
            tooltipObject.SetActive(isActive);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            SetTooltipActive(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            SetTooltipActive(false);
        }

        private void Start()
        {
            SetTooltipActive(false);
        }
    }
}