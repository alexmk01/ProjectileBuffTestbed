using Game.Core.BuildingBehaviour.Commands;
using Game.Core.BuildingBehaviour.Messages;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Game.Features.BuildingBehaviour
{
    public sealed class BuildingBehaviourFeatureInstaller : IInstaller
    {
        private readonly MessagePipeOptions messagePipeOptions;

        public BuildingBehaviourFeatureInstaller(MessagePipeOptions messagePipeOptions)
        {
            this.messagePipeOptions = messagePipeOptions;
        }
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterMessageBroker<int, EnableBuildingBehaviourCommand>(messagePipeOptions);
            builder.RegisterMessageBroker<int, DisableBuildingBehaviourCommand>(messagePipeOptions);
            builder.RegisterMessageBroker<BuildingBehaviourEffectAppliedMessage>(messagePipeOptions);
        }
    }
}