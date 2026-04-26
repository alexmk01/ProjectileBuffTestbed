using System;
using Common.Unity;
using Common.Unity.Tags.Hierarchical;
using UnityEngine;

namespace Game.Infrastructure.VFX
{
    [Serializable]
    public struct EffectPrefabData
    {
        [TagReference]
        public SerializableGUID Id;
        public GameObject EffectPrefab;
    }
}