using Unity.Entities;
using UnityEngine;

public class RegularAttack : SystemBase
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
        .WithAll<RegularAttackData>()
        .ForEach((Entity entity, ref UsingSkillData skillData, ref BattleData battleData, in CharacterStats characterStats) =>{
            // when the animation is done
            if(skillData.skill.keyTimes.IsEmpty){
                ecb.RemoveComponent<RegularAttackData>(entity);
                ecb.RemoveComponent<UsingSkillData>(entity);
            }
            // stop skill when too much damage is taken
            else if(battleData.DamageTaken > characterStats.battleStats.superArmor){
                // skip the action and add it to the wait the rest of the time
                
                battleData.useTime = skillData.skill.keyTimes[0];
                ecb.RemoveComponent<RegularAttackData>(entity);
                ecb.RemoveComponent<UsingSkillData>(entity);
            }
            else{
                if(battleData.useTime > skillData.skill.keyTimes[0]){
                    DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(skillData.target);
                    enemyDamages.Add(new DamageData{damage = skillData.skill.damageIncrease + characterStats.baseStats.attack, color = damageColor.white});
                    skillData.skill.keyTimes.RemoveAt(0);
                    // add bleeding to opponent if it doesn't have it already
                    foreach(StatusEffects effect in skillData.skill.effects){
                        switch(effect){
                            case StatusEffects.Bleed:
                                if(!HasComponent<BleedingData>(skillData.target)){
                                    ecb.AddComponent(skillData.target, new BleedingData{level = 1});
                                }
                                else{
                                    // have a chance to add more levels
                                }
                            break;
                        }
                    }
                    
                }
            }   
        }).Run();
    }
}
