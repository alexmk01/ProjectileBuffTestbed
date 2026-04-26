using Game.Core.Interaction.Events;
using Game.Core.Interaction.Requests;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Interaction
{
    public sealed class InteractionFeatureInstaller : IInstaller
    {
        private readonly MessagePipeOptions messagePipeOptions;

        public InteractionFeatureInstaller(MessagePipeOptions messagePipeOptions)
        {
            this.messagePipeOptions = messagePipeOptions;
        }
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterMessageBroker<EntityDragCandidateChangedMessage>(messagePipeOptions);
            builder.RegisterRequestHandler<StartEntityDragRequest, StartEntityDragResponse, BuildingDragHandler>(messagePipeOptions);
        }
    }
}