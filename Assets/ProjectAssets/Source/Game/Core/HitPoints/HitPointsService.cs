using System;

namespace Game.Core.HitPoints
{
    public class HitPointsService : IHitPointsService
    {
        public bool ApplyHealing(HitPointsState hitPoints, float amount)
        {
            if (amount > 0f && hitPoints.Current < hitPoints.Max)
            {
                hitPoints.Current = Math.Min(hitPoints.Current + amount, hitPoints.Max);
                return true;
            }

            return false;
        }
        
        public bool ApplyDamage(HitPointsState hitPoints, float amount)
        {
            if (amount > 0f && hitPoints.Current > 0f)
            {
                hitPoints.Current = Math.Max(hitPoints.Current - amount, 0f);
                return true;
            }

            return false;
        }
    }
}