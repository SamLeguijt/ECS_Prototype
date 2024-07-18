using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class BulletPrefabAuthoring : MonoBehaviour
{
    public GameObject bulletPrefabSmall;
    public GameObject bulletPrefabLarge;

    public List<BulletData> bulletDatas = new List<BulletData>();

    private class BulletPrefabBaker : Baker<BulletPrefabAuthoring>
    {
        public override void Bake(BulletPrefabAuthoring authoring)
        {
            // Create the primary entity of the authored GO.
            Entity bulletContainer = GetEntity(TransformUsageFlags.None);

            // Finally, we need to set the component with the Prefab references on the primary entity from this baker:
            AddComponent(bulletContainer, new EntityCustomNameComponent { Name = "BulletContainer" });

            Entity small = GetEntity(authoring.bulletPrefabSmall, TransformUsageFlags.Dynamic);
            Entity large = GetEntity(authoring.bulletPrefabLarge, TransformUsageFlags.Dynamic);


            BulletPrefabsComponent bulletEntityPrefabs = new BulletPrefabsComponent
            {
                smallBullet = small,
                largeBullet = large
            };

            AddComponent(bulletContainer, bulletEntityPrefabs);

            ObjectEntitiesReferences.CreateBullets(bulletEntityPrefabs, authoring.bulletDatas);
/*
            AddComponent(small, new EntityCustomNameComponent { Name = "Small" });
            AddComponent(large, new EntityCustomNameComponent { Name = "Large" });
*/

            /*Debug.Log("Make");
                // First we create the components, and assign the value from the SO referenced in the inspector (Authoring)
                // Note: First index of the authoring lists refers to the smallBullet, second to the largeBullet. 
                // Seperate fields are possible but is a little more tedious.
                BulletComponent bulletComponent = new BulletComponent
                {
                    Damage = authoring.smallData.Damage,
                    layerMask = authoring.smallData.CollisionLayers
                };

                LifetimeComponent lifetimeComponent = new LifetimeComponent()
                {
                    CurrentLifeTime = 0,
                    MaxLifeTime = authoring.smallData.Lifetime
                };

                MoveForwardComponent moveForwardComponent = new MoveForwardComponent()
                {
                    Speed = authoring.smallData.Speed
                };

                EntityCustomNameComponent customNameComponent = new EntityCustomNameComponent()
                {
                    Name = authoring.smallData.name
                };

                Debug.Log("AA");

                // Finally, assign the created components to the created bullet entities, with the index matching (0 == small, 1 == large).
                AddComponent(small, bulletComponent);
                AddComponent(small, lifetimeComponent);
                AddComponent(small, moveForwardComponent);
                AddComponent(small, customNameComponent);

                Debug.Log("BB");
*//*
                AddComponent(bulletEntityPrefabs.smallBullet, bulletComponent);
                AddComponent(bulletEntityPrefabs.smallBullet, lifetimeComponent);
                AddComponent(bulletEntityPrefabs.smallBullet, moveForwardComponent);
                AddComponent(bulletEntityPrefabs.smallBullet, customNameComponent);*//*
                Debug.Log("add");
*/

            //  }




        }
    }
}
