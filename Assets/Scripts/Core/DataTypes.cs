using Unity.Entities;
using Unity.Mathematics;

namespace Farmer.Core
{
    [System.Serializable]
    public struct Collider : IComponentData
    {
        public float2 Offset;
        public float2 Size;
    }

    public struct Velocity : IComponentData
    {
        public float3 Value;
    }
}
