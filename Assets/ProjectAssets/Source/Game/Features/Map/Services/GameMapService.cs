using System;
using System.Collections.Generic;
using System.Numerics;
using Common;
using Game.Core.Entities;
using Game.Core.Map;
using Game.Core.Map.Services;
using Game.Infrastructure.Entities;

namespace Game.Features.Map.Services
{    
    public sealed class GameMapService : IDisposable, IGameMapService
    {
        public ref readonly GameMapGridParameters GridParameters => ref gridParameters;
        public ReadOnlySpan<GameMapArea> EntityPlacementAreas => entityPlacementAreas;

        private readonly IGameMapGrid grid;
        private readonly GameMapGridParameters gridParameters;
        private readonly IEntityRegistry entityRegistry;
        private readonly GameMapArea[] entityPlacementAreas;

        public GameMapService(IGameMapGrid grid, IEntityRegistry entityRegistry, GameMapArea[] entityPlacementAreas)
        {
            this.grid = grid;
            gridParameters = new(grid);
            this.entityRegistry = entityRegistry;
            this.entityPlacementAreas = entityPlacementAreas;
        }
        
        public int GetCellIndex(Vector2 position) => grid.GetCellIndex(grid.GetCellCoords(position));
        
        public Vector2 GetEntityPlacementPosition(Vector2 worldPosition) => grid.GetCellPosition(grid.GetCellCoords(worldPosition));
        //TODO: implement IPlacementValidator
        public bool IsEntityPlacementAllowed(Vector2 worldPosition)
        {
            if (!entityPlacementAreas.IsNullOrEmpty())
            {
                bool isOutOfPlacementAreas = true;

                for (int i = 0; i < entityPlacementAreas.Length; i++)
                {
                    if (entityPlacementAreas[i].ContainsPoint(worldPosition))
                    {
                        isOutOfPlacementAreas = false;
                        break;
                    }
                }

                if (isOutOfPlacementAreas) return false;
            }

            return !grid.IsBusyCell(grid.GetCellCoords(worldPosition));
        }

        public bool TryGetEntityPlacementArea(IEntity entity, out GameMapArea area)
        {
            for (int i = 0; i < entityPlacementAreas.Length; i++)
            {
                if (entityPlacementAreas[i].ContainsPoint(entity.Position))
                {
                    area = entityPlacementAreas[i];
                    return true;
                }
            }

            area = default;
            return false;
        }
        
        public bool IsBusyCell(Vector2 position) => grid.IsBusyCell(grid.GetCellCoords(position));

        public void GetEntitiesInCell(Vector2 position, List<IEntity> entities)
        {
            entities.Clear();
            Span<int> ids = stackalloc int[16];
            int count = grid.GetEntities(GetCellIndex(position), ids);

            for (int i = 0; i < count; i++)
            {
                entities.Add(entityRegistry.GetEntity(ids[i]));
            }
        }
        
        public void MoveEntityToOtherCell(IEntity entity, Vector2 position)
        {
            //if (grid.GetCellCoords(entity.Position) == grid.GetCellCoords(position)) return;
            grid.RemoveEntity(entity.InstanceId);
            entity.Position = GetEntityPlacementPosition(position);
            grid.AddEntity(entity.InstanceId, position);
        }
        
        public void AddEntityToGrid(IEntity entity)
        {
            //TODO: asserts 
            grid.AddEntity(entity.InstanceId, entity.Position);
        }
        
        public void RemoveEntityFromGrid(IEntity entity)
        {
            grid.RemoveEntity(entity.InstanceId);
        }
        
        public void Dispose()
        {
            (grid as IDisposable)?.Dispose();
        }
    }
}