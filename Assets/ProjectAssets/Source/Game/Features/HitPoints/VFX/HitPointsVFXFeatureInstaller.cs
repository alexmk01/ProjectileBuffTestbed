using Game.Infrastructure.VFX;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.HitPoints.VFX
{
    public sealed class HitPointsVFXFeatureInstaller : IInstaller
    {
        private readonly EffectsDataAsset destructionEffectsDataAsset;
        private readonly GameObject damageEffectsPresenterPrefab;
        
        public HitPointsVFXFeatureInstaller(EffectsDataAsset destructionEffectsDataAsset, GameObject damageEffectsPresenterPrefab)
        {
            this.destructionEffectsDataAsset = destructionEffectsDataAsset;
            this.damageEffectsPresenterPrefab = damageEffectsPresenterPrefab;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntityDesctructionEffectsSpawner>().WithParameter(destructionEffectsDataAsset.EffectsData);
            builder.RegisterComponentInNewPrefab(damageEffectsPresenterPrefab.GetComponent<EntityDamageEffectsPresenter>(), Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}