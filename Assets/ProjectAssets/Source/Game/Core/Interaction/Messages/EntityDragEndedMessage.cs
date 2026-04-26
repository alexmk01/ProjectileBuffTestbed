using Game.Core.Entities;

namespace Game.Core.Interaction.Events
{
    public record struct EntityDragEndedMessage(IEntity Entity);
}