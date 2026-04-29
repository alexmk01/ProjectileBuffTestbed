
namespace Game.Core.LifeCycle
{
    public interface IKillable
    {
        bool IsKilled { get; }

        void Kill();
    }
}