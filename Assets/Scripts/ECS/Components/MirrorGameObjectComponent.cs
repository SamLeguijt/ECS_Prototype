using UnityEngine;
using Unity.Entities;

/// <summary>
/// This component is used to mirror certain properties of it's assigned targetGameObject. 
/// <br/> Example: Position and rotation updating to match the gameobject's position and rotation in <see cref="MirrorGameObjectOrientationSystem"/>
/// </summary>
public class MirrorGameObjectComponent : IComponentData
{
    public GameObject targetGameObject;
}
