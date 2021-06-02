using Unity.Entities;
using UnityEngine;

public class TechnoDefaultAttack : SystemBase
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
        .WithAll<TechnoDefaultAttackData>()
        .ForEach((Entity entity, ref UsingSkillData skillData, ref BattleData battleData, in CharacterStats characterStats) =>{
            // when the animation is done
            if(skillData.skill.keyTimes.IsEmpty){
                ecb.RemoveComponent<TechnoDefaultAttackData>(entity);
                ecb.RemoveComponent<UsingSkillData>(entity);
            }
            // stop skill when too much damage is taken
            else if(battleData.DamageTaken > characterStats.battleStats.superArmor){
                // skip the action and add it to the wait the rest of the time
                 
                battleData.useTime = skillData.skill.keyTimes[0];
                ecb.RemoveComponent<TechnoDefaultAttackData>(entity);
                ecb.RemoveComponent<UsingSkillData>(entity);
            }
            else{
                if(battleData.useTime > skillData.skill.keyTimes[0]){
                    // deal damage to opponent
                    DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(skillData.target);
                    enemyDamages.Add(new DamageData{damage = skillData.skill.damageIncrease + characterStats.baseStats.attack});
                    skillData.skill.keyTimes.RemoveAt(0);
                    // add bleeding to opponent if it doesn't have it already
                    if(!HasComponent<BleedingData>(skillData.target)){
                        ecb.AddComponent(skillData.target, new BleedingData{level = 1});
                    }
                    else{
                        // have a chance to add more levels
                    }
                }
            }   
        }).Run();
    }
}
