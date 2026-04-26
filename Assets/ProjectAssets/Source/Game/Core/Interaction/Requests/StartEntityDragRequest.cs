using System.Numerics;

namespace Game.Core.Interaction.Requests
{
    public record struct StartEntityDragRequest(Vector2 WorldPointerPosition);
}