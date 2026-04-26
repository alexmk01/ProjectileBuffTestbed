using System;
using Game.Core.Buildings;
using Game.Core.Construction.Services;
using Game.Core.Entities.Messages;
using MessagePipe;
using VContainer.Unity;

namespace Game.Features.Buildings
{
    public sealed class BuildingDestructionHandler : IInitializable, IDisposable
    {
        private readonly IDisposable disposables;

        public BuildingDestructionHandler(IBuildingConstructionService constructionService, ISubscriber<EntityKilledMessage> killedMessageSubscriber)
        {
            var disposablesBuilder = DisposableBag.CreateBuilder();
            //Handle destruction from damage etc
            killedMessageSubscriber
                .Subscribe(message =>
                {
                    if (message.Entity is IBuilding building) 
                    {
                        constructionService.DestroyBuilding(building);
                    }
                })
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