using System;
using Common.Unity;
using Game.Core.Map;
using UnityEngine;

namespace Game.Features.Construction.UI
{    
    public sealed class BuildingConstructionModeView : MonoBehaviour, IBuildingConstructionModeView
    {
        public GameObject GridHighlightSpritePrefab;
        public GameObject BuildingConstructionAreaHighlightPrefab;
        public Color AllowedConstructionColor = new(0f, 1f, 0f, 0.5f);
        public Color UnallowedConstructionColor = new(1f, 0f, 0f, 0.5f);
        
        private SpriteRenderer gridHighlightSprite;
        private SpriteRenderer[] buildingConstructionAreasHighlightSprites;
        private SpriteRenderer currentBuildingPreview;
        
        public void HighlightGrid(in GameMapGridParameters gridParameters, ReadOnlySpan<GameMapArea> buildingConstructionAreas)
        {
            if (GridHighlightSpritePrefab != null)
            {
                if (gridHighlightSprite == null)
                {
                    gridHighlightSprite = Instantiate(GridHighlightSpritePrefab).GetComponent<SpriteRenderer>();
                    gridHighlightSprite.drawMode = SpriteDrawMode.Simple;
                    Vector2 scale = gridHighlightSprite.bounds.size;
                    scale.x = gridParameters.CellSize / scale.x;
                    scale.y = gridParameters.CellSize / scale.y;
                    gridHighlightSprite.drawMode = SpriteDrawMode.Tiled;
                    gridHighlightSprite.size = Vector2.Scale(gridParameters.GridSize.ToUnity(), new(1f / scale.x, 1f / scale.y));
                    Transform highlightTransform = gridHighlightSprite.transform;
                    highlightTransform.localScale = scale;
                    highlightTransform.position = gridParameters.GridCenter.ToUnity() + (Vector2)(gridHighlightSprite.bounds.center - highlightTransform.position);
                }
                else
                {
                    gridHighlightSprite.gameObject.SetActive(true);
                }
            }
            
            if (BuildingConstructionAreaHighlightPrefab != null)
            {
                if (buildingConstructionAreasHighlightSprites == null)
                {
                    buildingConstructionAreasHighlightSprites = new SpriteRenderer[buildingConstructionAreas.Length];
                    
                    for (int i = 0; i < buildingConstructionAreas.Length; i++)
                    {
                        ref readonly GameMapArea buildingConstructionArea = ref buildingConstructionAreas[i];
                        var spriteRenderer = Instantiate(BuildingConstructionAreaHighlightPrefab).GetComponent<SpriteRenderer>();
                        spriteRenderer.drawMode = SpriteDrawMode.Sliced;
                        spriteRenderer.size = buildingConstructionArea.Size.ToUnity();
                        Transform renderTransform = spriteRenderer.transform;
                        renderTransform.localScale = Vector3.one;
                        renderTransform.position = buildingConstructionArea.Center.ToUnity() + (Vector2)(spriteRenderer.bounds.center - renderTransform.position);
                        buildingConstructionAreasHighlightSprites[i] = spriteRenderer;
                    }
                }
                else
                {
                    foreach (SpriteRenderer buildingConstructionAreaHighlightSprite in buildingConstructionAreasHighlightSprites)
                    {
                        buildingConstructionAreaHighlightSprite.gameObject.SetActive(true);
                    }
                }
            }
        }
        
        public void HideGridHighlight()
        {
            if (gridHighlightSprite != null)
            {
                gridHighlightSprite.gameObject.SetActive(false);
            }

            if (buildingConstructionAreasHighlightSprites != null)
            {
                foreach (SpriteRenderer buildingConstructionAreaHighlightSprite in buildingConstructionAreasHighlightSprites)
                {
                    buildingConstructionAreaHighlightSprite.gameObject.SetActive(false);
                }
            }
        }
        
        public void PreviewBuilding(GameObject buildingPrefab, Vector2 position, bool isConstructionAllowed)
        {
            if (currentBuildingPreview == null)
            {
                currentBuildingPreview = Instantiate(buildingPrefab).GetComponent<SpriteRenderer>();
                currentBuildingPreview.sortingOrder = 100;
            }
            
            currentBuildingPreview.transform.position = position;
            currentBuildingPreview.color = isConstructionAllowed ? AllowedConstructionColor : UnallowedConstructionColor;
        }

        public void HideBuildingPreview()
        {
            if (currentBuildingPreview == null) return;
            Destroy(currentBuildingPreview.gameObject);
            currentBuildingPreview = null;
        }
    }
}