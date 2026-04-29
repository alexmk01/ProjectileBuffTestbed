using System;
using System.Collections.Generic;
using System.Numerics;
using Game.Core.Entities;

namespace Game.Core.Map
{
    public interface IGameMap
    {
        ref readonly GameMapGridParameters GridParameters { get; }
        ReadOnlySpan<GameMapArea> EntityPlacementAreas { get; }
        
        int GetCellIndex(Vector2 position);
        void GetEntitiesInCell(Vector2 position, List<IEntity> entities);
        bool TryGetEntityPlacementArea(IEntity entity, out GameMapArea area);
        bool IsEntityPlacementAllowed(Vector2 worldPosition);
        Vector2 GetCellPosition(Vector2 worldPosition);
        bool IsBusyCell(Vector2 position);
        void MoveEntityToOtherCell(IEntity entity, Vector2 position);
        void AddEntityToGrid(IEntity entity);
        void RemoveEntityFromGrid(IEntity entity);
    }
}