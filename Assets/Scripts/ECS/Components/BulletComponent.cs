using Unity.Entities;
using UnityEngine;

/// <summary>
/// Component used for Bullet entities 
/// <br/> <br/> Relevant classes: 
/// <br/> <see cref="BulletPrefabSystem"/> -> Adds this component to the pre baked bullet entity prefab. 
/// <br/> <see cref="BulletCollisionSystem"/> -> Queries for entities with this component to handle and process collisions.
/// </summary>
public struct BulletComponent : IComponentData
{
    public float Damage;
    public LayerMask HittableLayers; 
}
