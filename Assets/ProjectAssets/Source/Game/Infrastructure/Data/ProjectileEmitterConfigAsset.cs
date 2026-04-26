using Common;
using Game.Core.Projectiles;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Infrastructure.Data
{
    [CreateAssetMenu]
    public sealed class ProjectileEmitterConfigAsset : ScriptableObject
    {
        public ProjectileEmitter.Parameters EmitterParameters = ProjectileEmitter.Parameters.Default;
        [SerializeReference, SubclassSelector]
        public IProjectilesRenderer ProjectilesRenderer;
        [SerializeReference, FormerlySerializedAs("ProjectilesModifiers"), SubclassSelector]
        public IProjectileProcessor[] ProjectileProcessors;
        
        public ProjectileEmitterConfig CreateConfig()
        {
            return new ProjectileEmitterConfig
            {
                EmitterParameters = EmitterParameters,
                ProjectilesRenderer = (IProjectilesRenderer)ProjectilesRenderer?.Clone(),
                ProjectileProcessors = ProjectileProcessors?.CloneArray()
            };
        }
    }
}