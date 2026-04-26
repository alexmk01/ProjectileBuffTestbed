using System.Numerics;
using Game.Core.Buildings;

namespace Game.Core.Construction.Commands
{
    public record struct ConstructBuildingCommand(BuildingId BuildingId, Vector2 Position);
}