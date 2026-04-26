using System;
using System.Numerics;
using Game.Core.BuildingBehaviour.Commands;
using Game.Core.Buildings;
using Game.Core.Construction.Commands;
using Game.Core.Entities;
using Game.Core.Interaction;
using Game.Core.Interaction.Events;
using Game.Core.Interaction.Requests;
using Game.Core.Map;
using Game.Core.Map.Services;
using Game.Core.Player.Controller.Messages;
using MessagePipe;
using UnityEngine.Pool;

namespace Game.Features.Interaction
{
    public sealed class BuildingDragHandler : IRequestHandler<StartEntityDragRequest, StartEntityDragResponse>, IDisposable
    {
        private readonly IGameMapService mapService;
        private readonly IBuildingRepository buildingRepository;
        private readonly IPublisher<EntityDragStartedMessage> entityDragStartedPublisher;
        private readonly IPublisher<EntityDragUpdatedMessage> entityDragUpdatedPublisher;
        private readonly IPublisher<EntityDragCandidateChangedMessage> entityDragCandidateChangedPublisher;
        private readonly IPublisher<DestroyBuildingCommand> destroyBuildingPublisher;
        private readonly IPublisher<int, EnableBuildingBehaviourCommand> enableBuildingBehaviourPublisher;
        private readonly IPublisher<int, DisableBuildingBehaviourCommand> disableBuildingBehaviourPublisher;
        private readonly IDisposable disposables;
        private int lastPointerCellIndex;
        private bool isBuildingConstructionModeActive;
        private IEntity dragCandidateBuilding;
        private IEntity draggingBuilding;
        private Vector2 dragStartPosition;
        private Vector2 dragTargetPosition;
        private EntityDragResult dragResult;
        
        private void UpdateDrag(Vector2 worldPointerPosition)
        {
            if (isBuildingConstructionModeActive)
            {
                ResetDragCandidateState();
                return;
            }
            
            if (draggingBuilding == null)
            {
                int pointerCellIndex = mapService.GetCellIndex(worldPointerPosition);
                
                if (pointerCellIndex != lastPointerCellIndex)
                {
                    using (ListPool<IEntity>.Get(out var entities))
                    {
                        mapService.GetEntitiesInCell(worldPointerPosition, entities);
                        var entityToDrag = entities.Find(entity => entity is IBuilding building && buildingRepository.IsPlayerBuilding(building));
                        SetDragCandidateBuilding(entityToDrag);
                    }
                    
                    lastPointerCellIndex = pointerCellIndex;
                }
            }
            else
            {
                //TODO: move to dragging entity presenter (MonoBehaviour), handle sprite draw order
                draggingBuilding.Position = worldPointerPosition;
                dragTargetPosition = mapService.GetEntityPlacementPosition(worldPointerPosition);
                dragResult = EntityDragResult.Success;

                if (mapService.IsBusyCell(dragTargetPosition) || mapService.GetCellIndex(dragStartPosition) == mapService.GetCellIndex(dragTargetPosition))
                {
                    dragResult = EntityDragResult.Failed;
                }
                else
                {
                    ReadOnlySpan<GameMapArea> placementAreas = mapService.EntityPlacementAreas;
                    bool destroyBuilding = true;

                    for (int i = 0; i < placementAreas.Length; i++)
                    {
                        if (placementAreas[i].ContainsPoint(dragTargetPosition))
                        {
                            destroyBuilding = false;
                            break;
                        }
                    }

                    if (destroyBuilding) dragResult = EntityDragResult.WillBeDestroyed;
                }

                entityDragUpdatedPublisher.Publish(new EntityDragUpdatedMessage(draggingBuilding, worldPointerPosition, dragTargetPosition, dragResult));
            }
        }

