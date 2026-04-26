using System;
using System.Collections.Generic;
using Common.Unity;
using Game.Core.Entities;
using Game.Core.HitPoints;
using Game.Core.HitPoints.Events;
using Game.Infrastructure.Entities;
using Game.Infrastructure.UI;
using MessagePipe;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using VContainer.Unity;

namespace Game.Features.HitPoints.UI
{
    public sealed class HitPointsScreenPresenter : IInitializable, ILateTickable, IDisposable
    {
        private readonly IEntityRegistry entityRegistry;
        private readonly IHitPointsScreenView view;
        private readonly IDisposable disposables;
        private Camera camera;

        public HitPointsScreenPresenter
        (
            IEntityRegistry entityRegistry,
            ISubscriber<HitPointsChangedMessage> hitPointsChangedSubscriber,
            IHitPointsScreenView view
        )
        {
            this.entityRegistry = entityRegistry;
            this.view = view;
            var disposablesBuilder = DisposableBag.CreateBuilder();
            
            hitPointsChangedSubscriber.Subscribe(message =>
                {
                    float amount = message.Amount;
                    Assert.AreNotApproximatelyEqual(0f, amount);
                    Vector2 position = message.Entity.Position.ToUnity();
                    
                    if (view.Transform.TryTransformScreenToLocalPosition(camera, position, out Vector2 localPosition))
                    {
                        view.ShowHitPointsChangeMessage(localPosition, amount);
                    }
                })
                .AddTo(disposablesBuilder);

            disposables = disposablesBuilder.Build();
        }
        
        public void Initialize()
        {
            camera = Camera.main;
        }
        
        public void LateTick()
        {
            using (ListPool<(Vector2 Position, HitPointsState HPState)>.Get(out var hpModels))
            {
                IReadOnlyList<IEntity> entities = entityRegistry.Entities;
                RectTransform viewTransform = view.Transform;
                
                for (int i = 0; i < entities.Count; i++)
                {
                    HitPointsState hpState = entities[i].HitPointsState;

                    if (hpState != null && hpState.Max > 0f)
                    {
                        Vector3 entityPosition = entities[i].Position.ToVector3();

                        if (viewTransform.TryTransformScreenToLocalPosition(camera, entityPosition, out Vector2 localPosition))
                        {
                            hpModels.Add((localPosition, hpState));
                        }
                    }
                }

                view.QueryHitPointsViews(hpModels.Count, out ReadOnlySpan<IHitPointsView> hpViews);

                for (int i = 0; i < hpModels.Count; i++)
                {
                    (Vector2 position, HitPointsState hpState) = hpModels[i];
                    hpViews[i].UpdateView(position, Mathf.Floor(hpState.Current), hpState.Max);
                }
            }
        }
        
        public void Dispose()
        {
            disposables.Dispose();
        } 
    }
}