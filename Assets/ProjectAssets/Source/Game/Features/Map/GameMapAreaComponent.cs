using Common.Unity;
using Game.Core.Map;
using UnityEngine;

namespace Game.Features.Map
{
    public sealed class GameMapAreaComponent : MonoBehaviour
    {
        public GameMapArea Area
        {
            get
            {
                Vector2 min = AreaMinPivot.position;
                Vector2 max = AreaMaxPivot.position;
                return new(Vector2.Min(min, max).ToNumerics(), Vector2.Max(min, max).ToNumerics());
            }
        }

        public Transform AreaMinPivot;
        public Transform AreaMaxPivot;

        [ContextMenu(nameof(CreatePivots))]
        private void CreatePivots()
        {
            if (AreaMinPivot == null)
            {
                AreaMinPivot = new GameObject("_MinPivot").transform;
                AreaMinPivot.parent = transform;
                AreaMinPivot.localPosition = Vector3.zero;
            }
            
            if (AreaMaxPivot == null)
            {
                AreaMaxPivot = new GameObject("_MaxPivot").transform;
                AreaMaxPivot.parent = transform;
                AreaMaxPivot.localPosition = new Vector3(1f, 1f, 0f);
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(transform);
#endif
        }

        private void Reset()
        {
            CreatePivots();
        }

        private void OnDrawGizmos()
        {
            if (AreaMinPivot != null && AreaMaxPivot != null)
            {
                GameMapArea area = Area;
                Vector3 center = area.Center.ToVector3();
                center.z = transform.position.z;
                Vector3 size = area.Size.ToVector3();
                
                Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
                Gizmos.DrawCube(center, size);
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}