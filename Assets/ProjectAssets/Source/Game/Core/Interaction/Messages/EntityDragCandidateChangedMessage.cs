using Game.Core.Entities;

namespace Game.Core.Interaction.Events
{
    public record struct EntityDragCandidateChangedMessage(IEntity LastDragCandidate, IEntity NewDragCandidate);
}