using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace Farmer.Core
{
    //Use simple Physic2D.BoxCast to collider with static geometry. Won't handle other ColliderComponent as
    //those are pure float2 data and not part of PhysX;
    //TODO : rewrite all that to be pure ECS job? just simple intersection with box & circle should be "simple" enough?
    //This won't handle built-in thing like tilemap collider though sadly...
    class ColliderSystem : ComponentSystem
    {
        public struct Data
        {
            public int Length;
            public ComponentDataArray<Position> Position;
            public ComponentDataArray<Velocity> Velocity;
            [ReadOnly] public ComponentDataArray<Collider> Collider;
        }

        [Inject] private Data m_Data;

        private RaycastHit2D[] m_HitCache = new RaycastHit2D[16];

        protected override void OnUpdate()
        {
            const float shell = 0.005f;
            float dt = Time.deltaTime;

            for (int index = 0; index < m_Data.Length; ++index)
            {
                var position = m_Data.Position[index].Value;
                var velocity = m_Data.Velocity[index].Value;
                var collider = m_Data.Collider[index];

                float2 vel = velocity.xy;

                float2 velY = new float2(0, vel.y);
                //first do vertical velocity
                float dist = math.abs(velY.y * dt);
                velY = math.normalize(velY);
                if (dist > 0.0001f)
                {
                    //That's probably pretty bad cause won't be able to transform that multithread at some point
                    //but until we get an ECS simple physic system...
                    int count = Physics2D.BoxCastNonAlloc(position.xy + collider.Offset, collider.Size, 0, velY, m_HitCache, dist);

                    for (int c = 0; c < count; ++c)
                    {
                        RaycastHit2D hit = m_HitCache[c];
                        float2 cntNorm = hit.normal;

                        float proj = math.dot(vel, cntNorm);

                        if (proj < 0)
                            vel = vel - proj * cntNorm;

                        float modifDist = m_HitCache[c].distance - shell;
                        dist = modifDist < dist ? modifDist : dist;
                    }

                    position.xy = position.xy + velY * dist;
                }


                float2 velX = new float2(vel.x, 0);
                //then horizontal velocity
                dist = math.abs(velX.x * dt);
                velX = math.normalize(velX);
                if (dist > 0.0001f)
                {
                    int count = Physics2D.BoxCastNonAlloc(position.xy + collider.Offset, collider.Size, 0, velX, m_HitCache, dist);

                    for (int c = 0; c < count; ++c)
                    {
                        RaycastHit2D hit = m_HitCache[c];
                        float2 cntNorm = hit.normal;

                        float proj = math.dot(vel, cntNorm);

                        if (proj < 0)
                            vel = vel - proj * cntNorm;

                        float modifDist = m_HitCache[c].distance - shell;
                        dist = modifDist < dist ? modifDist : dist;
                    }

                    position.xy = position.xy + velX * dist;
                }

                m_Data.Position[index] = new Position { Value = position };
                m_Data.Velocity[index] = new Velocity { Value = new float3(vel, 0) };
            }
        }
    }
}
