using Unity.Entities;

public struct FollowTargetComponent : IComponentData
{
    public Entity FollowTarget {  get;  set; }
    public float MovementSpeed { get; set; }
}
