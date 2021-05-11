using Unity.Entities;
using UnityEngine;

public class BattleDamageSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate(){
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex, ref CharacterStats characterStats, in DamageData damage) =>
            {
                Debug.Log("dealing damage");
                characterStats.health -= damage.damage;
                //animationData.hasTakenDamage = true;
                ecb.RemoveComponent<DamageData>(entity);
            }).Run();
    m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
