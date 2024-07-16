using Unity.Entities;

namespace ECS_Prototyping
{
    public struct LifetimeComponent : IComponentData
    {
        public float MaxLifeSeconds;
    }
}

