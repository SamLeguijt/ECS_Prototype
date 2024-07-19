using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Moves entities in their forward direction while rotating them towards it.
/// <br/> <br/> Relevant files:
/// <br/> <see cref="MoveForwardComponent"/> -> Queries entities that have this component to move them with a speed of <seealso cref="MoveForwardComponent.Speed"/>
/// </summary>

[BurstCompile]
[RequireMatchingQueriesForUpdate]
public partial class MovingForwardSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref LocalTransform transform, in MoveForwardComponent moveData) =>
        {
            float3 forward = math.forward(transform.Rotation);
            float3 targetDirection = math.normalize(forward); 

            quaternion targetRotation = quaternion.LookRotationSafe(targetDirection, math.up());
       
            // Add movement to the entity.
            transform.Position += moveData.Speed * SystemAPI.Time.DeltaTime * forward;

            // Add rotation to the entity.
            transform.Rotation = math.slerp(transform.Rotation, targetRotation, SystemAPI.Time.DeltaTime * moveData.Speed);

        }).Schedule();
    }
}
