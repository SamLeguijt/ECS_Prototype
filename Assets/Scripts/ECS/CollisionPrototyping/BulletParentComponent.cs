using Unity.Entities;

namespace ECS_Prototyping
{
    public struct BulletParentComponent : IComponentData
    {
        public Entity TrailEntity { get; set; }
        public Entity HexesEntity { get; set; }
    }

}
