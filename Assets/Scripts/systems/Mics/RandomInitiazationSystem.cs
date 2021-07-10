using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
[UpdateBefore(typeof(BasicEnemyMovementSystem))]
public class RandomInitiazationSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate(){
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
        .WithAll<RandomTag>()
        .WithNone<RandomData>()
        .ForEach((Entity entity, int entityInQueryIndex) => {
            ecb.AddComponent(entityInQueryIndex, entity, new RandomData{});
        }).ScheduleParallel();
        Entities
        .WithAll<RandomTag>()
        .ForEach((Entity entity, int entityInQueryIndex, ref RandomData randomData) => {
            randomData.Value = Random.CreateFromIndex((uint) entityInQueryIndex);
            ecb.RemoveComponent<RandomTag>(entityInQueryIndex, entity);
        }).ScheduleParallel();
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
