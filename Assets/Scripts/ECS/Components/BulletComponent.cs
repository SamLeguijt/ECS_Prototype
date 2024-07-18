using Unity.Entities;
using UnityEngine;

public struct BulletComponent : IComponentData
{
    public float Damage;
    public LayerMask layerMask; 
}
