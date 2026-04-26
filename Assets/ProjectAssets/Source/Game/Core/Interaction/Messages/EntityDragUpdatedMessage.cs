using System.Numerics;
using Game.Core.Entities;

namespace Game.Core.Interaction.Events
{
    public record struct EntityDragUpdatedMessage(IEntity Entity, Vector2 Position, Vector2 TargetPosition, EntityDragResult DragResult);
}