using UnityEngine;

namespace Game.Infrastructure.VFX
{
    [CreateAssetMenu]
    public sealed class EffectsDataAsset : ScriptableObject
    {
        public EffectPrefabData[] EffectsData;
    }
}