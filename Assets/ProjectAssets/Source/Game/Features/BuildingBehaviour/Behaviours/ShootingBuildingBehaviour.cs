using System;
using Common.Unity;
using Game.Core.Buildings;
using Game.Core.Projectiles;
using Game.Core.Projectiles.Services;
using Game.Infrastructure.Data;
using R3;
using UnityEngine;
using VContainer;

namespace Game.Features.BuildingBehaviour.Behaviours
{
    public sealed class ShootingBuildingBehaviour : BuildingBehaviourBase<ShootingBuildingBehaviour.Data>, IDisposable
    {
        [Serializable]
        public sealed class Data : IBuildingBehaviourData, IProjectileEmitterConfigProvider
        {
            ProjectileEmitterConfig IProjectileEmitterConfigProvider.ProjectileEmitterConfig => ProjectileEmitterConfig.CreateConfig();

            public ProjectileEmitterConfigAsset ProjectileEmitterConfig;
            public Transform ProjectileLaunchPivot;
            public Vector3 LocalShootDirection = Vector3.right;
            public float Cooldown = 3f;
        }

        private IProjectilesTimeService projectilesTimeService;
        private ProjectileEmitter projectileEmitter;
        private float nextShotTime;
        private IDisposable disposables;

        private void UpdateNextShotTime()
        {
            nextShotTime = projectilesTimeService.Time + BehaviourData.Cooldown;
        }
        
        [Inject]
        private void Construct(IProjectilesTimeService projectilesTimeService, ProjectileEmitter projectileEmitter)
        {
            this.projectilesTimeService = projectilesTimeService;
            this.projectileEmitter = projectileEmitter;
        }
        
        public override void Initialize()
        {
            var disposableBuilder = Disposable.CreateBuilder();
            UpdateNextShotTime();

            Observable.EveryUpdate()
                .Where(_ => IsActive && projectilesTimeService.Time >= nextShotTime)
                .Subscribe(_ =>
                {
                    Transform launchPivot = BehaviourData.ProjectileLaunchPivot;

                    var args = new ProjectileEmissionArgs
                    {
                        Position = launchPivot.position.ToNumericsVector2(),
                        Direction = (launchPivot.rotation * BehaviourData.LocalShootDirection).ToNumericsVector2()
                    };

                    projectileEmitter.EmitProjectile(args);
                    UpdateNextShotTime();
                })
                .AddTo(ref disposableBuilder);
            //Reset cooldown when not active    
            Observable.EveryUpdate()
                .Where(_ => !IsActive)
                .Subscribe(_ => UpdateNextShotTime())
                .AddTo(ref disposableBuilder);
            
            disposables = disposableBuilder.Build();
        }

        public ShootingBuildingBehaviour(Data data, IBuilding building) : base(data, building)
        {
        }
        
        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}