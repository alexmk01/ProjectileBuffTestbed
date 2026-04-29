
namespace Game.Core.HitPoints
{
    public interface IHitPointsService
    {
        bool ApplyDamage(HitPointsState hitPoints, float amount);
        bool ApplyHealing(HitPointsState hitPoints, float amount);
    }
}