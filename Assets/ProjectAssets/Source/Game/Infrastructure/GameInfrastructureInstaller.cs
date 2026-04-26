using Game.Infrastructure.Entities;
using VContainer;
using VContainer.Unity;

namespace Game.Infrastructure
{
    public sealed class GameInfrastructureInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<EntityRegistry>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}