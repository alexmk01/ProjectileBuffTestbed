
namespace Game.Core.Services
{
    public interface IGameTimeService
    {
        float Time { get; }
        float DeltaTime { get; }
        float TimeScale { get; set; }
    }
}