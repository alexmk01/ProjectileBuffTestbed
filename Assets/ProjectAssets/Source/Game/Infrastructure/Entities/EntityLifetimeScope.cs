using Game.Core.Entities;
using VContainer;
using VContainer.Unity;

namespace Game.Infrastructure
{
    public sealed class EntityLifetimeScope : LifetimeScope
    {
        internal IInstaller[] EntityScopeInstallers;
        
        protected override void Configure(IContainerBuilder builder)
        {
            if (TryGetComponent(out IEntity entity))
            {
                builder.RegisterComponent(entity);
            }

            foreach (IInstaller installer in EntityScopeInstallers)
            {
                installer.Install(builder);
            }
        }
        
        protected override void Awake()
        {
            autoRun = false;
            base.Awake();
        }
    }
}
