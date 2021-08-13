using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[Serializable]
public struct SharedSkillData{
    public Entity target;
    public float timePassed;
    public float recoveryTime;
    public float damageModifier;
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
    void Update(float deltaTime, Animator animator, ref SharedSkillData sharedSkillData);
}
