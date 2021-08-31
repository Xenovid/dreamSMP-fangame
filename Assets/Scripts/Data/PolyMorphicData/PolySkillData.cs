using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
[Serializable]
public struct SharedSkillData{

    public float chance;
    public float recoveryTime;
    public int cost;
    public FixedString32 name;
    public FixedString64 description;
}
[PolymorphicComponentDefinition(
    "PolySkillData",
    "Scripts/Data/PolyMorphicData/Generated",
    true,
    false,
    typeof(SharedSkillData)
)]
public interface IPolySkillData
{
    void UseSkill(int skillNumber, EntityCommandBuffer ecb, Animator animator, Entity target, Entity user, ref SharedSkillData sharedSkillData);

    String[] GetStrings();

    
}
