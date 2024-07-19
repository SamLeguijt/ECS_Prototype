using Unity.Entities;
using Unity.Transforms;

/// <summary>
/// Component used to move an Entity forward with a defined speed. 
/// <br/> <br/> Relevant classes:
/// <br/> <see cref="MovingForwardSystem"/> -> Queries for entities with this component to move them in the forward direction of their <seealso cref="LocalTransform"/>
/// <br/> <see cref="BulletPrefabSystem"/> -> Adds this component to Bullet entities
/// <br/> <see cref="PlayerController"/> -> Spawns a bullet and uses a corresponding <see cref="BulletData"/> SO to set the value of <see cref="Speed"/>
/// <br/> <see cref="ObjectEntitiesReferences"/> -> Referenced, but in unused method (Used to add this component to Bullets)
/// </summary>
public struct MoveForwardComponent : IComponentData
{
    public float Speed;
}
