using UnityEngine;
using UnityEngine.UI;

namespace Game.Features.Projectiles.UI
{    
    public sealed class ProjectilesTimePanelView : MonoBehaviour, IProjectilesTimePanelView
    {
        public Button FreezeTimeButton { get => freezeTimeButton; }
        public Button UnfreezeTimeButton { get => unfreezeTimeButton; }
        public Button SpeedUpTimeButton { get => speedUpTimeButton; }

        [SerializeField]
        private Button freezeTimeButton;

        [SerializeField]
        private Button unfreezeTimeButton;

        [SerializeField]
        private Button speedUpTimeButton;

        [SerializeField]
        private GameObject timeFreezeIndicator;

        private void SetTimeFreezeIndicatorActive(bool isActive)
        {
            if (timeFreezeIndicator == null) return;
            timeFreezeIndicator.SetActive(isActive);
        }
        
        private void Start()
        {
            SetTimeFreezeIndicatorActive(false);
            freezeTimeButton.onClick.AddListener(() => SetTimeFreezeIndicatorActive(true));
            unfreezeTimeButton.onClick.AddListener(() => SetTimeFreezeIndicatorActive(false));
            speedUpTimeButton.onClick.AddListener(() => SetTimeFreezeIndicatorActive(false));
        }
    }
}