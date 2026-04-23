using Unity.Entities;
using UnityEngine;

public struct FollowTargetComponent : IComponentData
{
    [field: SerializeField] public Entity FollowTarget {  get;  set; }
    [field: SerializeField] public float MovementSpeed { get; set; }
    [field: SerializeField] public float MoveDistanceThreshold { get; set; }    
    [field: SerializeField] public float RotateDistanceThreshold { get; set; }
    [field: SerializeField] public float StoppingDistance { get; set; }
}
