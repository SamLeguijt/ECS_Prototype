using Unity.Entities;

namespace ECS_Prototyping
{
    // Empty on purpose (== Tag).
    public struct WallTag : IComponentData
    {
        public Entity hitEffect;
    }
}

