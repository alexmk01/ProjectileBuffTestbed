using System.Numerics;

namespace Game.Core.Map
{
    public readonly struct GameMapArea
    {
        public Vector2 Size => Max - Min;
        public Vector2 Center => (Min + Max) * 0.5f;

        public readonly Vector2 Min;
        public readonly Vector2 Max;
        
        public GameMapArea(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }
        
        public bool ContainsPoint(Vector2 point)
        {
            return point.X >= Min.X && point.X <= Max.X && point.Y >= Min.Y && point.Y <= Max.Y;
        }
    }
}