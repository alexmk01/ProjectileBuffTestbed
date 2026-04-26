using Game.Features.BuildingBehaviour.VFX;
using Game.Features.HitPoints.VFX;
using Game.Infrastructure.VFX;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    public sealed class GameVFXLifetimeScope : LifetimeScope
    {
        public EffectsDataAsset EntityDestructionEffectsData;
        public EffectsDataAsset BuildingBehaviourEffectsData;
        public GameObject DamageEffectsPresenterPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            new HitPointsVFXFeatureInstaller(EntityDestructionEffectsData, DamageEffectsPresenterPrefab).Install(builder);
            new BuildingBehaviourVFXFeatureInstaller(BuildingBehaviourEffectsData).Install(builder);
        }
    }
}
