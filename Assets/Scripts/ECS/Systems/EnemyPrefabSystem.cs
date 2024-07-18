using UnityEngine;
using Unity.Entities;
using Unity.Collections;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class EnemyPrefabSystem : SystemBase
{
    private bool prefabsAreCreated = false;

    private EnemyPrefabComponent prefabContainer;
    private bool containerIsFound = false;

    protected override void OnCreate()
    {
        prefabsAreCreated = false;


        if (SystemAPI.TryGetSingleton(out EnemyPrefabComponent container))
        {
            prefabContainer = container;
        }
    }

    protected override void OnUpdate()
    {
        // Disable this system if values are assigned.
        if (prefabsAreCreated)
            this.Enabled = false;

        if (!containerIsFound)
        {
            if (SystemAPI.TryGetSingleton(out EnemyPrefabComponent container))
            {
                prefabContainer = container;
                containerIsFound = true;
            }
        }

        // Only get the container reference once in update
        if (!prefabsAreCreated && containerIsFound)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity basicEnemy;
            Entity aggesiveEnemy;
            Entity lurkingEnemy;

            // Create a list so we can iterate over the enemies and add the same components
            NativeList<Entity> enemyEntites = new NativeList<Entity>(Allocator.Temp)
            {
                // Assign the fields and add to the list simultaneously.
               ( basicEnemy = EntityManager.Instantiate(prefabContainer.basicEnemy)),
                ( aggesiveEnemy = EntityManager.Instantiate(prefabContainer.aggresiveEnemy)),
                ( lurkingEnemy = EntityManager.Instantiate(prefabContainer.lurkingEnemy)),
            };

            for (int i = 0; i < enemyEntites.Length; i++)
            {
                // Note: Can't set an archetype to the entities because that would remove components that were added during the baking process.
                EntityManager.SetName(enemyEntites[i], "Enemy_" + i);

                EntityManager.AddComponent(enemyEntites[i], typeof(EnemyTag));
                EntityManager.AddComponent(enemyEntites[i], typeof(FollowTargetComponent));
                EntityManager.AddComponent(enemyEntites[i], typeof(EntityCustomNameComponent));
            }

            if (ObjectEntitiesReferences.Instance != null)
            {
                ObjectEntitiesReferences.Instance.CreateFunctionalEnemyEntityPrefabs(basicEnemy, aggesiveEnemy, lurkingEnemy);
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();

            enemyEntites.Clear();
            enemyEntites.Dispose();

            prefabsAreCreated = true;
        } 
    }
}
