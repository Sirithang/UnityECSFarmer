using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Farmer.Core;
using UnityEngine.AddressableAssets;

namespace Farmer.Gameplay
{
    public class PlayerStateSystem : ComponentSystem
    {
        //--- States

        readonly string[] WalkingAnimName = { "walk_u", "walk_l", "walk_d", "walk_r" };

        public struct PlayerMoveStateEntryTag : IComponentData { }
        public struct PlayerMoveStateTag : IComponentData { }

        public struct PlayerMoveStateEntry
        {
            public int Length;
            public EntityArray entities;
            public ComponentDataArray<PlayerInput> playerInput; //tag the entity as player
            public ComponentDataArray<PlayerMoveStateEntryTag> moveState; //tag as state
        }

        public struct PlayerMoveState
        {
            public int Length;
            public EntityArray entities;
            public ComponentDataArray<PlayerInput> playerInput;
            public ComponentDataArray<Velocity> velocity;
            public ComponentDataArray<PlayerMoveStateTag> moveState;
        }

        [Inject] PlayerMoveStateEntry _moveStateEntry;
        [Inject] PlayerMoveState _moveState;

        protected override void OnUpdate()
        {
            for (int i = 0; i < _moveStateEntry.Length; ++i)
            {
                Entity e = _moveStateEntry.entities[i];
                EntityManager.AddComponent(e, typeof(PlayerMoveStateTag));
                EntityManager.RemoveComponent(e, typeof(PlayerMoveStateEntryTag));
            }

            for (int  i = 0; i < _moveState.Length; ++i)
            {
                EntityManager manager = EntityManager;
                Entity e = _moveState.entities[i];
                float angle = Vector3.SignedAngle(new Vector3(1, 1, 0), math.normalize(_moveState.velocity[i].Value), new Vector3(0, 0, 1));

                //an angle 
                angle = angle < 0 ? 360 + angle : angle;

                int index = Mathf.Clamp(Mathf.FloorToInt(angle / 90), 0, 3);

                Addressables.LoadAsset<SpriteAnimations>("playerAnimation").Completed += op2 =>
                {
                    manager.SetSharedComponentData(e, op2.Result.GetAnimation(WalkingAnimName[index]));
                };
            }
        }
    }
}
