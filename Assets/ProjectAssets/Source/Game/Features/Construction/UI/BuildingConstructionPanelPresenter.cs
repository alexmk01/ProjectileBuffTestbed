using System;
using System.Collections.Generic;
using Common;
using Common.Unity;
using Common.Unity.Tags.Hierarchical;
using Game.Core.Buildings;
using Game.Core.Construction.Commands;
using Game.Infrastructure;
using Game.Infrastructure.Entities.Factory;
using Game.Infrastructure.Localization;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Game.Features.Construction.UI
{
    public sealed class BuildingConstructionPanelPresenter : IInitializable, IDisposable
    {
        private const string LocalizationTable = "PlayerBuildings";

        private readonly IBuildingRepository buildingRepository;
        private readonly IPublisher<StartBuildingConstructionModeCommand> startConstructionModePublisher;
        private readonly IBuildingConstructionPanelView buildingConstructionPanelView;
        private IDisposable disposables;
        
        public BuildingConstructionPanelPresenter
        (
            IBuildingRepository buildingRepository,
            IPublisher<StartBuildingConstructionModeCommand> startConstructionModePublisher,
            IBuildingConstructionPanelView buildingConstructionPanelView
        )
        {
            this.buildingRepository = buildingRepository;
            this.startConstructionModePublisher = startConstructionModePublisher;
            this.buildingConstructionPanelView = buildingConstructionPanelView;
        }
        
        public void Initialize()
        {
            IReadOnlyList<IIdentifiable<BuildingId>> buildingsData = buildingRepository.BuildingsData;
            DisposableBuilder disposableBuilder = Disposable.CreateBuilder();
            
            for (int i = 0; i < buildingsData.Count; i++)
            {
                if (!buildingRepository.IsPlayerBuildingData(buildingsData[i])) continue;
                var buildingDescription = (EntityDescriptionComponent)buildingsData[i];
                SerializableGUID id = buildingDescription.Id;
                IBuildingButtonView button = buildingConstructionPanelView.CreateBuildingButton();
                button.BuildingImage = buildingDescription.GetComponentInChildren<SpriteRenderer>()?.sprite;
                button.SetLocalizedText(LocalizationTable, LocalizationUtility.GetDescriptionKey(id.ToTag()), (b, text) => b.TooltipText = text);
                button.ButtonComponent.OnClickAsObservable()
                    .Subscribe(_ => startConstructionModePublisher.Publish(new StartBuildingConstructionModeCommand(id.ToBuildingId())))
                    .AddTo(ref disposableBuilder);
            }
            
            disposables = disposableBuilder.Build();
        }
        
        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}