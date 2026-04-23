using Unity.Entities;
using UnityEngine;

public partial class AnimationSystem : SystemBase
{
    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, AnimationComponent animationComponent) =>
        {
            Debug.Log(entity + " Has animation component");

            if (animationComponent.RequestPlay)
            {
                Debug.Log("Play");
                animationComponent.animator.Play(animationComponent.animationName.ToString());
            }


        }).WithoutBurst().Run();

    }
}
