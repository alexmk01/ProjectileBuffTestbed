using System.Collections.Generic;
using System.Linq;
using Common.Unity;
using Game.Core.Map;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Features.Map.Loaders
{
    public sealed class PrefabBasedGameMapLoader : IGameMapLoader
    {
        private readonly GameObject mapPrefab;
        
        public PrefabBasedGameMapLoader(GameObject mapPrefab)
        {
            this.mapPrefab = mapPrefab;
        }

        public void LoadMap(out IGameMapGrid mapGrid, out GameMapArea[] entityPlacementAreas, out Transform[] mapEntityRoots)
        {
            static void GetEntityPlacementAreas(Transform root, List<GameMapAreaComponent> areas)
            {
                int childCount = root.childCount;

                for (int i = 0; i < childCount; i++)
                {
                    Transform child = root.GetChild(i);

                    if (child.TryGetComponent(out GameMapAreaComponent areaComponent))
                    {
                        areas.Add(areaComponent);
                    }
                    else if (child.childCount != 0)
                    {
                        GetEntityPlacementAreas(child, areas);
                    }
                }
            }

            mapGrid = null;
            entityPlacementAreas = null;
            mapEntityRoots = null;
            GameObject mapRootObject = Object.Instantiate(mapPrefab);

            if (mapRootObject.GetComponentInChildren<GridLayout>() is { } gridLayout)
            {
                using (ListPool<GameMapAreaComponent>.Get(out var placementAreaComponents))
                {
                    GetEntityPlacementAreas(mapRootObject.transform, placementAreaComponents);

                    if (placementAreaComponents.Count != 0)
                    {
                        Vector2 gridMin = new(float.MaxValue, float.MaxValue);
                        Vector2 gridMax = new(float.MinValue, float.MinValue);
                        mapEntityRoots = new Transform[placementAreaComponents.Count];

                        for (int i = 0; i < placementAreaComponents.Count; i++)
                        {
                            GameMapAreaComponent areaComponent = placementAreaComponents[i];
                            GameMapArea area = areaComponent.Area;
                            Vector2 areaMin = area.Min.ToUnity();
                            Vector2 areaMax = area.Max.ToUnity();
                            gridMin = Vector2.Min(gridMin, areaMin);
                            gridMax = Vector2.Max(gridMax, areaMax);
                            mapEntityRoots[i] = areaComponent.transform;
                        }

                        entityPlacementAreas = placementAreaComponents.Select(component => component.Area).ToArray();
                        float cellSize = Mathf.Max(gridLayout.cellSize.x + gridLayout.cellGap.x, gridLayout.cellSize.y + gridLayout.cellGap.y);
                        int cellCountX = Mathf.CeilToInt((gridMax.x - gridMin.x) / cellSize);
                        int cellCountY = Mathf.CeilToInt((gridMax.y - gridMin.y) / cellSize);
                        mapGrid = new GameMapGrid(gridMin.ToNumerics(), cellCountX, cellCountY, cellSize);
#if UNITY_EDITOR
                        mapRootObject.AddComponent<GameMapGridComponent>().Grid = mapGrid;
#endif
                    }
                }
            }
        }
    }
}