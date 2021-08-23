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

    public void UseSkill(int skillNumber, EntityManager entityManager, Entity target, Entity user, ref SharedSkillData sharedSkillData)
    {
        // getting nessesary components
        Animator animator = entityManager.GetComponentObject<Animator>(user);

        animator.Play(animationName.ToString());
        
        
        Entity damageEffectPrefab = BasicSkillSystem.instance.GetPrefab(damageEffectPrefabName.ToString());

        Entity damageEffect = entityManager.Instantiate(damageEffectPrefab);
        entityManager.SetComponentData(damageEffect, entityManager.GetComponentData<Translation>(target));
        entityManager.AddComponentData(user, new UsingSkillData { skillNumber = skillNumber, timePassed = 0});
    }
}
