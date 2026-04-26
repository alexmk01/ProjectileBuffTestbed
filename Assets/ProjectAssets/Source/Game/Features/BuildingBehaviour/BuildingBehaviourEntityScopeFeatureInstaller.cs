using Game.Features.BuildingBehaviour.Factory;
using VContainer;
using VContainer.Unity;

namespace Game.Features.BuildingBehaviour
{
    public sealed class BuildingBehaviourEntityScopeFeatureInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            var scope = (LifetimeScope)builder.ApplicationOrigin;
            
            if (scope.gameObject.TryGetComponent(out BuildingBehaviourDataComponent dataComponent) && dataComponent.BehaviourData is { } behaviourData)
            {
                builder.Register<BuildingBehaviourFactory>(Lifetime.Scoped).AsImplementedInterfaces().WithParameter(scope);
                builder.RegisterInstance(behaviourData);
                builder.RegisterComponent(scope.gameObject.AddComponent<BuildingBehaviourComponent>());
            }
        }
    }
}