using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class AnimationComponent : ICleanupComponentData
{
    public Animator animator;

    public FixedString32Bytes animationName;
    public bool RequestPlay;
}
