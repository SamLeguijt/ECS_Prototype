
using Unity.Entities;
using Unity.Collections;
using System.Security;

namespace ECS_Prototyping
{
    public partial class EntityLifetimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
            Entities
                .WithAll<LifetimeComponent>()
                .ForEach((Entity entity, ref LifetimeComponent lifetimeComponent) =>
                {
                    lifetimeComponent.MaxLifeSeconds -= SystemAPI.Time.DeltaTime;

                    if (lifetimeComponent.MaxLifeSeconds <= 0)
                    {
                        ECB.DestroyEntity(entity);
                    }
                })
                .WithoutBurst().Run();

            ECB.Playback(EntityManager);
        }
    }
}

