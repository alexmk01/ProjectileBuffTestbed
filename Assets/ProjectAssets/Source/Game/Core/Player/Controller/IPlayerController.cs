using System.Numerics;

namespace Game.Core.Player.Controller
{
    public interface IPlayerController
    {
       Vector2 PointerPosition { get; }
       Vector2 WorldPointerPosition { get; }
       bool IsActive { get; set; }
    }
}