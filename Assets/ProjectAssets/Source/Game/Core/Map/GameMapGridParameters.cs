using System.Numerics;

namespace Game.Core.Map
{
    public readonly struct GameMapGridParameters
    {
        public readonly Vector2 GridSize => GridMax - GridMin;
        public readonly Vector2 GridCenter => (GridMin + GridMax) * 0.5f;
        
        public readonly Vector2 GridMin;
        public readonly Vector2 GridMax;
        public readonly float CellSize;
        public readonly int CellCountX;
        public readonly int CellCountY;
        
        public GameMapGridParameters(IGameMapGrid grid)
        {
            GridMin = grid.Origin;
            CellSize = grid.CellSize;
            CellCountX = grid.CellCountX;
            CellCountY = grid.CellCountY;
            GridMax = new(GridMin.X + CellSize * CellCountX, GridMin.Y + CellSize * CellCountY);
        }
    }
}