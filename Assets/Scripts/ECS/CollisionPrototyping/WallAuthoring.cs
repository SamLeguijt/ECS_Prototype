using Unity.Entities;
using UnityEngine;


namespace ECS_Prototyping
{
    public class WallAuthoring : MonoBehaviour
    {
        public GameObject hitPrefab;

        public class WallBaker : Baker<WallAuthoring>
        {
            public override void Bake(WallAuthoring authoring)
            {
                Entity wallEntity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(wallEntity, new WallTag
                {
                    hitEffect = GetEntity(authoring.hitPrefab, TransformUsageFlags.Renderable)
                });
            }
        }
    }
}

