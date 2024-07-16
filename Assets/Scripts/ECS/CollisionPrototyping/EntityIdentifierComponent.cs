using Unity.Entities;
using Unity.Collections;

namespace ECS_Prototyping
{
    public struct EntityIdentifierComponent : IComponentData
    {
        public FixedString64Bytes Name { get; set; }
    }
}
