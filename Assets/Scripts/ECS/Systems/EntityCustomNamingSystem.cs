using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial class EntityCustomNamingSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = true;
    }

    protected override void OnUpdate()
    {
        int entitiesQueried = 0;
        int entitiesApplied = 0;

        Entities.WithoutBurst().ForEach((Entity entity, ref EntityCustomNameComponent customName) =>
        {
            entitiesQueried++;

            if (!customName.IsApplied)
            {
                if (customName.Name.Length > 0)
                {

                    EntityManager.SetName(entity, customName.Name.ToString());
                    customName.IsApplied = true;
                    entitiesApplied++;
                }
            }
        }).Run();
    
        if (entitiesQueried > 0 && entitiesApplied == entitiesQueried)
        {
            Enabled = false;
        }
    }
}
