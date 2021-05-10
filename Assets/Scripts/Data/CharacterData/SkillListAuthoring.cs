using Unity.Entities;
using Unity.Collections;
using UnityEngine;

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
        Entities.ForEach((Entity entity,SkillListAuthoring skillList) =>{
            DstEntityManager.AddBuffer<SkillData>(entity);
            DstEntityManager.AddBuffer<EquipedSkillData>(entity);
            DynamicBuffer<SkillData> skills = DstEntityManager.GetBuffer<SkillData>(entity);
            DynamicBuffer<EquipedSkillData> equipedSkills = DstEntityManager.GetBuffer<EquipedSkillData>(entity);

            foreach(SkillInfo skillInfo in skillList.skillInfos){
                Skill skill = new Skill{
                    name = skillInfo.name,
                    description = skillInfo.description,
                    damageBoost = skillInfo.damageBoost,
                    cost = skillInfo.cost
                };
                skills.Add(new SkillData{skill = skill});
            }
            foreach(SkillInfo skillInfo in skillList.equipedSkills){
                Skill skill = new Skill{
                    name = skillInfo.name,
                    description = skillInfo.description,
                    damageBoost = skillInfo.damageBoost,
                    cost = skillInfo.cost
                };
                equipedSkills.Add(new EquipedSkillData{skill = skill});
            }
        });
    }
}

[System.Serializable]
public struct SkillInfo{
    public string name;
    public string description;
    public int damageBoost;
    public int cost;
}

[System.Serializable]
public struct Skill{
    public FixedString32 name;
    public FixedString128 description;
    public int damageBoost;
    public int cost;
}