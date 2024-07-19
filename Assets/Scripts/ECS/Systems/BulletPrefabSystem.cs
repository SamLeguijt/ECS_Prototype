using UnityEngine;
using Unity.Entities;
using Unity.Collections;

/// <summary> 
/// System that creates Entity bullet prefabs based on GO bullet references. System adds required components to the entities. 
/// <br/> Has static methods to get a prefab. 
/// 
/// <br/> <br/>Related classes: <br/>
/// <see cref="BulletPrefabAuthoring"/> that convert GO to entites <br/> 
/// <see cref="BulletPrefabsComponent"/> that holds references to the converted entities.
/// </summary>

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class BulletPrefabSystem : SystemBase
{
    private static Entity smallBulletPrefab; 
    private static Entity largeBulletPrefab;

    private BulletPrefabsComponent prefabContainer;

    private bool containerIsFound = false;
    private bool prefabsAreCreated = false;

    protected override void OnCreate()
    {
        prefabsAreCreated = false;

        // Returns if there is exactly one entity that has the out component. 
        // Which will be the case since we only have one BulletPrefabAuthoring which holds references to GO bullet prefabs.
        if (SystemAPI.TryGetSingleton(out BulletPrefabsComponent container))
        {
            prefabContainer = container;
        }
    }

    protected override void OnUpdate()
    {
        // Disable this system if values are assigned.
        if (prefabsAreCreated)
            this.Enabled = false;

        // OnCreate does not always find the reference, so also try in Update.
        if (!containerIsFound)
        {
            if (SystemAPI.TryGetSingleton(out BulletPrefabsComponent container))
            {
                prefabContainer = container;
                containerIsFound = true;
            }
        }

        // We only want this system to execute over a few frames, cause it just creates the entity prefabs.
        if (!prefabsAreCreated && containerIsFound)
        {
            // We create a NativeList so we can iterate over the 
            NativeList<Entity> bulletEntities = new NativeList<Entity>(Allocator.Temp)
            {
                prefabContainer.smallBullet,
                prefabContainer.largeBullet
            };

            for (int i = 0; i < bulletEntities.Length; i++)
            {
                // Note: Can't set an archetype to the entities because that would remove components that were added during the baking process.
                EntityManager.SetName(bulletEntities[i], "BulletPrefab_" + i);

                EntityManager.AddComponent(bulletEntities[i], typeof(BulletComponent));
                EntityManager.AddComponent(bulletEntities[i], typeof(MoveForwardComponent));
                EntityManager.AddComponent(bulletEntities[i], typeof(EntityCustomNameComponent));
                EntityManager.AddComponent(bulletEntities[i], typeof(LifetimeComponent));

                EntityManager.SetEnabled(bulletEntities[i], false);
            }

            bulletEntities.Clear();
            bulletEntities.Dispose();

            smallBulletPrefab = prefabContainer.smallBullet;
            largeBulletPrefab = prefabContainer.largeBullet;

            prefabsAreCreated = true;
        }
    }

    public static Entity GetSmallBulletPrefab()
    {
        if (smallBulletPrefab == Entity.Null)
            return Entity.Null;

        return smallBulletPrefab;
    }
    
    public static Entity GetLargeBulletPrefab()
    {
        if (largeBulletPrefab == Entity.Null)
            return Entity.Null;

        return largeBulletPrefab;
    }
}

