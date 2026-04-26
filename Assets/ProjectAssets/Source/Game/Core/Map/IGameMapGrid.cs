using System;
using System.Numerics;

namespace Game.Core.Map
{
    public interface IGameMapGrid
    {
        int CellCount { get; }
        float CellSize { get; }
        int CellCountX { get; }
        int CellCountY { get; }
        Vector2 Origin { get; }

        bool AddEntity(int entityId, Vector2 position);
        bool RemoveEntity(int entityId);
        int GetEntities(int cellIndex, Span<int> entityIdsBuffer);
        int GetEntityCount(int cellIndex);
        GameMapCellCoords GetCellCoords(Vector2 position);
        GameMapCellCoords GetCellCoords(int cellIndex);
        int GetCellIndex(in GameMapCellCoords cellCoords);
        Vector2 GetCellPosition(in GameMapCellCoords cellCoords);
        bool IsBusyCell(in GameMapCellCoords cellCoords);
    }
}
