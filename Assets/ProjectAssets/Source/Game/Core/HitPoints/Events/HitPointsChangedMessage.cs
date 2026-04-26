using Game.Core.Entities;

namespace Game.Core.HitPoints.Events
{
    public record struct HitPointsChangedMessage(IEntity Entity, float Last, float Current, float Max)
    {
        public readonly float Amount => Current - Last;
    }
}