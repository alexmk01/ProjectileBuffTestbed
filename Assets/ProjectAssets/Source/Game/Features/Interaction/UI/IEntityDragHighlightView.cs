using Game.Core.Entities;

namespace Game.Features.Interaction.UI
{
    public interface IEntityDragHighlightView
    {
        void SetEntityHighlighted(IEntity entity, bool isHighlighted);
    }
}