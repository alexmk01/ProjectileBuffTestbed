using System.Numerics;

namespace Game.Core.Player.Controller.Messages
{
    public record struct PlayerPointerMoveMessage(Vector2 Position, Vector2 WorldPosition);
}