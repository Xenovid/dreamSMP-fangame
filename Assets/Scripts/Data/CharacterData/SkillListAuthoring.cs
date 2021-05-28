using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;

public class SkillListAuthoring : MonoBehaviour
{
    public SkillInfo[] skillInfos;
    public SkillInfo[] equipedSkills;
}
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
            Debug.Log(entity);
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
    public Skill SkillInfoToSkill(SkillInfo skillInfo){
        NativeList<int> temp = new NativeList<int>(Allocator.Persistent);
        foreach(int i in skillInfo.damageTimes){
            temp.Add(i);
        }
        return new Skill{
            name = skillInfo.name,
            description = skillInfo.description,
            functionName = skillInfo.functionName,
            damageIncrease = skillInfo.damageIncrease,
            animationName = skillInfo.animationName,
            cost = skillInfo.cost,
            waitTime = skillInfo.waitTime
            //damageTime = temp
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
    // the point in the animation where you deal damage
    [Range(0, 1)]
    public float[] damageTimes;
}

[System.Serializable]
public struct Skill{
    public FixedString32 name;
    public FixedString32 functionName;
    public FixedString128 description;
    public FixedString64 animationName;
    public int damageIncrease;
    public int cost;
    public float waitTime; 
    // the point in the animation where you deal damage
    //public NativeList<int> damageTime;
}