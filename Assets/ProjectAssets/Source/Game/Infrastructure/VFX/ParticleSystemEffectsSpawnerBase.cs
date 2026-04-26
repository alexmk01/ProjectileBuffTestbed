using Common.Tags;
using Common.Unity;
using Common.Unity.Tags.Hierarchical;
using UnityEngine;

namespace Game.Infrastructure.VFX
{
    public abstract class ParticleSystemEffectsSpawnerBase
    {
        private readonly EffectPrefabData[] effectsData;
        
        protected ParticleSystemEffectsSpawnerBase(EffectPrefabData[] effectsData)
        {
            this.effectsData = effectsData;
        }
        
        protected void SpawnEffect(in Tag effectId, Vector2 position)
        {
            if (!effectId.HasValue()) return;
            SerializableGUID effectGuid = effectId.ToGuid();
            
            for (int i = 0; i < effectsData.Length; i++)
            {
                if (effectsData[i].Id == effectGuid)
                {
                    GameObject effectObject = Object.Instantiate(effectsData[i].EffectPrefab);
                    
                    if (effectObject.TryGetComponent(out ParticleSystem particleSystem))
                    {
                        particleSystem.transform.position = position;
                        particleSystem.Stop();
                        ParticleSystem.MainModule mainModule = particleSystem.main;
                        mainModule.loop = false;
                        mainModule.stopAction = ParticleSystemStopAction.Destroy;
                        particleSystem.Play();
                    }
                    else
                    {
                        Common.DebugUtils.LogError($"Failed to spawn effect {effectId}.");
                        Object.Destroy(effectObject);
                    }

                    return;
                }
            }
        }
     }
}