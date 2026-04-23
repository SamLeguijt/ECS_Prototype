using Unity.Collections;
using Unity.Entities;

public struct BulletPrefabsComponent : IComponentData
{
    public Entity smallBullet;
    public Entity largeBullet;
}
