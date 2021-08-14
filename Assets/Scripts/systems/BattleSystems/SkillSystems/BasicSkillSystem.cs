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
        .WithAll<BasicSkillTag>()
        .WithoutBurst()
        .ForEach((Entity entity, ref UsingSkillData skillData, ref BattleData battleData, in CharacterStats characterStats, in Animator animator, in Translation translation) =>{
            // has three tracks to follow, will end once all tracks are finished
            /*
            bool isEmpty = true;
            switch(skillData.skill.CurrentTypeId){
                case PolySkillData.TypeId.BasicPolySkill:
                    skillData.skill.SharedSkillData.timePassed += dt;
                    if(skillData.skill.SharedSkillData.timePassed >= skillData.skill.BasicPolySkill.damageTime){
                        DynamicBuffer<DamageData> damages =  GetBuffer<DamageData>(skillData.skill.SharedSkillData.target);
                        damages.Add(new DamageData{damage = skillData.skill.BasicPolySkill.damage, type = skillData.skill.BasicPolySkill.damType});
                    }
                break;
            }
            
            
            if(isEmpty){
                ecb.RemoveComponent<BasicSkillTag>(entity);
                ecb.RemoveComponent<UsingSkillData>(entity);
            }*/
        }).Run();
    }
    public Entity GetPrefab(string prefabName){
        Entity prefabHolder = GetSingletonEntity<PrefabholderTag>();
        DynamicBuffer<PrefabReferenceEntity> prefabs = GetBuffer<PrefabReferenceEntity>(prefabHolder);
        
        foreach(PrefabReferenceEntity prefabReference in prefabs){
            if(prefabReference.prefabName  == prefabName){
                return prefabReference.prefab;
            }
        }
        Debug.Log("prefab not found");
        return EntityManager.CreateEntity();
    }
}
