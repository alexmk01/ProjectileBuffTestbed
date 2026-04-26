using System.Collections.Generic;
using Game.Core.Projectiles;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Projectiles
{
    public sealed class ProjectilesEntityScopeFeatureInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            var scope = (LifetimeScope)builder.ApplicationOrigin;
            
            if (scope.TryGetComponent(out IProjectileEmitterConfigProvider configProvider) && configProvider.ProjectileEmitterConfig is {} emitterConfig)
            {
                static void InjectProjectilesBehavioursDependencies(IObjectResolver resolver)
                {
                    var projectileEmitter = resolver.Resolve<ProjectileEmitter>();
                    IProjectilesRenderer renderer = projectileEmitter.ProjectilesRenderer;
                    IReadOnlyList<IProjectilesBehaviour> behaviours = projectileEmitter.ProjectilesBehaviours;
                    if (renderer != null) resolver.Inject(renderer);
                    
                    for (int i = 0; i < behaviours.Count; i++)
                    {
                        resolver.Inject(behaviours[i]);
                    }
                }
                
                builder.Register<ProjectileEmitter>(Lifetime.Scoped).AsSelf().WithParameter(emitterConfig);
                builder.RegisterComponent(scope.gameObject.AddComponent<ProjectileEmitterComponent>());
                builder.RegisterBuildCallback(InjectProjectilesBehavioursDependencies);
            }
        }
    }
}