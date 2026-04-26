using Game.Core.Entities;

namespace Game.Core.Interaction.Requests
{
    public record struct StartEntityDragResponse(IEntity Entity, bool CanBeDragged);
}