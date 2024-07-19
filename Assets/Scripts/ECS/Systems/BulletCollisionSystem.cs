using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

[BurstCompile, RequireMatchingQueriesForUpdate]
public partial class BulletCollisionSystem : SystemBase
{
    // These events can be used to process ECS bullet collisions outside of the ECS world. 
    // For instance, a MonoBehaviour score system might want to know how many bullets were hit,
    // or you might want to get info about a certain entity that got hit.
    public delegate void BulletCollisionHandler(Entity bullet, RigidBody body, RaycastHit hitInfo, CollisionLayersEnum collidedLayer);
    public static event BulletCollisionHandler OnECSBulletCollisionEvent;

    [BurstCompile]
    protected override void OnUpdate()
    {
        // Entities.ForEach will run as a job, which prevents us from using assigned fields from the class in the foreach query loop. 
        // So, we need to get a reference to the PhysicsWorld and create a native queue each frame so we can use it in the foreach. 
        // Might seem expensive, but is the way to go and not so expensive as you might think (source: trust me bro).
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        var collisionResults = new NativeList<CollisionResult>(Allocator.TempJob);

        // Implicit job execution.
        Entities
            .WithAll<BulletComponent, LocalTransform>() // Only iterate over entities that have these components. 
            .ForEach((Entity entity, in BulletComponent bullet, in LocalTransform transform) => // Loop through all entities with the components, with in access (read0only access to the component).
            {
                /* RAY SETUP */

                var rayHits = new NativeList<RaycastHit>(Allocator.Temp);

                // What layers does the bullet belong to?
                uint bulletLayers = (uint)CollisionLayersEnum.Projectile;

                // We can either set collidable layers manually in here (A) or retrieve it from our bullet component (B):
                /* A */
                uint collidableLayersManual = (uint)CollisionLayersEnum.Environment | (uint)CollisionLayersEnum.Enemy | (uint)CollisionLayersEnum.Default;

                /* B */
                int layers = (int)bullet.layerMask;
                uint collidableLayersComponent = (uint)layers; // Cant convert LayerMask type to uint type directly.

                // Note:
                // We fire the ray BACKWARDS from the bullet's position. 
                // This seems to be the 'industry standard' as far as I could find online, and makes the most sense because: 
                // We want to know if a collision has occured during the frame, not if it will happen. 
                float3 rayDirection = -math.forward(transform.Rotation);


                // Create a rayInput struct to store some info for our raycasts.
                RaycastInput rayInput = new RaycastInput
                {
                    // Start from the bullet's LocalTransform position
                    Start = transform.Position,

                    // End of the ray is from the start towards the BACK of the bullet.
                    End = transform.Position + rayDirection,


                    // Collision filter for the rays.
                    Filter = new CollisionFilter
                    {
                        BelongsTo = bulletLayers,

                        // Set hittable layers based on previous defined option A or B.
                        CollidesWith = collidableLayersManual,
                    }
                };

                Debug.DrawLine(rayInput.Start, rayInput.End, Color.blue);

                /* COLLISION DETECTION */

                // Cast a ray based on our defined ray input.
                if (physicsWorld.CastRay(rayInput, ref rayHits))
                {
                    // We will want to detect the closest hitpoint of the ray: 

                    float closestHitDistance = float.MaxValue;
                    RaycastHit closestHit = default;

                    // Look in our ray hits to find the closest hit.
                    for (var i = 0; i < rayHits.Length; i++)
                    {
                        if (rayHits[i].Fraction < closestHitDistance)
                        {
                            closestHit = rayHits[i];
                            closestHitDistance = rayHits[i].Fraction;
                        }
                    }

                    // Store reference to he rigidbody that got hit.
                    RigidBody body = physicsWorld.Bodies[closestHit.RigidBodyIndex];

                    collisionResults.Add(new CollisionResult
                    {
                        Bullet = entity,
                        RayHit = closestHit,
                        BodyHit = physicsWorld.Bodies[closestHit.RigidBodyIndex]
                    });

                }

                rayHits.Dispose();
            })
            .WithReadOnly(physicsWorld)
            .Schedule();

        Dependency.Complete();

        for (int i = 0; i < collisionResults.Length; i++)
        {
            CollisionResult result = collisionResults[i];
            ProcessCollision(result.Bullet, result.RayHit, result.BodyHit);
        }

        collisionResults.Dispose();
    }


    private void ProcessCollision(Entity bullet, RaycastHit hit, RigidBody body)
    {
        Entity entityCollided = body.Entity;
        CollisionFilter hitCollisionFilter = body.Collider.Value.GetCollisionFilter();
        CollisionLayersEnum collidedLayer = GetLayerFromIndex(hitCollisionFilter.BelongsTo);

        OnECSBulletCollisionEvent?.Invoke(bullet, body, hit, collidedLayer);

        // We can process the collision in multiple ways, here are two examples: 

        // A) Based on the layer that got hit by the bullet. 
        switch (collidedLayer)
        {
            case CollisionLayersEnum.Default:
                RemoveBulletOnCollision(bullet);
                break;
            case CollisionLayersEnum.Projectile:
                break;
            case CollisionLayersEnum.Enemy:
                RemoveBulletOnCollision(bullet);
                break;
            case CollisionLayersEnum.Environment:
                RemoveBulletOnCollision(bullet);
                break;
            default:
                break;
        }

        // B) Find a certain component on the entity that got hit, like an enemy: 

        // Check if the Entity is an enemy by finding the EnemyTag component on the entity. 
        if (EntityManager.HasComponent<EnemyTag>(entityCollided))
        {
            RemoveBulletOnCollision(entityCollided);
        }
    }

    private void RemoveBulletOnCollision(Entity entity)
    {
        EntityManager.DestroyEntity(entity);
    }

    private CollisionLayersEnum GetLayerFromIndex(uint belongsTo)
    {
        // Check which layer the filter belongs to
        foreach (CollisionLayersEnum layer in System.Enum.GetValues(typeof(CollisionLayersEnum)))
        {
            if ((belongsTo & (uint)layer) != 0)
            {
                return layer;
            }
        }
        return CollisionLayersEnum.Default;
    }

    // Struct to define the result of a bullet collision.
    private struct CollisionResult
    {
        public Entity Bullet;
        public RaycastHit RayHit;
        public RigidBody BodyHit;
    }
}
