using System;
using Common.Unity;
using Game.Core.Interaction.Events;
using MessagePipe;
using VContainer.Unity;

namespace Game.Features.Interaction.UI
{
    public sealed class EntityDragIndicatorPresenter : IInitializable, IDisposable
    {
        private readonly IEntityDragIndicatorView view;
        private readonly IDisposable disposables;

        public EntityDragIndicatorPresenter
        (
            ISubscriber<EntityDragUpdatedMessage> dragUpdatedSubscriber,
            ISubscriber<EntityDragEndedMessage> dragEndedSubscriber,
            IEntityDragIndicatorView view
        )
        {
            this.view = view;
            var disposableBuilder = DisposableBag.CreateBuilder();

            dragUpdatedSubscriber
                .Subscribe(message => this.view.UpdateView(message.TargetPosition.ToUnity(), message.DragResult))
                .AddTo(disposableBuilder);
            
            dragEndedSubscriber
                .Subscribe(_ => this.view.IsVisible = false)
                .AddTo(disposableBuilder);
            
            disposables = disposableBuilder.Build();
        }

        public void Initialize()
        {
            view.IsVisible = false;
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}