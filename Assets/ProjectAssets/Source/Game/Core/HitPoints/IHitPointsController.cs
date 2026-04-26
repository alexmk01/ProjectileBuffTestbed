
namespace Game.Core.HitPoints
{
    public interface IHitPointsController
    {
        bool ApplyDamage(HitPointsState hitPoints, float amount);
        bool ApplyHealing(HitPointsState hitPoints, float amount);
    }
}