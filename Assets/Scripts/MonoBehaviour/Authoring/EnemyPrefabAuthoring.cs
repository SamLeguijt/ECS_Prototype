using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class EnemyPrefabAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject basicEnemyPrefab;
    [SerializeField] private GameObject aggresiveEnemyPrefab;
    [SerializeField] private GameObject lurkingEnemyPrefab;

    public class EnemyPrefabBaker : Baker<EnemyPrefabAuthoring>
    {
        public override void Bake(EnemyPrefabAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            EnemyPrefabComponent component = new EnemyPrefabComponent
            {
                basicEnemy = GetEntity(authoring.basicEnemyPrefab, TransformUsageFlags.Dynamic),
                aggresiveEnemy = GetEntity(authoring.aggresiveEnemyPrefab, TransformUsageFlags.Dynamic),
                lurkingEnemy = GetEntity(authoring.lurkingEnemyPrefab, TransformUsageFlags.Dynamic)
            };

            AddComponent(entity, new EntityCustomNameComponent { Name = "ContainerEntity"});
            AddComponent(entity, component);
        }
    }
}
