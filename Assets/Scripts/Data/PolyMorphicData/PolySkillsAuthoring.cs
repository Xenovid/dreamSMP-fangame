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
            foreach(PolySkillInfo PolySkill in polySkillsAuthoring.skills){
                PolySkillInfo polySkill = PolySkill;
                switch(polySkill.typeID){
                    case PolySkillData.TypeId.BasicPolySkill:
                        BasicPolySkill skill =(BasicPolySkill) polySkill.skill;
                        skill.animationName = polySkill.importantStrings[0];
                        skill.damageEffectPrefabName = polySkill.importantStrings[1];
                        polySkill.data.name = polySkill.importantStrings[2];
                        polySkill.data.description = polySkill.importantStrings[3];
                        
                        skills.Add(new PolySkillData(skill, polySkill.data));
                    break;
                }
                
            }
        });
    }
}
