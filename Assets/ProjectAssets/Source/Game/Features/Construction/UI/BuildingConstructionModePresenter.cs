using System;
using System.Collections.Generic;
using Common;
using Common.Unity;
using Game.Core.Buildings;
using Game.Core.Construction.Commands;
using Game.Core.Map;
using Game.Core.Player.Controller.Messages;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Game.Features.Construction.UI
{
    public sealed class BuildingConstructionModePresenter : IInitializable, IDisposable
    {
        private readonly IBuildingRepository buildingRepository;

        public BuildingConstructionModePresenter
        (
            IBuildingRepository buildingRepository,
            IGameMap gameMap,
            ISubscriber<PlayerPointerMoveMessage> pointerMoveSubscriber,
            ISubscriber<StartBuildingConstructionModeCommand> constructionStartSubscriber,
            ISubscriber<CompleteBuildingConstructionModeCommand> constructionCompleteSubscriber,
            IBuildingConstructionModeView constructionModeView
        )
        {
            this.buildingRepository = buildingRepository;
            this.gameMap = gameMap;
            this.pointerMoveSubscriber = pointerMoveSubscriber;
            this.constructionStartSubscriber = constructionStartSubscriber;
            this.constructionCompleteSubscriber = constructionCompleteSubscriber;
            this.constructionModeView = constructionModeView;
        }

        private readonly IGameMap gameMap;
        private readonly ISubscriber<PlayerPointerMoveMessage> pointerMoveSubscriber;
        private readonly ISubscriber<StartBuildingConstructionModeCommand> constructionStartSubscriber;
        private readonly ISubscriber<CompleteBuildingConstructionModeCommand> constructionCompleteSubscriber;
        private readonly IBuildingConstructionModeView constructionModeView;
        private GameObject currentBuildingPreviewPrefab;
        private System.Numerics.Vector2 buildingPreviewPosition;
        private IDisposable disposables;

        private GameObject GetBuildingPreviewPrefab(BuildingId buildingId)
        {
            IReadOnlyList<IIdentifiable<BuildingId>> data = buildingRepository.BuildingsData;

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Id.Equals(buildingId))
                {
                    return ((Component)data[i]).gameObject;
                }
            }
            
            return null;
        }
        
        public void Initialize()
        {
            DisposableBuilder disposableBuilder = Disposable.CreateBuilder();
            
            constructionStartSubscriber
                .Subscribe(command =>
                {
                    constructionModeView.HighlightGrid(gameMap.GridParameters, gameMap.EntityPlacementAreas);
                    currentBuildingPreviewPrefab = GetBuildingPreviewPrefab(command.BuildingId);
                })
                .AddTo(ref disposableBuilder);

            constructionCompleteSubscriber
                .Subscribe(command =>
                {
                    constructionModeView.HideGridHighlight();
                    constructionModeView.HideBuildingPreview();
                    currentBuildingPreviewPrefab = null;
                })
                .AddTo(ref disposableBuilder);

            pointerMoveSubscriber
                .Subscribe(message => buildingPreviewPosition = gameMap.GetCellPosition(message.WorldPosition))
                .AddTo(ref disposableBuilder);

            Observable.EveryUpdate()
                .Where(_ => currentBuildingPreviewPrefab != null)
                .Subscribe(_ => constructionModeView.PreviewBuilding(currentBuildingPreviewPrefab, buildingPreviewPosition.ToUnity(), gameMap.IsEntityPlacementAllowed(buildingPreviewPosition)))
                .AddTo(ref disposableBuilder);
                
            disposables = disposableBuilder.Build();
        }
        
        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}