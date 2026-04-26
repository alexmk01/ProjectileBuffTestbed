using System;
using Common.Unity;
using Game.Core.BuildingBehaviour.Messages;
using Game.Features.BuildingBehaviour.Behaviours;
using Game.Infrastructure.UI;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.Features.BuildingBehaviour.UI
{
    public sealed class BuildingBehaviourScreenPresenter : IInitializable, IDisposable
    {
        private readonly IDisposable disposables;

        private string GetBehaviourEffectMessage(object behaviourData, float effectAmount)
        {
            return behaviourData switch
            {
                ModifyProjectileDamageBuildingBehaviour.Data => $"+{effectAmount} Damage",
                ModifyProjectileHitCountBuildingBehaviour.Data => $"+{effectAmount} Pierce",
                CloneProjectileBuildingBehaviour.Data => $"Cloned",
                _ => string.Empty,
            };
        }
        
        public BuildingBehaviourScreenPresenter(ISubscriber<BuildingBehaviourEffectAppliedMessage> behaviourEffectAppliedSubscriber, IBuildingBehaviourScreenView view)
        {
            var disposablesBuilder = DisposableBag.CreateBuilder();
            Camera camera = Camera.main;
            
            behaviourEffectAppliedSubscriber.Subscribe(message =>
                {
                    Vector2 position = message.Building.Position.ToUnity();

                    if (view.Transform.TryTransformScreenToLocalPosition(camera, position, out Vector2 localPosition))
                    {
                        object data = message.Behaviour.BehaviourData;
                        view.ShowBuildingBehaviourEffectMessage(localPosition, GetBehaviourEffectMessage(data, message.EffectAmount));
                    }
                })
                .AddTo(disposablesBuilder);

            disposables = disposablesBuilder.Build();
        }
        
        public void Initialize()
        {
        }
        
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}