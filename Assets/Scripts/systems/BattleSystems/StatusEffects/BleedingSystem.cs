using Unity.Entities;
using System;
public class BleedingSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    BattleSystem battleSystem;
    protected override void OnCreate()
    {
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleEnd += RemoveBleedingOnBattleEnd;
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities.ForEach((ref DynamicBuffer<DamageData> damages, ref BleedingData bleedingData) =>{
            bleedingData.timeFromLastDamageTick += dt;
            //deal damage every other second
            if(bleedingData.timeFromLastDamageTick >= 2){
                damages.Add(new DamageData{damage = bleedingData.level, color = damageColor.red});
                bleedingData.timeFromLastDamageTick = 0;
            }
        }).Schedule();
    }
    private void RemoveBleedingOnBattleEnd( Object sender, EventArgs e){
        EntityManager.CompleteAllJobs();
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        Entities
        .WithAll<BleedingData>()
        .ForEach((Entity entity) =>{
            ecb.RemoveComponent<BleedingData>(entity);
        }).Run();
    }
}
