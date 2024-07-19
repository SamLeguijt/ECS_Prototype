using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [Header("Firing Settings")]
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private float delayBetweenShots = 0.5f;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private bool fireSingleShot = true;

    [Header("Enemy Wave settings")]
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();
    [SerializeField] private float timeBetweenWaves = 5;
    [SerializeField] private int amountPerWave = 1;
    [SerializeField] private int totalWaves = 100;
    [Space, SerializeField] private bool enableWaves = false;

    [Header("Single Enemy spawns")]
    [SerializeField] private int amount = 1;
    [SerializeField] private bool spawnWave = false;

    Coroutine continuousWaves = null;
    Coroutine singleWave = null;
    Coroutine firingRoutine = null;

    EntityManager entityManager;

    private bool isMouseDown;



    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int i = 0; i < enemySpawnPoints.Count; i++)
        {
            enemySpawnPoints[i].position = new Vector3(enemySpawnPoints[i].position.x, 1, enemySpawnPoints[i].position.z);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            StartFire(fireSingleShot);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (firingRoutine != null)
                firingRoutine = null;

            isMouseDown = false;
        }


        if (spawnWave)
        {
            SpawnEnemyWave();
        }

        if (enableWaves)
        {
            StartWaves();
        }
        else
        {

            if (continuousWaves != null)
            {
                StopCoroutine(SpawnEnemyWavesRoutine());
                continuousWaves = null;
            }
        }
    }

    private void StartFire(bool fireSingleShot)
    {
        if (fireSingleShot)
        {
            playerController.Fire(bulletsPerShot);
        }
        else
        {
            if (firingRoutine == null)
            {
                firingRoutine = StartCoroutine(FireBulletsRoutine());
            }
        }
    }

    private IEnumerator FireBulletsRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(delayBetweenShots);

        while (isMouseDown)
        {
            playerController.Fire(bulletsPerShot);

            yield return delay;
        }
    }


    private void StartWaves()
    {
        if (continuousWaves == null)
            continuousWaves = StartCoroutine(SpawnEnemyWavesRoutine());
    }

    private void SpawnEnemyWave()
    {
        if (singleWave == null)
            singleWave = StartCoroutine(SpawnSingleWave());
    }

    private IEnumerator SpawnSingleWave()
    {
        for (int i = 0; i < amount; i++)
        {
            Entity basic = entityManager.Instantiate(ObjectEntitiesReferences.Instance.EntityReferences[ObjectEntitiesReferences.BASIC_ENEMY_KEY]);
            Entity agro = entityManager.Instantiate(ObjectEntitiesReferences.Instance.EntityReferences[ObjectEntitiesReferences.AGRO_ENEMY_KEY]);
            Entity lurk = entityManager.Instantiate(ObjectEntitiesReferences.Instance.EntityReferences[ObjectEntitiesReferences.LURK_ENEMY_KEY]);

            entityManager.SetComponentData(basic, new LocalTransform { Position = enemySpawnPoints[GetRandomSpawnPointIndex()].position, Scale = 1 });
            entityManager.SetComponentData(agro, new LocalTransform { Position = enemySpawnPoints[GetRandomSpawnPointIndex()].position, Scale = 1 });
            entityManager.SetComponentData(lurk, new LocalTransform { Position = enemySpawnPoints[GetRandomSpawnPointIndex()].position, Scale = 1 });

            entityManager.SetEnabled(basic, true);
            entityManager.SetEnabled(agro, true);
            entityManager.SetEnabled(lurk, true);
        }

        yield return new WaitForSeconds(1f);

        spawnWave = false;
        singleWave = null;
    }

    private IEnumerator SpawnEnemyWavesRoutine()
    {
        for (int i = 0; i < totalWaves; i++)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            for (int j = 0; j < amountPerWave; j++)
            {
                Entity basic = entityManager.Instantiate(ObjectEntitiesReferences.Instance.EntityReferences[ObjectEntitiesReferences.BASIC_ENEMY_KEY]);
                Entity agro = entityManager.Instantiate(ObjectEntitiesReferences.Instance.EntityReferences[ObjectEntitiesReferences.AGRO_ENEMY_KEY]);
                Entity lurk = entityManager.Instantiate(ObjectEntitiesReferences.Instance.EntityReferences[ObjectEntitiesReferences.LURK_ENEMY_KEY]);

                entityManager.SetComponentData(basic, new LocalTransform { Position = enemySpawnPoints[GetRandomSpawnPointIndex()].position, Scale = 1 });
                entityManager.SetComponentData(agro, new LocalTransform { Position = enemySpawnPoints[GetRandomSpawnPointIndex()].position, Scale = 1 });
                entityManager.SetComponentData(lurk, new LocalTransform { Position = enemySpawnPoints[GetRandomSpawnPointIndex()].position, Scale = 1 });

                entityManager.SetEnabled(basic, true);
                entityManager.SetEnabled(agro, true);
                entityManager.SetEnabled(lurk, true);
            }
        }
    }

    private int GetRandomSpawnPointIndex()
    {
        int maxCount = enemySpawnPoints.Count;
        int random = Random.Range(0, maxCount);

        return random;
    }
}
