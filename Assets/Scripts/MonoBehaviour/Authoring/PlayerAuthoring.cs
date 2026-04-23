using UnityEngine;
using Unity.Entities;

public class PlayerAuthoring : MonoBehaviour
{
    private class PlayerBakery : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            //Entity playerEntity = GetEntity(authoring.player, TransformUsageFlags.Dynamic);
            Entity playerEntity = GetEntity(authoring,TransformUsageFlags.Dynamic);

            AddComponent(playerEntity, new PlayerTag { } );
            AddComponent(playerEntity, new EntityCustomNameComponent { Name = "TESTEmtoy"} );

            Debug.Log("Baked");
           // ObjectEntitiesReferences.Instance.SetPlayer(playerEntity);
        }
    }
}
