using System;
using Game.Core.Entities;
using Game.Core.Interaction.Events;
using MessagePipe;
using VContainer.Unity;

namespace Game.Features.Interaction.UI
{
    public sealed class EntityDragHighlightPresenter : IInitializable, IDisposable
    {
        private readonly IDisposable disposables;

        public EntityDragHighlightPresenter
        (
            ISubscriber<EntityDragCandidateChangedMessage> entityDragCandidateChangedSubscriber,
            IEntityDragHighlightView view
        )
        {
            var disposableBuilder = DisposableBag.CreateBuilder();

            entityDragCandidateChangedSubscriber
                .Subscribe(message =>
                {
                    IEntity lastEntity = message.LastDragCandidate;
                    IEntity newEntity = message.NewDragCandidate;
                    if (lastEntity != null) view.SetEntityHighlighted(lastEntity, false);
                    if (newEntity != null) view.SetEntityHighlighted(newEntity, true);
                })
                .AddTo(disposableBuilder);
                
            disposables = disposableBuilder.Build();
        }
        
        public void Initialize()
        {
        }
        
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}