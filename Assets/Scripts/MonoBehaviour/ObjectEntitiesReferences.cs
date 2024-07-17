using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine.EventSystems;

public class ObjectEntitiesReferences : MonoBehaviour
{
    public static ObjectEntitiesReferences Instance = null;

    /* ----- ENTITY REFERENCES NAMES ----- */

    public const string PLAYER_ENTITY_NAME = "PlayerEntity";
    public const string ENEMY_ENTITY_BASE = "EnemyEntity_Variant_";


    /* ----- PROPERTIES ----- */

    public Dictionary<string, Entity> EntityReferences { get; private set; } = new Dictionary<string, Entity>();

    [field: SerializeField] public GameObject PlayerGO { get; private set; }
    public Entity PlayerEntity { get; private set; }


    /* ----- FIELDS ----- */

    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    private EntityManager entityManager;
    private EntityArchetype enemyArchetype;

    public static Entity enemyContainerEntity;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Instance = null;
        }

        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(Instance);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    private void Start()
    {
        CreatePlayerEntity();
        CreateEnemyEntities();
    }

    /// <summary>
    /// Adds an entity to the <see cref="EntityReferences"/> dictionary by a string as key and the related entity as value.
    /// <br/> <paramref name="customEntityName"/> Can be used to assign a
    /// </summary>
    /// <param name="relatedEntity"></param>
    /// <param name="customEntityName"></param>
    private void AddToDictionary(string enityNameKey, Entity relatedEntityValue, bool overrideCurrent = false)
    {
        if (EntityReferences.ContainsKey(enityNameKey))
        {
            if (!overrideCurrent)
                return;
            else
            {
                EntityReferences.Remove(enityNameKey);
            }
        }

        EntityReferences.Add(enityNameKey, relatedEntityValue);
    }

    public void SetPlayerEntity(Entity playerEntity, bool overrideCurrentPlayer = false)
    {
        PlayerEntity = playerEntity;

        AddToDictionary(PLAYER_ENTITY_NAME, PlayerEntity, overrideCurrentPlayer);
    }

    private void CreatePlayerEntity()
    {
        if (PlayerGO != null)
        {
            Entity playerEntity = entityManager.CreateSingleton<PlayerTag>();

            entityManager.AddComponentData(playerEntity, new EntityCustomNameComponent { Name = PLAYER_ENTITY_NAME });
            entityManager.AddComponentData(playerEntity, new LocalTransform { });
            entityManager.AddComponentData(playerEntity, new MirrorGameObjectComponent { targetGameObject = PlayerGO });

            SetPlayerEntity(playerEntity, true);
        }
    }

    private void CreateEnemyEntities()
    {
        if (enemyContainerEntity != null)
        {
            CreateEnemyEntityArchetype();

            EnemyPrefabComponent entityPrefabs = entityManager.GetComponentData<EnemyPrefabComponent>(enemyContainerEntity);

            Entity basicEnemyPrefab = entityManager.Instantiate(entityPrefabs.basicEnemy);
            entityManager.SetArchetype(basicEnemyPrefab, enemyArchetype);

            entityManager.SetComponentData(basicEnemyPrefab, new EntityCustomNameComponent { Name = "BasicEnemyEntity" });
            entityManager.SetComponentData(basicEnemyPrefab, new FollowTargetComponent
            {
                FollowTarget = PlayerEntity,
            });

                EntityReferences.Add("BasicEnemyEntity", basicEnemyPrefab);
            /*
                    for (int i = 0; i < enemyPrefabs.Count; i++)
                    {
                        enemyPrefabs[i].TryGetComponent(out EnemyPrefab enemy);

                        if (enemy != null)
                        {



                            Entity enemyEntity = entityManager.CreateEntity(enemyArchetype);

                            entityManager.SetComponentData(enemyEntity, new EntityCustomNameComponent { Name = enemy.Prefab.name });
                            entityManager.SetComponentData(enemyEntity, new FollowTargetComponent
                            {
                                FollowTarget = PlayerEntity,
                                MovementSpeed = enemy.Data.MovementSpeed,
                                MoveDistanceThreshold = enemy.Data.PlayerInRangeMoveThreshold,
                                RotateDistanceThreshold = enemy.Data.PlayerInRangeRotationThreshold,
                                StoppingDistance = enemy.Data.StoppingDistance,
                            });
                            entityManager.SetComponentData(enemyEntity, new LocalTransform { Scale = 1 });
                            Mesh Mesh = enemy.meshFilter.sharedMesh;
                            Material mat = enemy.renderers.sharedMaterial;

                            entityManager.SetSharedComponentManaged(enemyEntity, new RenderMesh { mesh = Mesh, material = mat });
            */
        }
    }
    

    private void CreateEnemyEntityArchetype()
    {
        enemyArchetype = entityManager.CreateArchetype(
            typeof(EnemyTag),
            typeof(LocalTransform),
            typeof(FollowTargetComponent),
            typeof(EntityCustomNameComponent),
            typeof(RenderMesh)
            );
    }
}
