using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class MovingForwardSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref LocalTransform transform, in MoveForwardComponent moveData) =>
        {
            float3 forward = math.forward(transform.Rotation);
            float3 up = math.mul(transform.Rotation, new float3(0, 1, 0));

            float3 targetDirection = math.normalize(forward); // Assuming forward is the target direction

            float3 backwards = -forward;
            // Add movement
            transform.Position += moveData.Speed * SystemAPI.Time.DeltaTime * forward;



            // Compute the new rotation
            quaternion targetRotation = quaternion.LookRotationSafe(targetDirection, math.up());

            // Apply the new rotation
            transform.Rotation = math.slerp(transform.Rotation, targetRotation, SystemAPI.Time.DeltaTime * moveData.Speed);


            Debug.DrawRay(transform.Position, backwards, Color.red);

        }).Schedule();
    }
}
