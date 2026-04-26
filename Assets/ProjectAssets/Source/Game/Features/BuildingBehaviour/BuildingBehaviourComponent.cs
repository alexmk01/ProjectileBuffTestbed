using System;
using Game.Core.BuildingBehaviour;
using Game.Core.BuildingBehaviour.Commands;
using Game.Core.BuildingBehaviour.Factory;
using Game.Core.Buildings;
using Game.Core.Entities;
using MessagePipe;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

namespace Game.Features.BuildingBehaviour
{
    public sealed class BuildingBehaviourComponent : MonoBehaviour
    {
        private IBuildingBehaviour behaviour;
        private IDisposable disposables;
        
        [Inject]
        private void Construct
        (
            IEntity entity,
            IBuildingBehaviourFactory factory,
            IBuildingBehaviourData behaviourData,
            ISubscriber<int, EnableBuildingBehaviourCommand> enableSubscriber,
            ISubscriber<int, DisableBuildingBehaviourCommand> disableSubscriber
        )
        {
            var building = (IBuilding)entity;
            Assert.IsNotNull(building);
            behaviour = factory.CreateBehaviour(new BuildingBehaviourFactoryArgs(building, behaviourData));
            var disposableBuilder = DisposableBag.CreateBuilder();
            enableSubscriber.Subscribe(building.InstanceId, _ => enabled = true).AddTo(disposableBuilder);
            disableSubscriber.Subscribe(building.InstanceId, _ => enabled = false).AddTo(disposableBuilder);
            if (behaviour is IDisposable disposableBehaviour) disposableBehaviour.AddTo(disposableBuilder);
            disposables = disposableBuilder.Build();
        }

        private void OnEnable()
        {
            if (behaviour != null) behaviour.IsActive = true;
        }

        private void Start()
        {
            behaviour?.Initialize();
        }

        private void OnDisable()
        {
            if (behaviour != null) behaviour.IsActive = false;
        }

        private void OnDestroy()
        {
            disposables?.Dispose();
        }
    }
}