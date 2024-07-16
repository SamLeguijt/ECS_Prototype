using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;
using Unity.Transforms;

namespace ECS_Prototyping
{
    [BurstCompile]
    public partial class BulletCollisionSystem : SystemBase
    {
        PhysicsWorldSingleton physicsWorld;
        EntityCommandBuffer entityCommandBuffer;

        public delegate void BulletCollisionHandler(Entity bullet, RigidBody body, Unity.Physics.RaycastHit hitInfo, CollisionLayerECS collidedLayer);
        public static event BulletCollisionHandler OnBulletCollisionEvent;

        [BurstCompile]
        protected override void OnUpdate()
        {
            physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            NativeArray<Entity> allEntities = EntityManager.GetAllEntities();

            foreach (Entity entity in allEntities)
            {
                // Only look for entites with LocalTransform and BulletParentComponent components.
                if (EntityManager.HasComponent<LocalTransform>(entity) && EntityManager.HasComponent<BulletParentComponent>(entity))
                {
                    Entity bulletEntity = entity;
                    LocalTransform bulletTransform = EntityManager.GetComponentData<LocalTransform>(bulletEntity);

                    bool hasCollided = false;

                    var Hits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);

                    float3 direction = -math.forward(bulletTransform.Rotation);

                    RaycastInput rayInput = new RaycastInput
                    {
                        Start = bulletTransform.Position,
                        End = bulletTransform.Position + direction,

                        Filter = new CollisionFilter
                        {
                            BelongsTo = (uint)CollisionLayerECS.Projectile,
                            CollidesWith = (uint)CollisionLayerECS.Environment | (uint)CollisionLayerECS.Enemy | (uint)CollisionLayerECS.Default,
                        }
                    };

                    Debug.DrawLine(rayInput.Start, rayInput.End, Color.blue);

                    if (physicsWorld.CastRay(rayInput, ref Hits))
                    {
                        float closestHitDistance = float.MaxValue;

                        // Assigning an initial value
                        Unity.Physics.RaycastHit hit = Hits[0];

                        for (var i = 0; i < Hits.Length; i++)
                        {
                            if (Hits[i].Fraction < closestHitDistance)
                            {
                                hit = Hits[i];
                                closestHitDistance = Hits[i].Fraction;
                            }
                        }

                        Hits.Dispose();

                        var body = physicsWorld.Bodies[hit.RigidBodyIndex];

                        ProcessCollision(bulletEntity, hit, body, hasCollided);
                        hasCollided = true;
                    }
                }
            }

            entityCommandBuffer.Playback(EntityManager);
            entityCommandBuffer.Dispose();
        }


        private void ProcessCollision(Entity bullet, Unity.Physics.RaycastHit hit, RigidBody body, bool hasCollided, bool useLayers = true)
        {
            Entity entityCollided = body.Entity;
            // Apply Dev setting
            if (useLayers)
            {
                /* Using collision layers */

                CollisionFilter hitCollisionFilter = body.Collider.Value.GetCollisionFilter();

                CollisionLayerECS physicsLayer = GetLayerFromIndex(hitCollisionFilter.BelongsTo);

                if (!hasCollided)
                    OnBulletCollisionEvent?.Invoke(bullet, body, hit, physicsLayer);

                switch (physicsLayer)
                {
                    case CollisionLayerECS.Default:
                        Debug.Log("Default layer hit...");
                        RemoveBulletOnCollision(bullet);

                        break;
                    case CollisionLayerECS.Projectile:
                        Debug.Log("Projectile layer hit...");

                        break;
                    case CollisionLayerECS.Enemy:
                        Debug.Log("Enemy layer hit...");
                        RemoveBulletOnCollision(bullet);
                        break;
                    case CollisionLayerECS.Environment:
                        Debug.Log("Environment layer hit...");
                        RemoveBulletOnCollision(bullet);
                        break;
                    default:
                        Debug.Log("Nothing layer hit...");
                        break;
                }
            }
            else
            {
                /* Using Entity components */

                if (EntityManager.HasComponent<WallTag>(entityCollided))
                {
                    Debug.Log("Environment layer hit...");

                    RemoveBulletOnCollision(entityCollided);
                }
            }
        }

        private void RemoveBulletOnCollision(Entity entity)
        {
            BulletParentComponent bulletData = EntityManager.GetComponentData<BulletParentComponent>(entity);

            entityCommandBuffer.DestroyEntity(bulletData.TrailEntity);
            entityCommandBuffer.DestroyEntity(bulletData.HexesEntity);

            entityCommandBuffer.DestroyEntity(entity);
        }

        private CollisionLayerECS GetLayerFromIndex(uint belongsTo)
        {
            // Check which layer the filter belongs to
            foreach (CollisionLayerECS layer in System.Enum.GetValues(typeof(CollisionLayerECS)))
            {
                if ((belongsTo & (uint)layer) != 0)
                {
                    return layer;
                }
            }
            return CollisionLayerECS.Default;
        }
    }
}