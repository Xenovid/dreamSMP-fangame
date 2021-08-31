using Unity.Entities;
using UnityEngine;

public class DestroyOnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, Animator animator, ref DestroyOnData destroyOnData ) => {
            destroyOnData.timePassed += dt;

            if(animator.isActiveAndEnabled && animator.GetCurrentAnimatorClipInfo(0)[0].clip.length <= destroyOnData.timePassed + .01){
                EntityManager.DestroyEntity(entity);
            }
        }).Run();
    }
}
