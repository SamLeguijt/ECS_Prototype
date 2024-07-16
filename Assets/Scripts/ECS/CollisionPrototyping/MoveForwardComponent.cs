using Unity.Entities;

namespace ECS_Prototyping
{
    public struct MoveForwardComponent : IComponentData
    {
        public float Speed { get; set; }
    }
}

