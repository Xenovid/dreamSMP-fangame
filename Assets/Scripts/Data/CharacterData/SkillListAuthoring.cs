using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

public class SkillListAuthoring : MonoBehaviour
{
    public SkillInfo[] skillInfos;
    public SkillInfo[] equipedSkills;
}
[System.Serializable]
public struct SkillData : IBufferElementData{
    public Skill skill;
}
public struct EquipedSkillData : IBufferElementData{
    public Skill skill;
}
public class SkillConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach(( SkillListAuthoring skillList) =>{
            Entity entity = GetPrimaryEntity(skillList);
            DstEntityManager.AddBuffer<SkillData>(entity);
            DstEntityManager.AddBuffer<EquipedSkillData>(entity);
            DynamicBuffer<SkillData> skills = DstEntityManager.GetBuffer<SkillData>(entity);
            DynamicBuffer<EquipedSkillData> equipedSkills = DstEntityManager.GetBuffer<EquipedSkillData>(entity);

            foreach(SkillInfo skillInfo in skillList.skillInfos){
                skills.Add(new SkillData{skill = SkillInfoToSkill(skillInfo)});
            }
            foreach(SkillInfo skillInfo in skillList.equipedSkills){
                equipedSkills.Add(new EquipedSkillData{skill = SkillInfoToSkill(skillInfo)});
            }
        });
    }
    public static Skill SkillInfoToSkill(SkillInfo skillInfo){
        FixedList64<float> keyTimes = new FixedList64<float>();
        FixedList32<StatusEffects> effects = new FixedList32<StatusEffects>();
        foreach(float number in skillInfo.keyTimes){
            keyTimes.Add(number);
        }
        foreach(StatusEffects effect in skillInfo.effects){
            effects.Add(effect);
        }
        return new Skill{
            name = skillInfo.name,
            description = skillInfo.description,
            functionName = skillInfo.functionName,
            damageIncrease = skillInfo.damageIncrease,
            animationName = skillInfo.animationName,
            cost = skillInfo.cost,
            waitTime = skillInfo.waitTime,
            keyTimes = keyTimes,
            skillType = skillInfo.skillType,
            effects = effects
        };
    }
}

[System.Serializable]
public struct SkillInfo{
    public string name;
    public string description;
    public string functionName;
    public string animationName;
    public int damageIncrease;
    public int cost;
    public float waitTime;
    // the key points in animation
    [SerializeField]
    public List<float> keyTimes;
    public SkillType skillType;
    public List<StatusEffects> effects;
}


public enum SkillType{
    Regular
}
public enum StatusEffects{
    Bleed
}