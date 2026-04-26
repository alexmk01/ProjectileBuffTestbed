using Game.Infrastructure.VFX;
using VContainer;
using VContainer.Unity;

namespace Game.Features.BuildingBehaviour.VFX
{
    public sealed class BuildingBehaviourVFXFeatureInstaller : IInstaller
    {
        private readonly EffectsDataAsset buildingBehaviourEffectsDataAsset;

        public BuildingBehaviourVFXFeatureInstaller(EffectsDataAsset buildingBehaviourEffectsDataAsset)
        {
            this.buildingBehaviourEffectsDataAsset = buildingBehaviourEffectsDataAsset;
        }
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BuildingBehaviourEffectSpawner>().WithParameter(buildingBehaviourEffectsDataAsset.EffectsData);
        }
    }
}