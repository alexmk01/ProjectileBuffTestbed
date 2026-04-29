using Game.Core.BuildingBehaviour;
using Game.Core.Projectiles;
using UnityEngine;

namespace Game.Features.BuildingBehaviour
{
    public sealed class BuildingBehaviourDataComponent : MonoBehaviour, IProjectileEmitterConfigProvider
    {
        ProjectileEmitterConfig IProjectileEmitterConfigProvider.ProjectileEmitterConfig
        {
            get => (BehaviourData as IProjectileEmitterConfigProvider)?.ProjectileEmitterConfig;
        }
        
        [SerializeReference, SubclassSelector]
        public IBuildingBehaviourData BehaviourData;
    }
}