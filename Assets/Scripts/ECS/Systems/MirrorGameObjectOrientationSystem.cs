using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// This system updates the position and rotation of entities to match a GameOject specified in the entity's <see cref="MirrorGameObjectComponent.TargetGameObject"/>
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))] // <- Standard gameplay group 
public partial class MirrorGameObjectOrientationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((Entity entity, MirrorGameObjectComponent mirrorComponent, ref LocalTransform transform) =>
        {
            if (!mirrorComponent.ObjectMirrorsEntity)
            {
                transform.Position = mirrorComponent.TargetGameObject.transform.position;
                transform.Rotation = mirrorComponent.TargetGameObject.transform.rotation;
            }
            else
            {
                mirrorComponent.TargetGameObject.transform.position = transform.Position;
                mirrorComponent.TargetGameObject.transform.rotation = transform.Rotation;
            }

        }).Run();
    }
}
