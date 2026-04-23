using Unity.Entities;
using Unity.Collections;

/// <summary> 
/// System that creates Entity enemy prefabs based on GO EnemyPrefab references. System adds required components to the entities. 
/// <br/> After adding the components <see cref="ObjectEntitiesReferences"/> sets the values based on predefined values.
/// 
/// <br/> <br/>Related classes: <br/>
/// <see cref="ObjectEntitiesReferences"/> sets the values for the Entity prefabs. <br/> 
/// <see cref="EnemyPrefabAuthoring"/> converts GO enemy prefabs to entities. <br/>
/// <see cref="EnemyPrefabComponent"/> holds references to the converted entities.
/// </summary>

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
            // Create a list so we can iterate over the enemies and add the same components.
            NativeList<Entity> enemyEntites = new NativeList<Entity>(Allocator.Temp)
            {
                (prefabContainer.basicEnemy),
                (prefabContainer.aggresiveEnemy),
                (prefabContainer.lurkingEnemy)
            };

            for (int i = 0; i < enemyEntites.Length; i++)
            {
                // Note: Can't set an archetype to the entities because that would remove components that were added during the baking process.
                EntityManager.SetName(enemyEntites[i], "EnemyPrefab_" + i);

                // Add all the components an enemy should have. This does NOT set any data values, but just adds the components themselves.
                EntityManager.AddComponent(enemyEntites[i], typeof(Prefab));
                EntityManager.AddComponent(enemyEntites[i], typeof(EnemyTag));
                EntityManager.AddComponent(enemyEntites[i], typeof(FollowTargetComponent));
                EntityManager.AddComponent(enemyEntites[i], typeof(EntityCustomNameComponent));
            }

            if (ObjectEntitiesReferences.Instance != null)
            {
                // We created the entity prefabs with the required components, but the components dont have any values set yet.
                // Because different enemies require different settings, we can now call a function that sets the values of the components based on predefined values (such as SO, Monobehavior fields etc.).
                ObjectEntitiesReferences.Instance.SetEnemyPrefabValues(prefabContainer.basicEnemy, prefabContainer.aggresiveEnemy, prefabContainer.lurkingEnemy);
            }

            // We should always dispose the NativeList 
            enemyEntites.Clear();
            enemyEntites.Dispose();

            prefabsAreCreated = true;
        }
    }
}

