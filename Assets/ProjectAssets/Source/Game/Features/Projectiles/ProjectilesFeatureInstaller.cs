using Game.Core.Projectiles;
using Game.Features.Projectiles.Services;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Projectiles
{
    public sealed class ProjectilesFeatureInstaller : IInstaller
    {
        private readonly MessagePipeOptions messagePipeOptions;
        
        public ProjectilesFeatureInstaller(MessagePipeOptions messagePipeOptions)
        {
            this.messagePipeOptions = messagePipeOptions;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ProjectilesTimeService>();
            builder.RegisterMessageBroker<ProjectileEmissionArgs>(messagePipeOptions);
        }
    }
}