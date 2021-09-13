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
    public importantString[] importantStrings;
}
[System.Serializable]
public struct importantString{
    public string stringName;
    public string theString;
}

public class PolySkillsConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PolySkillsAuthoring polySkillsAuthoring) => {
            Entity entity = GetPrimaryEntity(polySkillsAuthoring);
            DynamicBuffer<PolySkillData> skills = DstEntityManager.AddBuffer<PolySkillData>(entity);
            foreach(PolySkillInfo PolySkill in polySkillsAuthoring.skills){
                PolySkillInfo polySkill = PolySkill;
                switch(polySkill.typeID){
                    case PolySkillData.TypeId.BasicPolySkill:
                        BasicPolySkill skill = (BasicPolySkill) polySkill.skill;
                        skill.animationName = polySkill.importantStrings[2].theString;
                        skill.damageEffectPrefabName = polySkill.importantStrings[3].theString;
                        polySkill.data.name = polySkill.importantStrings[0].theString;
                        polySkill.data.description = polySkill.importantStrings[1].theString;
                        
                        skills.Add(new PolySkillData(skill, polySkill.data));
                    break;
                    case PolySkillData.TypeId.ThrowablePolySkill:
                        ThrowablePolySkill throwableSkill = (ThrowablePolySkill) polySkill.skill;
                        throwableSkill.animationName = polySkill.importantStrings[2].theString;
                        throwableSkill.damageEffectPrefabName = polySkill.importantStrings[3].theString;
                        polySkill.data.name = polySkill.importantStrings[0].theString;
                        polySkill.data.description = polySkill.importantStrings[1].theString;
                        
                        skills.Add(new PolySkillData(throwableSkill, polySkill.data));
                    break;
                }
                
            }
        });
    }
}
