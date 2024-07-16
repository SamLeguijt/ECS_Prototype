using Unity.Entities;
using Unity.Collections;

namespace ECS_Prototyping
{
    public partial class EntityNamingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<EntityIdentifierComponent>()
                .WithChangeFilter<EntityIdentifierComponent>()
                .ForEach((Entity entity, in EntityIdentifierComponent nameComponent) =>
                {
                    FixedString64Bytes entityName = entity.ToFixedString();

                    if (entityName != nameComponent.Name)
                    {
                        EntityManager.SetName(entity, nameComponent.Name);
                    }
                })
                .WithoutBurst().Run();
        }
    }
}

