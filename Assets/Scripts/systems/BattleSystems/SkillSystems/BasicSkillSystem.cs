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
        .ForEach((Entity entity, ref UsingSkillData usingSkill, ref BattleData battleData, in CharacterStats characterStats, in Animator animator, in Translation translation, in DynamicBuffer<PolySkillData> skills) =>{
            // switches the function based on what skill type the used skill is
            switch(skills[usingSkill.skillNumber].CurrentTypeId){
                case PolySkillData.TypeId.BasicPolySkill:
                    usingSkill.timePassed += dt;
                    if(usingSkill.timePassed >= skills[usingSkill.skillNumber].BasicPolySkill.damageTime){
                        DynamicBuffer<DamageData> damages =  GetBuffer<DamageData>(usingSkill.target);
                        damages.Add(new DamageData{damage = skills[usingSkill.skillNumber].BasicPolySkill.damage, type = skills[usingSkill.skillNumber].BasicPolySkill.damType});
                    }
                    else if(skills[usingSkill.skillNumber].SharedSkillData.recoveryTime < usingSkill.timePassed){

                    }
                break;
            }
        }).Run();
    }
    public Entity GetPrefab(string prefabName){
        Entity prefabHolder = GetSingletonEntity<PrefabholderTag>();
        DynamicBuffer<PrefabReferenceEntity> prefabs = GetBuffer<PrefabReferenceEntity>(prefabHolder);
        Debug.Log(prefabName);


        foreach (PrefabReferenceEntity prefabReference in prefabs){
            
            if(prefabReference.prefabName  == prefabName){
                return prefabReference.prefab;
            }
        }
        Debug.Log("prefab not found");
        return EntityManager.CreateEntity();
    }
}
