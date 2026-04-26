using System;
using Game.Core.Construction.Commands;
using Game.Core.Construction.Services;
using MessagePipe;
using VContainer.Unity;

namespace Game.Features.Construction
{
    public sealed class BuildingCommandsHandler : IInitializable, IDisposable
    {
        private readonly IDisposable disposables;

        public BuildingCommandsHandler
        (
            IBuildingConstructionService constructionService,
            ISubscriber<ConstructBuildingCommand> constructBuildingSubscriber,
            ISubscriber<DestroyBuildingCommand> destroyBuildingSubscriber
        )
        {
            var disposablesBuilder = DisposableBag.CreateBuilder();

            constructBuildingSubscriber
                .Subscribe(command => constructionService.TryConstructBuilding(command.BuildingId, command.Position))
                .AddTo(disposablesBuilder);

            destroyBuildingSubscriber
                .Subscribe(command => constructionService.DestroyBuilding(command.Building))
                .AddTo(disposablesBuilder);

            disposables = disposablesBuilder.Build();
        }
        
        void IInitializable.Initialize()
        {
        }
        
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}