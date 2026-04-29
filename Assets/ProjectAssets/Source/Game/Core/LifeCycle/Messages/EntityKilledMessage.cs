
namespace Game.Core.LifeCycle.Messages
{
    public record struct EntityKilledMessage(IKillable Entity);
}