        private void EndDrag()
        {
            if (draggingBuilding == null) return;

            switch (dragResult)
            {
                case EntityDragResult.Success:
                    draggingBuilding.Position = dragTargetPosition;
                    mapService.MoveEntityToOtherCell(draggingBuilding, dragTargetPosition);
                    break;

                case EntityDragResult.WillBeDestroyed:
                    destroyBuildingPublisher.Publish(new DestroyBuildingCommand((IBuilding)draggingBuilding));
                    break;

                case EntityDragResult.Failed:
                    draggingBuilding.Position = dragStartPosition;
                    break;
            }

            enableBuildingBehaviourPublisher.Publish(draggingBuilding.InstanceId, new EnableBuildingBehaviourCommand());
            draggingBuilding = null;
            ResetDragCandidateState();
        }
        
        private void SetDragCandidateBuilding(IEntity newDragCandidate)
        {
            if (dragCandidateBuilding != newDragCandidate)
            {
                entityDragCandidateChangedPublisher.Publish(new EntityDragCandidateChangedMessage(dragCandidateBuilding, newDragCandidate));
                dragCandidateBuilding = newDragCandidate;
            }
        }
        
        private void ResetDragCandidateState()
        {
            SetDragCandidateBuilding(null);
            lastPointerCellIndex = int.MinValue;
        }

        public BuildingDragHandler
        (
            IGameMapService mapService,
            IBuildingRepository buildingRepository,
            ISubscriber<PlayerPointerMoveMessage> pointerMoveSubscriber,
            ISubscriber<EntityDragEndedMessage> entityDragEndedSubscriber,
            ISubscriber<StartBuildingConstructionModeCommand> startBuildingConstructionModeSubscriber,
            ISubscriber<CompleteBuildingConstructionModeCommand> completeBuildingConstructionModeSubscriber,
            IPublisher<EntityDragStartedMessage> entityDragStartedPublisher,
            IPublisher<EntityDragUpdatedMessage> entityDragUpdatedPublisher,
            IPublisher<EntityDragCandidateChangedMessage> entityDragCandidateChangedPublisher,
            IPublisher<DestroyBuildingCommand> destroyBuildingPublisher,
            IPublisher<int, EnableBuildingBehaviourCommand> enableBuildingBehaviourPublisher,
            IPublisher<int, DisableBuildingBehaviourCommand> disableBuildingBehaviourPublisher)
        {
            this.mapService = mapService;
            this.buildingRepository = buildingRepository;
            this.entityDragStartedPublisher = entityDragStartedPublisher;
            this.entityDragUpdatedPublisher = entityDragUpdatedPublisher;
            this.entityDragCandidateChangedPublisher = entityDragCandidateChangedPublisher;
            this.destroyBuildingPublisher = destroyBuildingPublisher;
            this.enableBuildingBehaviourPublisher = enableBuildingBehaviourPublisher;
            this.disableBuildingBehaviourPublisher = disableBuildingBehaviourPublisher;
            var disposableBuilder = DisposableBag.CreateBuilder();

            pointerMoveSubscriber
                .Subscribe(message => UpdateDrag(message.WorldPosition))
                .AddTo(disposableBuilder);

            entityDragEndedSubscriber
                .Subscribe(_ => EndDrag())
                .AddTo(disposableBuilder);

            startBuildingConstructionModeSubscriber
                .Subscribe(_ => isBuildingConstructionModeActive = true)
                .AddTo(disposableBuilder);
            
            completeBuildingConstructionModeSubscriber
                .Subscribe(_ => isBuildingConstructionModeActive = false)
                .AddTo(disposableBuilder);

            disposables = disposableBuilder.Build();
            ResetDragCandidateState();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        public StartEntityDragResponse Invoke(StartEntityDragRequest request)
        {
            if (dragCandidateBuilding != null)
            {
                draggingBuilding = dragCandidateBuilding;
                dragStartPosition = draggingBuilding.Position;
                SetDragCandidateBuilding(null);
                entityDragStartedPublisher.Publish(new EntityDragStartedMessage(draggingBuilding));
                disableBuildingBehaviourPublisher.Publish(draggingBuilding.InstanceId, new DisableBuildingBehaviourCommand());
                UpdateDrag(request.WorldPointerPosition);
                return new StartEntityDragResponse(draggingBuilding, true);
            }
            
            return new StartEntityDragResponse(null, false);
        }
    }
}