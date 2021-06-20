using Unity.Entities;
using UnityEngine;

public class PauseSystem : SystemBase
{
    protected override void OnUpdate()
    {
        
    }
    public void Pause(){
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity) => {
            EntityManager.AddComponent<PausedTag>(entity);
        }).Run();
        Entities
        .WithoutBurst()
        .ForEach((Animator animator) => {
            animator.speed = 0;
        }).Run();
    }
    public void UnPause(){
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<PausedTag>()
        .ForEach((Entity entity) => {
            EntityManager.RemoveComponent<PausedTag>(entity);
        }).Run();
        Entities
        .WithoutBurst()
        .ForEach((Animator animator) => {
            animator.speed = 1;
        }).Run();
    }

}
