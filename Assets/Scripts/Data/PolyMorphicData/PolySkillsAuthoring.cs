using System.Collections;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PolySkillsAuthoring : MonoBehaviour
{
    public List<PolySkillInfo> skills;
}
[System.Serializable]
public struct PolySkillInfo{
    [SerializeReference]
    public IPolySkillData skill;
    public PolySkillData.TypeId typeID;
    public SharedSkillData data;
    [SerializeField]
    public List<string> importantStrings;
}

public class PolySkillsConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PolySkillsAuthoring polySkillsAuthoring) => {
            Entity entity = GetPrimaryEntity(polySkillsAuthoring);
            DynamicBuffer<PolySkillData> skills = DstEntityManager.AddBuffer<PolySkillData>(entity);
            foreach(PolySkillInfo polySkill in polySkillsAuthoring.skills){
                switch(polySkill.typeID){
                    case PolySkillData.TypeId.BasicPolySkill:
                        BasicPolySkill skill =(BasicPolySkill) polySkill.skill;
                        skill.animationName = polySkill.importantStrings[0];
                        skill.damageEffectPrefabName = polySkill.importantStrings[1];
                        skills.Add(new PolySkillData((BasicPolySkill)polySkill.skill, polySkill.data));
                    break;
                }
                
            }
        });
    }
}
