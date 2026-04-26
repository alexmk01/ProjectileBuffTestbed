using System;
using Common.Unity;
using Game.Core.Buildings;
using Game.Core.Construction.Commands;
using Game.Core.Entities;
using Game.Core.Interaction.Events;
using Game.Core.Interaction.Requests;
using Game.Core.Player.Controller;
using Game.Core.Player.Controller.Messages;
using MessagePipe;
using R3;
using ReactiveInputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Features.Player.Controller
{
    public sealed class PlayerController : IPlayerController, IInitializable, IDisposable
    {
        public System.Numerics.Vector2 PointerPosition => pointerPosition.ToNumerics();
        public System.Numerics.Vector2 WorldPointerPosition => worldPointerPosition.ToNumerics();

        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive == value) return;

                if (value)
                {
                    inputActions.Enable();
                }
                else
                {
                    inputActions.Disable();
                    CompleteBuildingConstruction();
                    CompleteEntityDrag();
                }

                isActive = value;
            }
        }
        
        private readonly PlayerInputActions inputActions;
        private readonly IRequestHandler<StartEntityDragRequest, StartEntityDragResponse> entityDragHandler;
        private readonly ISubscriber<StartBuildingConstructionModeCommand> constructionStartSubscriber;
        private readonly ISubscriber<CompleteBuildingConstructionModeCommand> constructionCompleteSubscriber;
        private readonly IPublisher<PlayerPointerMoveMessage> pointerMovePublisher;
        private readonly IPublisher<ConstructBuildingCommand> contructBuildingPublisher;
        private readonly IPublisher<CompleteBuildingConstructionModeCommand> constructionCompletePublisher;
        private readonly IPublisher<EntityDragEndedMessage> entityDragEndedPublisher;
        private Camera playerCamera;
        private Vector2 pointerPosition;
        private Vector2 worldPointerPosition;
        private BuildingId? constructionBuildingId;
        private IEntity draggingEntity;
        private bool suppressEntityDrag;
        private bool isActive;
        private IDisposable disposables;

        private void CompleteBuildingConstruction()
        {
            if (!constructionBuildingId.HasValue) return;
            constructionCompletePublisher.Publish(new CompleteBuildingConstructionModeCommand());
        }
        
        private void SuppressEntityDrag(bool suppress) => suppressEntityDrag = suppress;

        private void CompleteEntityDrag()
        {
            if (draggingEntity == null) return;
            entityDragEndedPublisher.Publish(new EntityDragEndedMessage(draggingEntity));
            draggingEntity = null;
        }
        
        public PlayerController
        (
            PlayerInputActions inputActions,
            IRequestHandler<StartEntityDragRequest, StartEntityDragResponse> entityDragMediator,
            ISubscriber<StartBuildingConstructionModeCommand> constructionStartSubscriber,
            ISubscriber<CompleteBuildingConstructionModeCommand> constructionCompleteSubscriber,
            IPublisher<PlayerPointerMoveMessage> pointerMovePublisher,
            IPublisher<ConstructBuildingCommand> contructBuildingPublisher,
            IPublisher<CompleteBuildingConstructionModeCommand> constructionCompletePublisher,
            IPublisher<EntityDragEndedMessage> entityDragEndedPublisher
        )
        {
            this.inputActions = inputActions;
            this.entityDragHandler = entityDragMediator;
            this.constructionStartSubscriber = constructionStartSubscriber;
            this.constructionCompleteSubscriber = constructionCompleteSubscriber;
            this.pointerMovePublisher = pointerMovePublisher;
            this.contructBuildingPublisher = contructBuildingPublisher;
            this.constructionCompletePublisher = constructionCompletePublisher;
            this.entityDragEndedPublisher = entityDragEndedPublisher;
        }

        public void Initialize()
        {
            playerCamera = Camera.main;
            var disposablesBuilder = Disposable.CreateBuilder();
            disposablesBuilder.Add(inputActions);
            PlayerInputActions.PlayerActions playerActions = inputActions.Player;

            Observable.EveryUpdate()
                .Where(_ => isActive && Mouse.current != null)
                .Select(_ => Mouse.current.position.ReadValue())
                .DistinctUntilChanged()
                .Subscribe(position =>
                {
                    pointerPosition = position;
                    worldPointerPosition = playerCamera.ScreenToWorldPoint(position);
                    pointerMovePublisher.Publish(new PlayerPointerMoveMessage(pointerPosition.ToNumerics(), worldPointerPosition.ToNumerics()));
                })
                .AddTo(ref disposablesBuilder);

            playerActions.ConstructBuilding.PerformedAsObservable()
                .Where(_ => constructionBuildingId.HasValue)
                .Subscribe(_ =>
                {
                    contructBuildingPublisher.Publish(new ConstructBuildingCommand(constructionBuildingId.Value, worldPointerPosition.ToNumerics()));
                    CompleteBuildingConstruction();
                    SuppressEntityDrag(true);
                })
                .AddTo(ref disposablesBuilder);
            
            playerActions.ConstructBuilding.CanceledAsObservable()
                .Subscribe(_ => SuppressEntityDrag(false))
                .AddTo(ref disposablesBuilder);

            playerActions.CancelBuildingConstruction.PerformedAsObservable()
                .Subscribe(_ => CompleteBuildingConstruction())
                .AddTo(ref disposablesBuilder);

            playerActions.StartDrag.PerformedAsObservable()
                .Where(_ => !suppressEntityDrag)
                .Subscribe(_ =>
                {
                    var request = new StartEntityDragRequest(worldPointerPosition.ToNumerics());
                    StartEntityDragResponse response = entityDragHandler.Invoke(request);
                    draggingEntity = response.CanBeDragged ? response.Entity : null;
                })
                .AddTo(ref disposablesBuilder);

            playerActions.StartDrag.CanceledAsObservable()
                .Subscribe(_ => CompleteEntityDrag())
                .AddTo(ref disposablesBuilder);
            
            constructionStartSubscriber.Subscribe(command => constructionBuildingId = command.BuildingId).AddTo(ref disposablesBuilder);
            constructionCompleteSubscriber.Subscribe(command => constructionBuildingId = null).AddTo(ref disposablesBuilder);
            disposables = disposablesBuilder.Build();
            IsActive = true;
        }
        
        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}