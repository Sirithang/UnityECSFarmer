using Unity.Entities;
using Unity.Collections;
using UnityEngine;

namespace Farmer.Core
{
    public struct PlayerInput : IComponentData
    { }

    class PlayerInputSystem : ComponentSystem
    {
        public struct Group
        {
            public int Length;
            [ReadOnly] public ComponentDataArray<PlayerInput> PlayerInput;
            [WriteOnly] public ComponentDataArray<Velocity> Velocity;
        }

        [Inject] private Group m_Data;

        protected override void OnUpdate()
        {
            for (int index = 0; index < m_Data.Length; ++index)
            {
                //When new Input System will be around, could index all those lookup with an index set in the PlayerInput component to handle multiple pads?

                m_Data.Velocity[index] = new Velocity { Value = new Unity.Mathematics.float3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * 10.0f };
            }
        }
    }
}
