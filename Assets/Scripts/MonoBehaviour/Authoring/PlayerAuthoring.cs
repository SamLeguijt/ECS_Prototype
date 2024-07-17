using UnityEngine;
using Unity.Entities;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private class PlayerBakery : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity playerEntity = GetEntity(authoring.player, TransformUsageFlags.Dynamic);

            AddComponent(playerEntity, new PlayerTag { } );
            AddComponent(playerEntity, new EntityCustomNameComponent { Name = ObjectEntitiesReferences.PLAYER_ENTITY_NAME} );

            Debug.Log("Baked");
            ObjectEntitiesReferences.Instance.SetPlayer(authoring.player, playerEntity);
        }
    }
}
