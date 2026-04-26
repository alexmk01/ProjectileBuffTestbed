
namespace Game.Core.HitPoints
{
    public sealed class HitPointsState
    {
        public float Current { get; internal set; }
        public float Max { get; internal set; }

        public HitPointsState(float current, float max)
        {
            Current = current;
            Max = max;
        }

        public HitPointsState(float max) : this(max, max) { }
    }
}