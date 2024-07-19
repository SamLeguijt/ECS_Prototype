using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using TMPro;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private CurrentBullet selectedBulletPrefab;
    [SerializeField] private bool useECSBullet = true;

    [Header("References")]
    [SerializeField] private Transform firePoint = null;

    [Header("Bullet references")]
    [SerializeField] private GameObject smallBulletPrefab = null;
    [SerializeField] private GameObject largeBulletPrefab = null;
    [SerializeField] private BulletData smallBulletData = null;
    [SerializeField] private BulletData largeBulletData = null;

    EntityManager entityManager;
    Entity firepointEntity;

    Entity smallBulletEntityPrefab;
    Entity largeBulletEntityPrefab;
    Entity currentSelection;


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
            FireBullet();
        }
    }

    private void FireBullet()
    { 
        // Make sure references are assigned (Cant be done in start, then returns null).
        if (smallBulletEntityPrefab == Entity.Null)
            smallBulletEntityPrefab = BulletPrefabSystem.GetSmallBulletPrefab();
        
        if (largeBulletEntityPrefab == Entity.Null)
            largeBulletEntityPrefab = BulletPrefabSystem.GetLargeBulletPrefab();

        if (useECSBullet)
        {
            switch (selectedBulletPrefab)
            {
                case CurrentBullet.SmallBullet:
                    currentSelection = smallBulletEntityPrefab;
                    break;
                case CurrentBullet.LargeBullet:
                    currentSelection = largeBulletEntityPrefab;
                    break;
                default:
                    break;
            }

            Entity bullet = entityManager.Instantiate(currentSelection);
            LocalTransform firepointTransform = entityManager.GetComponentData<LocalTransform>(firepointEntity);

            entityManager.SetComponentData(bullet, new MoveForwardComponent { Speed = smallBulletData.Speed });
            entityManager.SetComponentData(bullet, new LocalTransform { Position = firepointTransform.Position, Rotation = firepointTransform.Rotation, Scale = 1 }); ;

            entityManager.SetEnabled(bullet, true);
        }
        else
        {
            // TODO: Fix normal bullet rotation.

            GameObject bulletObject;
            Bullet bullet;

            switch (selectedBulletPrefab)
            {
                case CurrentBullet.SmallBullet:
                    bulletObject = Instantiate(smallBulletPrefab, firePoint.position, firePoint.rotation);
                    bullet = bulletObject.GetComponent<Bullet>();
                    bullet.Init(smallBulletData);
                    break;
                case CurrentBullet.LargeBullet:
                    bulletObject = Instantiate(largeBulletPrefab, firePoint.position, firePoint.rotation);
                    bullet = bulletObject.GetComponent<Bullet>();
                    bullet.Init(largeBulletData);
                    break;
                default:
                    break;
            }
        }
    }

    private enum CurrentBullet
    {
        SmallBullet,
        LargeBullet
    }
}
