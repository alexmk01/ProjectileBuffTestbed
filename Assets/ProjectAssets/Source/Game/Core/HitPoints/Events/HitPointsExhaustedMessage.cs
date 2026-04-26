using Game.Core.Entities;

namespace Game.Core.HitPoints.Events
{
    public record struct HitPointsExhaustedMessage(IEntity Entity);
}