using Game.Features.Player.Controller;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Player
{
    public sealed class PlayerFeatureInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PlayerController>().WithParameter(new PlayerInputActions());
        }
    }
}