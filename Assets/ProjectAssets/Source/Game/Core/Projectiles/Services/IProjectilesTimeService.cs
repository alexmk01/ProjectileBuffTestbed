using Game.Core.Services;

namespace Game.Core.Projectiles.Services
{
    public interface IProjectilesTimeService : IGameTimeService
    {
        bool IsProjectilesTimeFrozen { get; }
    }
}