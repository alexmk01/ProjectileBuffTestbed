using Common.Unity;
using Common.Unity.Tags.Hierarchical;
using UnityEngine;

namespace Game.Infrastructure.Entities.Factory
{
    public class EntityDescriptionComponent : MonoBehaviour
    {
        [TagReference]
        public SerializableGUID Id;
        public float MaxHitPoints = 100f;
    }
}