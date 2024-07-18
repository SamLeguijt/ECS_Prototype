using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class BulletPrefabAuthoring : MonoBehaviour
{
    [SerializeField] private List<GameObject> bulletPrefabs = new List<GameObject>();
    [SerializeField] private List<BulletData> bulletData = new List<BulletData>();

    private class BulletPrefabBaker : Baker<BulletPrefabAuthoring>
    {
        public override void Bake(BulletPrefabAuthoring authoring)
        {
            // Create the primary entity of the authored GO.
            Entity bulletContainer = GetEntity(TransformUsageFlags.None);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetName(bulletContainer, "BulletContainer");
            
            // Create the component that the primary entity will hold for references to the bullet entity prefabs.
            BulletPrefabsComponent bulletEntityPrefabs = new BulletPrefabsComponent
            {
                // Convert the Authoring GO to entities and assign the indices to small/large bullet respectively.
                // Note: These entities have rendering components as well as oter components that were converted by baking process.
                // We still need to assign our bullet components to make them interact with our systems.
                smallBullet = GetEntity(authoring.bulletPrefabs[0], TransformUsageFlags.Dynamic),
                largeBullet = GetEntity(authoring.bulletPrefabs[1], TransformUsageFlags.Dynamic)
            };

            // Create a list so we can iterate over the entities (instead of having to write below code twice).
            NativeList<Entity> bulletEntities = new NativeList<Entity>()
            {
                bulletEntityPrefabs.smallBullet,
                bulletEntityPrefabs.largeBullet
            };

            // Iterate over the converted bullet entities, and assign the components needed to make them actual bullets.
            for (int i = 0; i < bulletEntities.Length; i++)
            {
                // First we create the components, and assign the value from the SO referenced in the inspector (Authoring)
                // Note: First index of the authoring lists refers to the smallBullet, second to the largeBullet. 
                // Seperate fields are possible but is a little more tedious.
                BulletComponent bulletComponent = new BulletComponent
                {
                    Damage = authoring.bulletData[i].Damage,
                    layerMask = authoring.bulletData[i].CollisionLayers
                };

                LifetimeComponent lifetimeComponent = new LifetimeComponent()
                {
                    CurrentLifeTime = 0,
                    MaxLifeTime = authoring.bulletData[i].Lifetime
                };

                MoveForwardComponent moveForwardComponent = new MoveForwardComponent()
                {
                    Speed = authoring.bulletData[i].Speed
                };

                EntityCustomNameComponent customNameComponent = new EntityCustomNameComponent()
                {
                    Name = authoring.bulletPrefabs[i].name
                };

                // Finally, assign the created components to the created bullet entities, with the index matching (0 == small, 1 == large).
                AddComponent(bulletEntities[i], bulletComponent);
                AddComponent(bulletEntities[i], lifetimeComponent);
                AddComponent(bulletEntities[i], moveForwardComponent);
                AddComponent(bulletEntities[i], customNameComponent);
            }

            // Finally, we need to set the component with the Prefab references on the primary entity from this baker:
            AddComponent(bulletContainer, bulletEntityPrefabs);

            bulletEntities.Clear(); 
            bulletEntities.Dispose();
        }
    }
}
