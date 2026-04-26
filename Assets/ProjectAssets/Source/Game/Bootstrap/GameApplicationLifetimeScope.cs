using Game.Infrastructure.Data;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameApplicationLifetimeScope : LifetimeScope
    {
        public GameConfigAsset GameConfigAsset;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(GameConfigAsset.CreateConfig());
        }
    }
}
