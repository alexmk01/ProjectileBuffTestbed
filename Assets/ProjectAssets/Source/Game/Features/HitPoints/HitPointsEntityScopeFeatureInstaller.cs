using Game.Core.Entities;
using Game.Core.HitPoints;
using UnityEngine.Assertions;
using VContainer;
using VContainer.Unity;

namespace Game.Features.HitPoints
{
    public sealed class HitPointsEntityScopeFeatureInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            var scope = (LifetimeScope)builder.ApplicationOrigin;
            //Entity scope installer
            Assert.IsTrue(scope.TryGetComponent<IEntity>(out _));
            builder.Register<HitPointsController>(Lifetime.Scoped).As<IHitPointsController>();
            builder.RegisterComponent(scope.gameObject.AddComponent<HitPointsComponent>());
        }
    }
}