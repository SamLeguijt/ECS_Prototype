
using Unity.Collections;
using Unity.Entities;

public struct EntityCustomNameComponent : IComponentData
{
    public FixedString128Bytes Name;
    public bool IsApplied;
}
