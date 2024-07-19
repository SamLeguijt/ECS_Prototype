using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[BurstCompile, RequireMatchingQueriesForUpdate]
public partial class LifetimeDepletionSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        // Schedule the job and get the handle
        JobHandle jobHandle = Entities.ForEach((Entity entity, ref LifetimeComponent lifetime) =>
        {
            if (lifetime.CurrentLifeTime > lifetime.MaxLifeTime)
            {
                ecb.DestroyEntity(entity);
            }

            lifetime.CurrentLifeTime += SystemAPI.Time.DeltaTime;

        }).Schedule(Dependency); // Pass the current Dependency to ensure proper scheduling
    
        // Ensure all jobs are completed before playing back the ECB
        jobHandle.Complete();

        // Playback the ECB after the job has completed
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
