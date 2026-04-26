using System;
using Game.Core.Entities;
using Game.Core.HitPoints;
using Game.Core.HitPoints.Commands;
using Game.Core.HitPoints.Events;
using MessagePipe;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

namespace Game.Features.HitPoints
{
    public sealed class HitPointsComponent : MonoBehaviour
    {
        private IEntity entity;
        private IHitPointsController hitPointsController;
        private IPublisher<HitPointsChangedMessage> changeMessagePublisher;
        private IPublisher<HitPointsExhaustedMessage> exhaustedMessagePublisher;
        private IDisposable disposables;

        [Inject]
        private void Construct
        (
            IEntity entity,
            IHitPointsController hitPointsController,
            ISubscriber<ChangeHitPointsCommand> changeCommandSubscriber,
            IPublisher<HitPointsChangedMessage> changeMessagePublisher,
            IPublisher<HitPointsExhaustedMessage> exhaustedMessagePublisher
        )
        {
            Assert.IsNotNull(entity?.HitPointsState, name);
            this.entity = entity;
            this.hitPointsController = hitPointsController;
            this.changeMessagePublisher = changeMessagePublisher;
            this.exhaustedMessagePublisher = exhaustedMessagePublisher;
            var disposablesBuilder = DisposableBag.CreateBuilder();
            changeCommandSubscriber.Subscribe(ModifyHitPoints).AddTo(disposablesBuilder);
            disposables = disposablesBuilder.Build();
        }
          
        private void PublishHitPointsChanged(float lastHP)
        {
            HitPointsState hpState = entity.HitPointsState;
            changeMessagePublisher.Publish(new HitPointsChangedMessage(entity, lastHP, hpState.Current, hpState.Max));
        }
        
        private void ModifyHitPoints(ChangeHitPointsCommand command)
        {
            if (command.EntityId != entity.InstanceId) return;
            HitPointsState hpState = entity.HitPointsState;
            float lastHP = hpState.Current;

            if (command.Amount > 0f)
            {
                if (hitPointsController.ApplyHealing(hpState, command.Amount))
                {
                    PublishHitPointsChanged(lastHP);
                }
            }
            else if (hitPointsController.ApplyDamage(hpState, -command.Amount))
            {
                PublishHitPointsChanged(lastHP);
            }

            if (hpState.Current <= 0f)
            {
                exhaustedMessagePublisher.Publish(new HitPointsExhaustedMessage(entity));
                entity.Kill();
            }
        }

        private void OnDestroy()
        {
            disposables?.Dispose();
        }
    }
}