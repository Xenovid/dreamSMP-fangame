using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class AnimationData : IComponentData
{
    public Direction previouslyFacing;
    public string currentAnimation;
    public bool inIdleAnimation;

    public string idleUpAnimationName;
    public string idleDownAnimationName;
    public string idleRightAnimationName;
    public string idleLeftAnimationName;

    public string walkUpAnimationName;
    public string walkDownAnimationName;
    public string walkRightAnimationName;
    public string walkLeftAnimationName;
}
