using Common.Unity;
using Game.Core.Map;
using UnityEngine;

namespace Game.Features.Map
{
    public sealed class GameMapGridComponent : MonoBehaviour
    {
        public IGameMapGrid Grid { get; internal set; }
        
        private void OnDrawGizmosSelected()
        {
            if (Grid == null) return;
            int cellCountX = Grid.CellCountX;
            int cellCountY = Grid.CellCountY;
            float cellSize = Grid.CellSize;
            Vector2 size = new(cellCountX * cellSize, cellCountY * cellSize);
            Vector2 origin = Grid.Origin.ToUnity();

            Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
            Gizmos.DrawWireCube(origin + size * 0.5f, size);
            
            for (int i = 0; i < cellCountX; i++)
            {
                Gizmos.DrawRay(origin + new Vector2 { x = i * cellSize }, new Vector2 { y = size.y });
            }

            for (int i = 0; i < cellCountY; i++)
            {
                Gizmos.DrawRay(origin + new Vector2 { y = i * cellSize }, new Vector2 { x = size.x });
            }
        
            int cellCount = Grid.CellCount;
            Vector3 gizmoSize = new(cellSize, cellSize);
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            
            for (int i = 0; i < cellCount; i++)
            {
                GameMapCellCoords cellCoords = Grid.GetCellCoords(i);
                if (!Grid.IsBusyCell(cellCoords)) continue;
                Gizmos.DrawCube(Grid.GetCellPosition(cellCoords).ToVector3(), gizmoSize);
            }
        }
    }
}