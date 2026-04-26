using System.Numerics;

namespace Game.Core.Buildings.Factory
{
    public record struct BuildingFactoryArgs(BuildingId BuildingId, Vector2 Position);
}