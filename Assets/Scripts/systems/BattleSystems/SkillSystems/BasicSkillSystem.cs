using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

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
        .WithoutBurst()
        .ForEach((Entity entity, ref UsingSkillData skillData, ref BattleData battleData, in CharacterStats characterStats, in Animator animator, in Translation translation) =>{
            // has three tracks to follow, will end once all tracks are finished
            bool isEmpty = true;
            if(skillData.skill.trackData.animationTrack.Length > 0){
                if(skillData.skill.trackData.animationTrack[0].time < battleData.useTime){
                    animator.Play(skillData.skill.trackData.animationTrack[0].animationName.ToString());
                    skillData.skill.trackData.animationTrack.RemoveAt(0);
                }
                isEmpty =false;
            }
            if(skillData.skill.trackData.damageTrack.Length > 0){
                if(skillData.skill.trackData.damageTrack[0].time < battleData.useTime){
                    DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(skillData.target);
                    enemyDamages.Add(new DamageData{damage = skillData.skill.trackData.damageTrack[0].damage + characterStats.baseStats.attack, type = damageType.physical, statusEffect = skillData.skill.trackData.damageTrack[0].statusEffect});
                    skillData.skill.trackData.damageTrack.RemoveAt(0);
                }
                isEmpty = false;
            }
            if(skillData.skill.trackData.prefabTrack.Length > 0){
                if(skillData.skill.trackData.prefabTrack[0].instatiationTime < battleData.useTime){
                    Entity prefabEntity = ecb.Instantiate(GetPrefab(skillData.skill.trackData.prefabTrack[0].prefabName.ToString()));
                    ecb.AddComponent(prefabEntity, new TransitionData{oldPosition = translation.Value, newPosition = GetComponent<Translation>(skillData.target).Value});
                    ecb.AddComponent(prefabEntity, new BattlePrefabData{target = skillData.target});
                    skillData.skill.trackData.prefabTrack.RemoveAt(0);
                }
                isEmpty = false;
            }
            
            if(isEmpty){
                ecb.RemoveComponent<BasicSkillTag>(entity);
                ecb.RemoveComponent<UsingSkillData>(entity);
            }
        }).Run();
    }
    private Entity GetPrefab(string prefabName){
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
