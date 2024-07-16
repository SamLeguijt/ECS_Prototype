using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

namespace ECS_Prototyping
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class EntityPrefabSystem : SystemBase
    {
        private static Entity BulletPrefab;
        private static Entity TrailPrefab;
        private static Entity HexesPrefab;

        bool bulletsAreCreated = false;

        private static EntityManager manager;

        protected override void OnCreate()
        {
            base.OnCreate();

            // Assign static field reference.
            manager = EntityManager;

            bulletsAreCreated = false;
        }

        protected override void OnUpdate()
        {
            // Disable this system if values are assigned.
            if (bulletsAreCreated)
                this.Enabled = false;

            // Only get the container reference once in update
            if (!bulletsAreCreated && SystemAPI.TryGetSingleton(out EntitiesReferences container))
            {
                CreateBulletPrefabs(container);
            }
        }

        /// <summary>
        /// Creates entities for the static references of this class and assigns relevant components to them. <br/><br/>
        /// Uses the <paramref name="container"/> to get the converted GameObject prefabs as entities with their components and values.
        /// </summary>
        /// <param name="container"></param>
        private void CreateBulletPrefabs(EntitiesReferences container)
        {
            EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

            NativeList<Entity> bulletPrefabs = new NativeList<Entity>(Allocator.Temp)
                {
                    // Add and assign multiple empty entities to the list to iterate over.
                    (BulletPrefab = EntityManager.Instantiate(container.BulletVisualsPrefab)),
                    (HexesPrefab = EntityManager.Instantiate(container.HexesPrefab)),
                    (TrailPrefab = EntityManager.Instantiate(container.TrailPrefab))

                };

            // Iterate over the empty entities and add components.
            foreach (Entity emptyEntityPrefab in bulletPrefabs)
            {
                // Add Tag components
                EntityManager.AddComponentData(emptyEntityPrefab, new Prefab()); // This tag auto removes the earlier instantiated entity.
                EntityManager.AddComponent(emptyEntityPrefab, typeof(BulletPartTag));

                // Set initial name to recognize in the EntitiesHierarchy
                EntityManager.SetName(emptyEntityPrefab, $"BaseBulletPrefabPart [{emptyEntityPrefab.Index}]");

                // Add empty components without values to the prefab.
                EntityManager.AddComponent(emptyEntityPrefab, typeof(LocalTransform));
                EntityManager.AddComponent(emptyEntityPrefab, typeof(MoveForwardComponent));
                EntityManager.AddComponent(emptyEntityPrefab, typeof(LifetimeComponent));
                EntityManager.AddComponent(emptyEntityPrefab, typeof(EntityIdentifierComponent));
            }

            // Assign initial value to the BulletEntitie it's Entity references. 
            EntityManager.AddComponentData(BulletPrefab, new BulletParentComponent
            {
                TrailEntity = TrailPrefab,
                HexesEntity = HexesPrefab
            });

            ECB.Playback(EntityManager);
            ECB.Dispose();


            bulletPrefabs.Clear();
            bulletPrefabs.Dispose();

            bulletsAreCreated = true;
        }

        public static Entity GetBulletPrefab()
        {
            if (BulletPrefab != Entity.Null && manager.HasComponent(BulletPrefab, typeof(BulletPartTag)))
                return BulletPrefab;
            else
            {
                Debug.LogWarning("BulletPrefab entity is null, cannot return the baked prefab. Returning an empty entity!");
                return manager.CreateEntity();
            }
        }

        public static Entity GetTrailPrefab()
        {
            if (TrailPrefab != Entity.Null && manager.HasComponent(TrailPrefab, typeof(BulletPartTag)))
                return TrailPrefab;
            else
            {
                Debug.LogWarning("TrailPrefab entity is null, cannot return the baked prefab. Returning an empty entity!");
                return manager.CreateEntity();
            }
        }

        public static Entity GetHexesPrefab()
        {
            if (HexesPrefab != Entity.Null && manager.HasComponent(TrailPrefab, typeof(BulletPartTag)))
                return HexesPrefab;
            else
            {
                Debug.LogWarning("HexesPrefab entity is null, cannot return the baked prefab. Returning an empty entity!");
                return manager.CreateEntity();
            }
        }
    }
}