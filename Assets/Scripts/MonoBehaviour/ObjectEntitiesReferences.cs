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

    public const string PLAYER_ENTITY_KEY = "PlayerEntity";
    public const string BASIC_ENEMY_KEY = "BasicEnemy";
    public const string AGRO_ENEMY_KEY = "AgroEnemy";
    public const string LURK_ENEMY_KEY = "LurkEnemy";


    /* ----- PROPERTIES ----- */

    public Dictionary<string, Entity> EntityReferences { get; private set; } = new Dictionary<string, Entity>();

    [field: SerializeField] public GameObject PlayerGO { get; private set; }
    public Entity PlayerEntity { get; private set; }


    /* ----- FIELDS ----- */

    [SerializeField] private BaseEnemyData basicEnemyData = null;
    [SerializeField] private BaseEnemyData aggresiveEnemyData = null;
    [SerializeField] private BaseEnemyData lurkingEnemyData = null;

    private EntityManager entityManager;
    public EntityArchetype enemyArchetype;
    public static Entity enemyContainerEntity;

    public Entity basicEnemy;
    public Entity agroEnemy;
    public Entity lurkEnemy;

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
    
        StartCoroutine(SpawnWaves(10));
    }

    private IEnumerator SpawnWaves(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            yield return new WaitForSeconds(3);
            Debug.Log("Spawning wave");

            Entity basic = entityManager.Instantiate(EntityReferences[BASIC_ENEMY_KEY]);
            Entity agro = entityManager.Instantiate(EntityReferences[AGRO_ENEMY_KEY]);
            Entity lurk = entityManager.Instantiate(EntityReferences[LURK_ENEMY_KEY]);

            entityManager.SetEnabled(basic, true);
            entityManager.SetEnabled(agro, true);
            entityManager.SetEnabled(lurk, true);
        }
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
    
    /// <summary>
    /// Assigns <paramref name="playerEntity"/> to be the primary PlayerEntity to reference in other components and systems. 
    /// </summary>
    /// <param name="playerEntity"></param>
    /// <param name="overrideCurrentPlayerInDict"></param>
    public void SetPlayerEntity(Entity playerEntity)
    {
        PlayerEntity = playerEntity;

        AddToDictionary(PLAYER_ENTITY_KEY, PlayerEntity, true);
    }

    private void CreatePlayerEntity()
    {
        if (PlayerGO != null)
        {
            Entity playerEntity = entityManager.CreateSingleton<PlayerTag>();

            entityManager.AddComponentData(playerEntity, new EntityCustomNameComponent { Name = PLAYER_ENTITY_KEY });
            entityManager.AddComponentData(playerEntity, new LocalTransform { });
            entityManager.AddComponentData(playerEntity, new MirrorGameObjectComponent { targetGameObject = PlayerGO });

            SetPlayerEntity(playerEntity);
        }
    }

    public void CreateFunctionalEnemyEntityPrefabs(Entity basicEnemy, Entity agroEnemy, Entity lurkingEnemy)
    {
        // Note: 
        // Example of setting the data of each enemy entity to their respective SO data. 
        // When actually implementing a similar concept, should be made with enum of enemytype and list/dict or something similar instead of having to set values values per enemy as seen below. 

        if (basicEnemy != Entity.Null)
        {
            entityManager.SetComponentData(basicEnemy, new FollowTargetComponent
            {
                MovementSpeed = basicEnemyData.MovementSpeed,
                MoveDistanceThreshold = basicEnemyData.PlayerInRangeMoveThreshold,
                RotateDistanceThreshold = basicEnemyData.PlayerInRangeRotationThreshold,
                StoppingDistance = basicEnemyData.StoppingDistance
            });
            entityManager.SetComponentData(basicEnemy, new EntityCustomNameComponent { Name = BASIC_ENEMY_KEY });
            entityManager.SetEnabled(basicEnemy, false);

            AddToDictionary(BASIC_ENEMY_KEY, basicEnemy);
        }

        if (agroEnemy != Entity.Null)
        {
            entityManager.SetComponentData(agroEnemy, new FollowTargetComponent
            {
                MovementSpeed = aggresiveEnemyData.MovementSpeed,
                MoveDistanceThreshold = aggresiveEnemyData.PlayerInRangeMoveThreshold,
                RotateDistanceThreshold = aggresiveEnemyData.PlayerInRangeRotationThreshold,
                StoppingDistance = aggresiveEnemyData.StoppingDistance
            });

            entityManager.SetComponentData(agroEnemy, new EntityCustomNameComponent { Name = AGRO_ENEMY_KEY });
            entityManager.SetEnabled(agroEnemy, false);

            AddToDictionary(AGRO_ENEMY_KEY, agroEnemy);

        }

        if (lurkingEnemy != Entity.Null)
        {
            entityManager.SetComponentData(lurkingEnemy, new FollowTargetComponent
            {
                MovementSpeed = lurkingEnemyData.MovementSpeed,
                MoveDistanceThreshold = lurkingEnemyData.PlayerInRangeMoveThreshold,
                RotateDistanceThreshold = lurkingEnemyData.PlayerInRangeRotationThreshold,
                StoppingDistance = lurkingEnemyData.StoppingDistance
            });

            entityManager.SetComponentData(lurkingEnemy, new EntityCustomNameComponent { Name = LURK_ENEMY_KEY });
            entityManager.SetEnabled(lurkingEnemy, false);

            AddToDictionary(LURK_ENEMY_KEY, lurkingEnemy);
        }
    }
}