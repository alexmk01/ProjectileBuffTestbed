using Game.Core.Entities;

namespace Game.Core.Interaction.Events
{
    public record struct EntityDragStartedMessage(IEntity Entity);
}