using UnityEngine;
using UnityEngine.UI;

namespace Game.Features.Construction.UI
{
    public interface IBuildingButtonView
    {
        Button ButtonComponent { get; }
        Sprite BuildingImage { set; }
        string TooltipText { set; }
    }
}