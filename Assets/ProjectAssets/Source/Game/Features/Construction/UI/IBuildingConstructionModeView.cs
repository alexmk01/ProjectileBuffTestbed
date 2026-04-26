using System;
using Game.Core.Map;
using UnityEngine;

namespace Game.Features.Construction.UI
{
    public interface IBuildingConstructionModeView
    {
        void HighlightGrid(in GameMapGridParameters gridParameters, ReadOnlySpan<GameMapArea> buildingConstructionAreas);
        void HideGridHighlight();
        void PreviewBuilding(GameObject buildingPrefab, Vector2 position, bool isConstructionAllowed);
        void HideBuildingPreview();
    }
}