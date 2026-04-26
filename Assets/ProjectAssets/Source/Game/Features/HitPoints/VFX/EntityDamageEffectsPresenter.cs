using System;
using Game.Core.HitPoints.Events;
using LitMotion;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.HitPoints.VFX
{
    public sealed class EntityDamageEffectsPresenter : MonoBehaviour, IInitializable
    {
        public Color DamageEffectColor = Color.orange;
        public float DamageEffectDuration = 1f;
        
        private IDisposable disposables;
        
        [Inject]
        private void Construct(ISubscriber<HitPointsChangedMessage> hitPointsChangedSubscriber)
        {
            var disposableBuilder = DisposableBag.CreateBuilder();

            hitPointsChangedSubscriber
                .Subscribe(message =>
                {
                    if (message.Amount < 0f && !Mathf.Approximately(message.Current, 0f))
                    {
                        if (((Component)message.Entity).TryGetComponent(out SpriteRenderer renderer))
                        {
                            LMotion.Create(renderer.color, DamageEffectColor, DamageEffectDuration)
                                .WithLoops(2, LoopType.Yoyo)
                                .WithEase(Ease.InOutQuad)
                                .Bind(color => renderer.color = color);
                        }
                    }
                })
                .AddTo(disposableBuilder);

            disposables = disposableBuilder.Build();
        }
        
        public void Initialize()
        {
        }
        
        private void OnDestroy()
        {
            disposables?.Dispose();
        }
    }
}