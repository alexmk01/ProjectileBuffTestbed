using UnityEngine.UI;

namespace Game.Features.Projectiles.UI
{
    public interface IProjectilesTimePanelView
    {
        Button FreezeTimeButton { get; }
        Button UnfreezeTimeButton { get; }
        Button SpeedUpTimeButton { get; }
    }
}