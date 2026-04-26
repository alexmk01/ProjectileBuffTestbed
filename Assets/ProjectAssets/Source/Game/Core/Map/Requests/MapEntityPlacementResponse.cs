using System.Numerics;

namespace Game.Core.Map.Requests
{
    public record struct MapEntityPlacementResponse(bool CanBePlaced, Vector2 Position);
}