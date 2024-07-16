using UnityEngine;
using Unity.Entities;

namespace ECS_Prototyping
{
    public class EntitiesReferencesAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject bulletVisualsPrefab;
        [SerializeField] private GameObject trailPrefab;
        [SerializeField] private GameObject hexesPrefab;

        public class EntitiesReferencesBaker : Baker<EntitiesReferencesAuthoring>
        {
            public override void Bake(EntitiesReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                EntitiesReferences component = new EntitiesReferences
                {
                    BulletVisualsPrefab = GetEntity(authoring.bulletVisualsPrefab, TransformUsageFlags.Dynamic),
                    TrailPrefab = GetEntity(authoring.trailPrefab, TransformUsageFlags.Dynamic),
                    HexesPrefab = GetEntity(authoring.hexesPrefab, TransformUsageFlags.Dynamic)
                };

                AddComponent(entity, component);
            }
        }
    }

}
