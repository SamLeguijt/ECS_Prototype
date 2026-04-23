using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class BulletPrefabAuthoring : MonoBehaviour
{
    public GameObject bulletPrefabSmall;
    public GameObject bulletPrefabLarge;

    private class BulletPrefabBaker : Baker<BulletPrefabAuthoring>
    {
        public override void Bake(BulletPrefabAuthoring authoring)
        {
            // Create the primary entity of the authored GO.
            Entity bulletContainer = GetEntity(TransformUsageFlags.None);

            BulletPrefabsComponent bulletEntityPrefabs = new BulletPrefabsComponent
            {
                smallBullet = GetEntity(authoring.bulletPrefabSmall, TransformUsageFlags.Dynamic),
                largeBullet = GetEntity(authoring.bulletPrefabLarge, TransformUsageFlags.Dynamic)
            };

            AddComponent(bulletContainer, new EntityCustomNameComponent { Name = "BulletContainer" });

            AddComponent(bulletContainer, bulletEntityPrefabs);
        }
    }
}
