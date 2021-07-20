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
        FixedList128<SkillPrefabInstatiationData> prefabTrack = new FixedList128<SkillPrefabInstatiationData>();
        FixedList64<SkillDamageTrackData> damageTrack = new FixedList64<SkillDamageTrackData>();
        FixedList128<AnimationTrackData> animationTrack = new FixedList128<AnimationTrackData>();

        if(skillInfo.skillTrack.prefabTrack != null){
            foreach(SkillPrefabInstatiationInfo prefabInfo in skillInfo.skillTrack.prefabTrack){
                SkillPrefabInstatiationData prefabInstatiationData = new SkillPrefabInstatiationData{prefabName = prefabInfo.prefabName, instatiationTime = prefabInfo.instatiationTime};
                prefabTrack.Add(prefabInstatiationData);
            }
        }
        if(skillInfo.skillTrack.damageTrack != null){
            foreach(SkillDamageTrackData damageData in skillInfo.skillTrack.damageTrack){
                damageTrack.Add(damageData);
            }
        }
        if(skillInfo.skillTrack.animationTrack != null){
            foreach(AnimationTrackInfo animationTrackInfo in skillInfo.skillTrack.animationTrack){
                AnimationTrackData animationTrackData = new AnimationTrackData{animationName = animationTrackInfo.animationName, time = animationTrackInfo.time};
                animationTrack.Add(animationTrackData);
            }
        }
        SkillTrackData skillTrackData = new SkillTrackData{prefabTrack = prefabTrack, damageTrack = damageTrack, animationTrack = animationTrack};
        return new Skill{
            name = skillInfo.name,
            description = skillInfo.description,
            damageIncrease = skillInfo.damageIncrease,
            cost = skillInfo.cost,
            waitTime = skillInfo.waitTime,
            trackData = skillTrackData
        };
    }
}

[System.Serializable]
public struct SkillInfo{
    public string name;
    public string description;
    public int damageIncrease;
    public int cost;
    public float waitTime;
    // the key points in animation
    public SkillTrackInfo skillTrack;

}
[System.Serializable]
public struct SkillTrackInfo
{
    public SkillPrefabInstatiationInfo[] prefabTrack;
    public SkillDamageTrackData[] damageTrack;
    public AnimationTrackInfo[] animationTrack;
}

[System.Serializable]
public struct AnimationTrackInfo{
    public string animationName;
    public float time;
}

public enum SkillType{
    Regular
}
public enum StatusEffects{
    Bleed
}
