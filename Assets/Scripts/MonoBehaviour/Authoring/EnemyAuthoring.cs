using Unity.Entities;
using UnityEngine;


public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private bool isMovableEnemy = true;
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float movementThreshold = 20f;
    [SerializeField] private float rotateThreshold = 25f;
    [SerializeField] private float stoppingDistance = 1f;

    // Baker class should be nested inside the Authoring (MonoBehaviour) class.
    public class EnemyPrefabBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            // Declare how the entity will be used: Movable? -> Dynamic. Static but visible? -> Renderable. Empty entity? -> None/
            TransformUsageFlags transformFlags = TransformUsageFlags.None;

            // Here we set the flags according to the setting on our athoring component (in the inspector).
            if (authoring.isMovableEnemy)
                transformFlags = TransformUsageFlags.Dynamic;
            else
                transformFlags = TransformUsageFlags.Renderable;

            // Create an entity with the TransformUsage flags.
            Entity enemyEntity = GetEntity(authoring.enemyPrefab, transformFlags);

            // If it is a movable enemy, we add a FollowTargetComponent to as behaviour for the entity. 
            if (authoring.isMovableEnemy)
            {
                // We set the movementspeed of the component to our authoring' s base speed, which is specified in the inspector.
                FollowTargetComponent followComponent = new FollowTargetComponent { 
                    MovementSpeed = authoring.baseMoveSpeed, 
                    MoveDistanceThreshold = authoring.movementThreshold, 
                    RotateDistanceThreshold = authoring.rotateThreshold,
                    StoppingDistance = authoring.stoppingDistance,
                };

                AddComponent(enemyEntity, followComponent);
            }

            // Finally, we add a EnemyTag to the entity to easily identify the entity from our systems.
            AddComponent(enemyEntity, new EnemyTag { });
            AddComponent(enemyEntity, new EntityCustomNameComponent { Name = "Enemy" });
        }
    }
}
