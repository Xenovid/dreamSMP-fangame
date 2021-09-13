using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class BattleThrowableSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithNone<TransitionData, DestroyOnData>()
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((BattleThrowableData battleThrowableData, Animator animator, Entity entity) => {
            // plays the animation for when it hits, and sets it up so when the animation finishes it delets itself
            animator.Play(battleThrowableData.HitAnimationName);
            EntityManager.AddComponent<DestroyOnData>(entity);
        }).Run();
    }
}
