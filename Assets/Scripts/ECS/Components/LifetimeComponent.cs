using Unity.Entities;

public struct LifetimeComponent : IComponentData
{
    public float CurrentLifeTime;
    public float MaxLifeTime;
}
