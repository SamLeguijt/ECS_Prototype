using System.Dynamic;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This system is used to give Entities that have a <see cref="EntityCustomNameComponent"/> a name visible in the hierarchy. Name value is set in the <see cref="EntityCustomNameComponent.Name"/>
/// <br/> System gets disabled after applying a name to each entity with the component, which will happen within a few Update frames.
/// </summary>
/// 
public partial class EntityCustomNamingSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = true;
    }

    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((Entity entity, ref EntityCustomNameComponent customName) =>
        {
            if (!customName.IsApplied)
            {
                if (customName.Name.Length > 0)
                {
                    EntityManager.SetName(entity, customName.Name.ToString());
                    customName.IsApplied = true;    
                }
            }

        }).Run();
    }
}
