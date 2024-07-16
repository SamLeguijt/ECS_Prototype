using Unity.Entities;

namespace ECS_Prototyping
{
    public struct EntitiesReferences : IComponentData
    {
        public Entity BulletVisualsPrefab;
        public Entity TrailPrefab;
        public Entity HexesPrefab;
    }
}
