
namespace Game.Core.Projectiles.Events
{
    public record struct ProjectileCellChangedMessage(ProjectileEmitter Emitter, int ProjectileIndex, int LastCellIndex, int CellIndex);
}