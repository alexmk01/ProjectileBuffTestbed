using Common.Unity;
using Game.Core.Entities;
using Game.Core.Entities.Messages;
using Game.Core.HitPoints;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace Game.Infrastructure.Entities
{
    public class EntityMainComponent : MonoBehaviour, IEntity
    {
        public int InstanceId { get; internal set; }
        
        System.Numerics.Vector2 IEntity.Position
        {
            get => transform.position.ToNumericsVector2();
            set => transform.position = value.ToUnity();
        }
        
        public HitPointsState HitPointsState { get; internal set; }

        private IPublisher<EntityKilledMessage> killedMessagePublisher;
        private bool isKilled;

        [Inject]
        private void Construct(IPublisher<EntityKilledMessage> killedMessagePublisher)
        {
            this.killedMessagePublisher = killedMessagePublisher;
        }

        public void Kill()
        {
            if (isKilled) return;
            isKilled = true;
            Destroy(gameObject);
        }
        
        protected virtual void OnDestroy()
        {
            //Registry removal fallback
            killedMessagePublisher.Publish(new EntityKilledMessage(this));
        }
    }
}