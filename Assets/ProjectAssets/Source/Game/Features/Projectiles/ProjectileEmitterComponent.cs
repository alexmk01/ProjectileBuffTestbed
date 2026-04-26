using System;
using Game.Core.Projectiles;
using Game.Core.Projectiles.Services;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace Game.Features.Projectiles
{
    public sealed class ProjectileEmitterComponent : MonoBehaviour
    {
        private IProjectilesTimeService projectilesTime;
        private ProjectileEmitter projectileEmitter;
        private IDisposable disposables;
        
        [Inject]
        private void Construct(IProjectilesTimeService projectilesTime, ProjectileEmitter projectileEmitter, ISubscriber<int, ProjectileEmissionArgs> projectileEmissionSubscriber)
        {
            this.projectilesTime = projectilesTime;
            this.projectileEmitter = projectileEmitter;
            var disposablesBuilder = DisposableBag.CreateBuilder();
            projectileEmitter.AddTo(disposablesBuilder);
            projectileEmissionSubscriber.Subscribe(projectileEmitter.Id, EmitProjectile).AddTo(disposablesBuilder);
            disposables = disposablesBuilder.Build();
        }

        private void EmitProjectile(ProjectileEmissionArgs args) => projectileEmitter.EmitProjectile(args);

        private void Update()
        {
            projectileEmitter.Update(projectilesTime.Time, projectilesTime.DeltaTime);
        }

        private void OnDestroy()
        {
            disposables?.Dispose();
        }
    }
}