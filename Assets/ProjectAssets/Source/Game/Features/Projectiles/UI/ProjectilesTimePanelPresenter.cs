using System;
using Game.Core.Data;
using Game.Core.Projectiles.Commands;
using MessagePipe;
using R3;
using VContainer.Unity;

namespace Game.Features.Projectiles.UI
{
    public sealed class ProjectilesTimePanelPresenter : IInitializable, IDisposable
    {
        private readonly GameConfig gameConfig;
        private readonly IPublisher<SetProjectileTimeStateCommand> setTimeStatePublisher;
        private readonly IProjectilesTimePanelView view;
        private IDisposable disposables;
        
        public ProjectilesTimePanelPresenter(GameConfig gameConfig, IPublisher<SetProjectileTimeStateCommand> setTimeStatePublisher, IProjectilesTimePanelView view)
        {
            this.gameConfig = gameConfig;
            this.setTimeStatePublisher = setTimeStatePublisher;
            this.view = view;
        }
        
        public void Initialize()
        {
            var disposableBuilder = Disposable.CreateBuilder();
            view.FreezeTimeButton.OnClickAsObservable()
                .Subscribe(_ => setTimeStatePublisher.Publish(new SetProjectileTimeStateCommand(0f)))
                .AddTo(ref disposableBuilder);
            view.UnfreezeTimeButton.OnClickAsObservable()
                .Subscribe(_ => setTimeStatePublisher.Publish(new SetProjectileTimeStateCommand(1f)))
                .AddTo(ref disposableBuilder);
            view.SpeedUpTimeButton.OnClickAsObservable()
                .Subscribe(_ => setTimeStatePublisher.Publish(new SetProjectileTimeStateCommand(gameConfig.ProjectilesTimeSpeedUpMultiplier)))
                .AddTo(ref disposableBuilder);
            disposables = disposableBuilder.Build();
        }
        
        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}