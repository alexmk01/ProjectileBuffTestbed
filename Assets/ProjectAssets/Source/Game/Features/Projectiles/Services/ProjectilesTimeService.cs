using System;
using Game.Core.Projectiles.Commands;
using Game.Core.Projectiles.Services;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.Features.Projectiles.Services
{
    public sealed class ProjectilesTimeService : IProjectilesTimeService, ITickable, IDisposable
    {
        private const float MinTimeScale = 0.000001f;

        public float Time => time;
        public float DeltaTime => UnityEngine.Time.unscaledDeltaTime * timeScale;

        public float TimeScale
        {
            get => timeScale;
            set
            {
                timeScale = Mathf.Max(value, MinTimeScale);
                IsProjectilesTimeFrozen = Mathf.Approximately(timeScale, MinTimeScale);
            }
        }
        
        public bool IsProjectilesTimeFrozen { get; private set; }

        private readonly IDisposable disposables;
        private float time;
        private float timeScale;

        public ProjectilesTimeService(ISubscriber<SetProjectileTimeStateCommand> setTimeStateSubscriber)
        {
            timeScale = 1f;
            var disposableBuilder = DisposableBag.CreateBuilder();
            //TODO: move to presenter?
            setTimeStateSubscriber.Subscribe(message => TimeScale = message.TimeMultiplier).AddTo(disposableBuilder);
            disposables = disposableBuilder.Build();
        }
        
        public void Tick()
        {
            time += DeltaTime;
        }
        
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}