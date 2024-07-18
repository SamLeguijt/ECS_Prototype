using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private Transform firePoint = null;

    [SerializeField] private bool useEcsBullet;

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
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(firePoint.forward, firePoint.up));

        }
        else
        {

        }
    }
}
