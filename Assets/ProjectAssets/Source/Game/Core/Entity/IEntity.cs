using System.Numerics;
using Game.Core.HitPoints;

namespace Game.Core.Entities
{
    public interface IEntity
    {
        int InstanceId { get; }
        Vector2 Position { get; set; }
        HitPointsState HitPointsState { get; }
    }
}
