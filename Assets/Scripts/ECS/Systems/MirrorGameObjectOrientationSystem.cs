using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// This system updates the position and rotation of entities to match a GameOject specified in the entity's <see cref="MirrorGameObjectComponent.targetGameObject"/>
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))] // <- Standard gameplay group 
public partial class MirrorGameObjectOrientationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((Entity entity, MirrorGameObjectComponent mirrorComponent, ref LocalTransform transform) =>
        {
            if (!mirrorComponent.objectMirrorsEntity)
            {
                transform.Position = mirrorComponent.targetGameObject.transform.position;
                transform.Rotation = mirrorComponent.targetGameObject.transform.rotation;
            }
            else
            {
                mirrorComponent.targetGameObject.transform.position = transform.Position;
                mirrorComponent.targetGameObject.transform.rotation = transform.Rotation;
            }

        }).Run();
    }
}
