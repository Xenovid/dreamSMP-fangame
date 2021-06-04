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
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityQuery technoQuery = GetEntityQuery(typeof(TechnoData), typeof(CharacterStats));
        CharacterStats technoStats = technoQuery.GetSingleton<CharacterStats>();
        Entity technoEntity = technoQuery.GetSingletonEntity();

        float dt = Time.DeltaTime;
        Entities
        .WithNone<DownTag>()
        .ForEach((ref DynamicBuffer<DamageData> damages, ref BleedingData bleedingData) =>{
            bleedingData.timeFromLastDamageTick += dt;
            //deal damage every other second
            if(bleedingData.timeFromLastDamageTick >= 2){
                damages.Add(new DamageData{damage = bleedingData.level, type = damageType.bleeding});
                bleedingData.timeFromLastDamageTick = 0;
                if(technoStats.maxPoints <= technoStats.points + bleedingData.level){
                    technoStats.points = technoStats.maxPoints;
                }
                else{
                    technoStats.points += bleedingData.level;
                }
                ecb.SetComponent(technoEntity, technoStats);
            }
        }).Schedule();
        Entities
        .WithAll<DownTag>()
        .ForEach((Entity entity, ref BleedingData bleedingData) =>{
            ecb.RemoveComponent<BleedingData>(entity);
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
