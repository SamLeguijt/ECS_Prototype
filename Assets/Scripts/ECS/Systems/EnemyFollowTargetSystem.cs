using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class EnemyFollowTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<EnemyTag>().ForEach((Entity entity, ref LocalTransform transform, ref FollowTargetComponent followComponent) =>
        {
            if (followComponent.FollowTarget == Entity.Null)
                followComponent.FollowTarget = ObjectEntitiesReferences.Instance.PlayerEntity;

            LocalTransform targetTrasform = EntityManager.GetComponentData<LocalTransform>(followComponent.FollowTarget);

            Vector3 distanceVector = targetTrasform.Position - transform.Position;
            distanceVector.y = 0;

            RotateTowardsTarget(ref transform, ref followComponent, distanceVector);
            MoveTowardsTarget(ref transform, ref followComponent, distanceVector);

        }).WithoutBurst().Run();
    }

    private void RotateTowardsTarget(ref LocalTransform enemyTransform, ref FollowTargetComponent followComponent, Vector3 entityToTargetVector)
    {
        if (entityToTargetVector.magnitude < followComponent.RotateDistanceThreshold)
        {
            Vector3 directionVector = entityToTargetVector.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionVector, enemyTransform.Up());

            enemyTransform.Rotation = targetRotation;
        }
    }

    private void MoveTowardsTarget(ref LocalTransform enemyTransform, ref FollowTargetComponent followComponent, Vector3 entityToTargetVector)
    {
        if (entityToTargetVector.magnitude < followComponent.MoveDistanceThreshold && entityToTargetVector.magnitude > followComponent.StoppingDistance)
        {
            float3 forward = math.forward(enemyTransform.Rotation);

            // Add movement
            enemyTransform.Position += forward * followComponent.MovementSpeed * SystemAPI.Time.DeltaTime;
        }

    }
}