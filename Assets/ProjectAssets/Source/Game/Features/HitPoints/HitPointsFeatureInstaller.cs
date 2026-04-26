using Game.Core.HitPoints.Commands;
using Game.Core.HitPoints.Events;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Game.Features.HitPoints
{
    public sealed class HitPointsFeatureInstaller : IInstaller
    {
        private readonly MessagePipeOptions messagePipeOptions;

        public HitPointsFeatureInstaller(MessagePipeOptions messagePipeOptions)
        {
            this.messagePipeOptions = messagePipeOptions;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterMessageBroker<ChangeHitPointsCommand>(messagePipeOptions);
            builder.RegisterMessageBroker<HitPointsChangedMessage>(messagePipeOptions);
            builder.RegisterMessageBroker<HitPointsExhaustedMessage>(messagePipeOptions);
        }
    }
}