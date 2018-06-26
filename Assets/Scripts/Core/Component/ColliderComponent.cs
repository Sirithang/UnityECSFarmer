using Unity.Entities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Farmer.Core
{
    class ColliderComponent : ComponentDataWrapper<Farmer.Core.Collider>
    {
        public void OnDrawGizmosSelected()
        {
            UnityEngine.Gizmos.DrawWireCube((UnityEngine.Vector2)transform.position + (UnityEngine.Vector2)this.Value.Offset, (UnityEngine.Vector2)this.Value.Size);
        }
    }
}
