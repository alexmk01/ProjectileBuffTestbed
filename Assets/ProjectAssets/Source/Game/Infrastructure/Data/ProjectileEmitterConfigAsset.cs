using System;
using Common;
using Game.Core.Projectiles;
using UnityEngine;

namespace Game.Infrastructure.Data
{
    [CreateAssetMenu]
    public sealed class ProjectileEmitterConfigAsset : ScriptableObject
    {
        public ProjectileEmitter.Parameters EmitterParameters = ProjectileEmitter.Parameters.Default;
        [SerializeReference, SubclassSelector]
        public IProjectilesRenderer ProjectilesRenderer;
        [SerializeReference, SubclassSelector]
        public IProjectilesBehaviour[] ProjectilesModifiers;
        
        public ProjectileEmitterConfig CreateConfig()
        {
            return new ProjectileEmitterConfig
            {
                EmitterParameters = EmitterParameters,
                ProjectilesRenderer = (IProjectilesRenderer)ProjectilesRenderer?.Clone(),
                ProjectilesBehaviours = ProjectilesModifiers?.CloneArray()
            };
        }
    }
}