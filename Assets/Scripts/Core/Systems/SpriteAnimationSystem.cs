using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Farmer.Core
{
    public struct AnimationStateData : IComponentData
    {
        public float CurrentTime;
        public int CurrentSprite;
        public float Step;
    }

    class SpriteAnimationSystem : ComponentSystem
    {
        //this will "filter" all object that had a  Animation component added but still don't hold state
        public struct Added
        {
            public int Length;
            public EntityArray Entities;
            [ReadOnly] public SharedComponentDataArray<SpriteAnimations.Animation> Animation;
            [ReadOnly] public ComponentArray<SpriteRenderer> Renderers;
            [ReadOnly] public SubtractiveComponent<AnimationStateData> missing;
        }

        public struct Data
        {
            public int Length;
            public EntityArray Entities;
            [ReadOnly] public SharedComponentDataArray<SpriteAnimations.Animation> Animation;
            [ReadOnly] public ComponentArray<SpriteRenderer> Renderers;
            public ComponentDataArray<AnimationStateData> States;
        }

        [Inject] private Added m_Added;
        [Inject] private Data m_Data;

        protected override void OnUpdate()
        {
            //Process all that were added
            for (int index = 0; index < m_Added.Length; ++index)
            {
                var anim = m_Added.Animation[index];
                PostUpdateCommands.AddComponent(m_Added.Entities[index], new AnimationStateData
                {
                    CurrentTime = Random.Range(0, anim.time),
                    CurrentSprite = 0,
                    Step = anim.time / anim.sprites.Length
                });
            }


            float dt = Time.deltaTime;

            for (int index = 0; index < m_Data.Length; ++index)
            {
                var state = m_Data.States[index];
                var anim = m_Data.Animation[index];

                state.CurrentTime += dt;

                if (state.CurrentTime > state.Step * state.CurrentSprite)
                {
                    state.CurrentSprite = (state.CurrentSprite + 1) % anim.sprites.Length;

                    if (state.CurrentSprite == 0) // we looped, reduce the current time
                        state.CurrentTime -= anim.time;

                    m_Data.Renderers[index].sprite = anim.sprites[state.CurrentSprite];
                }

                m_Data.States[index] = state;
            }
        }
    }
}
