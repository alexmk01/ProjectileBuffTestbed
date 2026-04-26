using Game.Core.Buildings;

namespace Game.Core.Construction.Commands
{
    public record struct StartBuildingConstructionModeCommand(BuildingId BuildingId);
}