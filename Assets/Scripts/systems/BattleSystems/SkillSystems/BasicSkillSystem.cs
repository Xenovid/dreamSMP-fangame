using Unity.Entities;
using UnityEngine;

public class BasicSkillSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnStartRunning()
    {
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        Entities
        .WithAll<BasicSkillTag>()
        .ForEach((Entity entity, ref UsingSkillData skillData, ref BattleData battleData, in CharacterStats characterStats) =>{
            // when the animation is done
            if(skillData.skill.keyTimes.IsEmpty){
                ecb.RemoveComponent<BasicSkillTag>(entity);
                ecb.RemoveComponent<UsingSkillData>(entity);
            }
            // stop skill
            else if(battleData.DamageTaken > characterStats.battleStats.superArmor){
                // skip the action and add it to the wait the rest of the time
                 
                battleData.useTime = skillData.skill.keyTimes[0];
                ecb.RemoveComponent<BasicSkillTag>(entity);
                ecb.RemoveComponent<UsingSkillData>(entity);
            }
            else{
                if(battleData.useTime > skillData.skill.keyTimes[0]){
                    // deal damage to opponent
                    DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(skillData.target);
                    enemyDamages.Add(new DamageData{damage = skillData.skill.damageIncrease + characterStats.baseStats.attack, type = damageType.physical});
                    skillData.skill.keyTimes.RemoveAt(0);
                }
            }   
        }).Run();
    }
}
