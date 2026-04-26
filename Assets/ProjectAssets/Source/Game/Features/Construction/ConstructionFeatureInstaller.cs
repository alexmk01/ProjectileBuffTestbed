using Game.Core.Construction.Commands;
using Game.Features.Construction.Services;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Construction
{
    public sealed class ConstructionFeatureInstaller : IInstaller
    {
        private readonly MessagePipeOptions messagePipeOptions;

        public ConstructionFeatureInstaller(MessagePipeOptions messagePipeOptions)
        {
            this.messagePipeOptions = messagePipeOptions;
        }
        
        public void Install(IContainerBuilder builder)
        {
            builder.Register<BuildingSpawner>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<BuildingConstructionService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterEntryPoint<BuildingConstructionPresenter>();
            builder.RegisterEntryPoint<BuildingDestructionHandler>();
            builder.RegisterMessageBroker<StartBuildingConstructionModeCommand>(messagePipeOptions);
            builder.RegisterMessageBroker<ConstructBuildingCommand>(messagePipeOptions);
            builder.RegisterMessageBroker<CompleteBuildingConstructionModeCommand>(messagePipeOptions);
            builder.RegisterMessageBroker<DestroyBuildingCommand>(messagePipeOptions);
        }
    }
}