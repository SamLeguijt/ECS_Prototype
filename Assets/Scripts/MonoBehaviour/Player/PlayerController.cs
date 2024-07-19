using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using TMPro;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private Transform firePoint = null;
    [SerializeField] private BulletData smallBulletData  = null;
    [SerializeField] private BulletData largeBulletData  = null;

    [SerializeField] private bool useEcsBullet;


    EntityManager entityManager;
    Entity firepointEntity;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        firepointEntity = entityManager.CreateEntity(typeof(LocalTransform), typeof(MirrorGameObjectComponent), typeof(EntityCustomNameComponent));
        entityManager.SetComponentData(firepointEntity, new MirrorGameObjectComponent { targetGameObject = firePoint.gameObject, objectMirrorsEntity = false });
        entityManager.SetComponentData(firepointEntity, new EntityCustomNameComponent { Name = "FirepointEntity" });

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet(useEcsBullet);
        }
    }

    private void FireBullet(bool ecsBullet)
    {
        if (!ecsBullet)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(transform.forward));

        }
        else
        {
            Entity bulletPrefab = BulletPrefabSystem.GetSmallBulletPrefab();
            Entity bullet = entityManager.Instantiate(bulletPrefab);
            LocalTransform firepointTransform = entityManager.GetComponentData<LocalTransform>(firepointEntity);

            entityManager.SetComponentData(bullet, new MoveForwardComponent { Speed = smallBulletData.Speed });
            entityManager.SetComponentData(bullet, new LocalTransform { Position = firepointTransform.Position, Rotation = firepointTransform.Rotation,  Scale = 1});;

            entityManager.SetEnabled(bullet, true);
        }
    }
}
