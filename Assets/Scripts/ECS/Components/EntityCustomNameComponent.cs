
using Unity.Collections;
using Unity.Entities;

/// <summary>
/// This component is used to give an Entity a custom name, visible in the EntitiesHierarchy for identification. 
/// <br/> The unmanaged type <see cref="FixedString32Bytes"/> is used instead of the managed 'string' type, and allows up to 16 characters. 
/// <br/> This component is used in the following system: <see cref="EntityCustomNamingSystem"/> to apply the name to entities in the first few frames of Update.
/// </summary>
public struct EntityCustomNameComponent : IComponentData
{
    public FixedString32Bytes Name;
    public bool IsApplied;
}
