using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct BasicPolySkill : IPolySkillData
{
    public int damage;
    public damageType damType;
    public float damageTime;
    public FixedString32 animationName;
    public FixedString32 damageEffectPrefabName;

    public void Update(float deltaTime, EntityManager entityManager, ref SharedSkillData sharedSkillData)
    {
       
    }

    public void UseSkill(Animator animator, EntityManager entityManager, ref SharedSkillData sharedSkillData)
    {
        animator.Play(animationName.ToString());
        Entity damageEffectPrefab = BasicSkillSystem.instance.GetPrefab(damageEffectPrefabName.ToString());
        Entity damageEffect = entityManager.Instantiate(damageEffectPrefab);
        entityManager.SetComponentData(damageEffect, entityManager.GetComponentData<Translation>(sharedSkillData.target));
    }
}
