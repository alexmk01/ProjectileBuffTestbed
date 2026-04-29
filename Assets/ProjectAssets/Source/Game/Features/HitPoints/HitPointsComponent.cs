using System;
using Game.Core.Entities;
using Game.Core.HitPoints;
using Game.Core.HitPoints.Commands;
using Game.Core.HitPoints.Events;
using Game.Core.LifeCycle;
using MessagePipe;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

namespace Game.Features.HitPoints
{
    public sealed class HitPointsComponent : MonoBehaviour
    {
        private IEntity entity;
        private IHitPointsService hitPointsService;
        private IPublisher<HitPointsChangedMessage> changeMessagePublisher;
        private IPublisher<HitPointsExhaustedMessage> exhaustedMessagePublisher;
        private IDisposable disposables;

        [Inject]
        private void Construct
        (
            IEntity entity,
            IHitPointsService hitPointsService,
            ISubscriber<ChangeHitPointsCommand> changeCommandSubscriber,
            IPublisher<HitPointsChangedMessage> changeMessagePublisher,
            IPublisher<HitPointsExhaustedMessage> exhaustedMessagePublisher
        )
        {
            Assert.IsNotNull(entity?.HitPointsState, name);
            this.entity = entity;
            this.hitPointsService = hitPointsService;
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
                if (hitPointsService.ApplyHealing(hpState, command.Amount))
                {
                    PublishHitPointsChanged(lastHP);
                }
            }
            else if (hitPointsService.ApplyDamage(hpState, -command.Amount))
            {
                PublishHitPointsChanged(lastHP);
            }

            if (hpState.Current <= 0f)
            {
                exhaustedMessagePublisher.Publish(new HitPointsExhaustedMessage(entity));
                (entity as IKillable )?.Kill();
            }
        }

        private void OnDestroy()
        {
            disposables?.Dispose();
        }
    }
}