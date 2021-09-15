using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;

public class PauseSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithAny<PausedTag, BattleData>()
        .ForEach((ref Translation translation, ref Rotation rotation, ref PhysicsVelocity velocity) => {
            translation.Value = new float3(translation.Value.x, translation.Value.y, 0);
            rotation.Value = quaternion.EulerXYZ(new float3(0,0,0));
            velocity.Linear = new float3(0,0,0);
        }).ScheduleParallel();
    }
    public void BattlePause(){
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithNone<BattleData>()
        .ForEach((Entity entity) => {
            EntityManager.AddComponent<PausedTag>(entity);
        }).Run();
        Entities
        .WithoutBurst()
        .WithNone<BattleData>()
        .ForEach((Animator animator) => {
            animator.speed = 0;
        }).Run();
    }
    public void BattleUnPause(){
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithNone<BattleData>()
        .ForEach((Entity entity) => {
            EntityManager.RemoveComponent<PausedTag>(entity);
        }).Run();
        Entities
        .WithoutBurst()
        .WithNone<BattleData>()
        .ForEach((Animator animator) => {
            animator.speed = 1;
        }).Run();
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
        Entities.ForEach((ref PhysicsVelocity Velocity) => {
            Velocity.Linear = 0;
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
