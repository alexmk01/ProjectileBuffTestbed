
namespace Game.Core.HitPoints.Commands
{
    public record struct ChangeHitPointsCommand(int EntityId, float Amount);
}