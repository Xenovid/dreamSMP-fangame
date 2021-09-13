using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

public class BasicSkillSystem : SystemBase
{
    public static BasicSkillSystem instance;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnStartRunning()
    {
        instance = this;
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        float dt = Time.DeltaTime;

        Entities
        .WithoutBurst()
        .ForEach((Entity entity, ref DynamicBuffer<PolySkillData> skills, ref UsingSkillData usingSkill, ref BattleData battleData, in CharacterStats characterStats, in Animator animator, in Translation translation) =>{
            // switches the function based on what skill type the used skill is
            usingSkill.timePassed += dt;
            switch(skills[usingSkill.skillNumber].CurrentTypeId){
                case PolySkillData.TypeId.BasicPolySkill:
                    if(usingSkill.timePassed >= skills[usingSkill.skillNumber].BasicPolySkill.damageTime && !skills[usingSkill.skillNumber].BasicPolySkill.dealtDamage){
                        // have to turn it into a variable to change it
                        PolySkillData skill = skills[usingSkill.skillNumber];
                        skill.BasicPolySkill.dealtDamage = true;
                        skills[usingSkill.skillNumber] = skill;

                        DynamicBuffer<DamageData> damages =  GetBuffer<DamageData>(usingSkill.target);
                        damages.Add(new DamageData{damage = skills[usingSkill.skillNumber].BasicPolySkill.damage, type = damageType.physical});

                        switch(skills[usingSkill.skillNumber].BasicPolySkill.damType){
                            case damageType.bleeding:
                                if(HasComponent<BleedingData>(usingSkill.target)){
                                    BleedingData bleedingData = GetComponent<BleedingData>(usingSkill.target);
                                    bleedingData.level += 1;
                                    SetComponent(usingSkill.target, bleedingData);
                                }
                                else{
                                    ecb.AddComponent(usingSkill.target, new BleedingData{level = 1});
                                }


                            break;
                        }
                        
                    }
                    else if(skills[usingSkill.skillNumber].BasicPolySkill.prefabSpawnTime <= usingSkill.timePassed && !skills[usingSkill.skillNumber].BasicPolySkill.spawnedPrefab){
                        Entity damageEffectPrefab = BasicSkillSystem.instance.GetPrefab(skills[usingSkill.skillNumber].BasicPolySkill.damageEffectPrefabName.ToString());

                        Entity damageEffect = ecb.Instantiate(damageEffectPrefab);
                        if(HasComponent<BattleOffsetData>(usingSkill.target)){
                            Translation prefabTranslation = new Translation{Value = EntityManager.GetComponentData<Translation>(usingSkill.target).Value + GetComponent<BattleOffsetData>(usingSkill.target).offset};
                            ecb.SetComponent(damageEffect, prefabTranslation);
                        }
                        else{
                            ecb.SetComponent(damageEffect, EntityManager.GetComponentData<Translation>(usingSkill.target));
                        }

                        PolySkillData skill = skills[usingSkill.skillNumber];
                        skill.BasicPolySkill.spawnedPrefab = true;
                        skills[usingSkill.skillNumber] = skill;
                    }
                    else if(skills[usingSkill.skillNumber].SharedSkillData.recoveryTime <= usingSkill.timePassed){
                        PolySkillData skill = skills[usingSkill.skillNumber];
                        skill.BasicPolySkill.dealtDamage = false;
                        skill.BasicPolySkill.spawnedPrefab = false;
                        skills[usingSkill.skillNumber] = skill;
                        ecb.RemoveComponent<UsingSkillData>(entity);
                    }
                break;
                case PolySkillData.TypeId.ThrowablePolySkill:
                    if(usingSkill.timePassed >= skills[usingSkill.skillNumber].ThrowablePolySkill.damageTime && !skills[usingSkill.skillNumber].ThrowablePolySkill.dealtDamage){
                        // have to turn it into a variable to change it
                        PolySkillData skill = skills[usingSkill.skillNumber];
                        skill.ThrowablePolySkill.dealtDamage = true;
                        skills[usingSkill.skillNumber] = skill;

                        DynamicBuffer<DamageData> damages =  GetBuffer<DamageData>(usingSkill.target);
                        damages.Add(new DamageData{damage = skills[usingSkill.skillNumber].ThrowablePolySkill.damage, type = damageType.physical});

                        switch(skills[usingSkill.skillNumber].ThrowablePolySkill.damType){
                            case damageType.bleeding:
                                if(HasComponent<BleedingData>(usingSkill.target)){
                                    BleedingData bleedingData = GetComponent<BleedingData>(usingSkill.target);
                                    bleedingData.level += 1;
                                    SetComponent(usingSkill.target, bleedingData);
                                }
                                else{
                                    ecb.AddComponent(usingSkill.target, new BleedingData{level = 1});
                                }


                            break;
                        }
                        
                    }
                    else if(skills[usingSkill.skillNumber].ThrowablePolySkill.prefabSpawnTime <= usingSkill.timePassed && !skills[usingSkill.skillNumber].ThrowablePolySkill.spawnedPrefab){
                        Entity damageEffectPrefab = BasicSkillSystem.instance.GetPrefab(skills[usingSkill.skillNumber].ThrowablePolySkill.damageEffectPrefabName.ToString());

                        Entity damageEffect = ecb.Instantiate(damageEffectPrefab);
                        
                        if(HasComponent<BattleOffsetData>(usingSkill.target)){
                            if(HasComponent<BattleOffsetData>(entity)){
                                BattleOffsetData battleOffsetData = GetComponent<BattleOffsetData>(entity);
                                ecb.AddComponent(damageEffect, new TransitionData{
                                    oldPosition = EntityManager.GetComponentData<Translation>(entity).Value + battleOffsetData.offset,
                                    newPosition = EntityManager.GetComponentData<Translation>(usingSkill.target).Value + GetComponent<BattleOffsetData>(usingSkill.target).offset,
                                    duration = skills[usingSkill.skillNumber].ThrowablePolySkill.prefabThrowDuration
                                });
                            }
                            else{
                                Translation prefabTranslation = new Translation{Value = EntityManager.GetComponentData<Translation>(usingSkill.target).Value + GetComponent<BattleOffsetData>(usingSkill.target).offset};
                                ecb.SetComponent(damageEffect, prefabTranslation);
                            }
                            
                            
                        }
                        else{
                            if(HasComponent<BattleOffsetData>(entity)){
                                BattleOffsetData battleOffsetData = GetComponent<BattleOffsetData>(entity);
                                ecb.AddComponent(damageEffect, new TransitionData{
                                    oldPosition = EntityManager.GetComponentData<Translation>(entity).Value + battleOffsetData.offset,
                                    newPosition = EntityManager.GetComponentData<Translation>(usingSkill.target).Value,
                                    duration = skills[usingSkill.skillNumber].ThrowablePolySkill.prefabThrowDuration
                                });
                            }
                            ecb.SetComponent(damageEffect, EntityManager.GetComponentData<Translation>(usingSkill.target));
                        }

                        PolySkillData skill = skills[usingSkill.skillNumber];
                        skill.ThrowablePolySkill.spawnedPrefab = true;
                        skills[usingSkill.skillNumber] = skill;
                    }
                    else if(skills[usingSkill.skillNumber].SharedSkillData.recoveryTime <= usingSkill.timePassed){
                        PolySkillData skill = skills[usingSkill.skillNumber];
                        skill.ThrowablePolySkill.dealtDamage = false;
                        skill.ThrowablePolySkill.spawnedPrefab = false;
                        skills[usingSkill.skillNumber] = skill;
                        ecb.RemoveComponent<UsingSkillData>(entity);
                    }
                break;
            }
        }).Run();
    }
    public Entity GetPrefab(string prefabName){
        Entity prefabHolder = GetSingletonEntity<PrefabholderTag>();
        DynamicBuffer<PrefabReferenceEntity> prefabs = GetBuffer<PrefabReferenceEntity>(prefabHolder);
        //Debug.Log(prefabName);


        foreach (PrefabReferenceEntity prefabReference in prefabs){
            
            if(prefabReference.prefabName  == prefabName){
                return prefabReference.prefab;
            }
        }
        Debug.Log("prefab not found");
        return EntityManager.CreateEntity();
    }
}
