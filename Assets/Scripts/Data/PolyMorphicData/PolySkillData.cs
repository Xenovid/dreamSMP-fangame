using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[Serializable]
public struct SharedSkillData{
    public Entity target;
    public float timePassed;
    public float recoveryTime;
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
    void Update(float deltaTime, EntityManager entityManager, ref SharedSkillData sharedSkillData);
    void UseSkill(Animator animator, EntityManager entityManager, ref SharedSkillData sharedSkillData);

    
}
