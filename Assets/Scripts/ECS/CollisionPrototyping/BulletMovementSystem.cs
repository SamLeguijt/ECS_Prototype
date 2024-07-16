using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

namespace ECS_Prototyping
{
    [BurstCompile]
    public partial class BulletMovementSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnUpdate()
        {
            foreach (var (transformComponent, moveComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MoveForwardComponent>>().WithAll<BulletPartTag>())
            {
                float3 forward = math.forward(transformComponent.ValueRW.Rotation);
                float3 up = math.mul(transformComponent.ValueRW.Rotation, new float3(0, 1, 0));

                // Add movement
                transformComponent.ValueRW.Position += moveComponent.ValueRO.Speed * SystemAPI.Time.DeltaTime * forward;

                //Debug.DrawRay(bulletTransform.ValueRO.Position, backwards, Color.green);
                //Debug.DrawRay(transformComponent.ValueRO.Position, up, Color.cyan);
            }
        }
    }
}
