using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct BasicPolySkill : IPolySkillData
{ 
    public static string[] ImportantStrings = {"skill name", " skill description" ,"animationName", "damageEffectPrefabName"};
    public int damage;
    public damageType damType;
    
    public float damageTime;
    [HideInInspector]
    public bool dealtDamage;
    [HideInInspector]
    public bool spawnedPrefab;
    public FixedString32 animationName;
    public FixedString32 damageEffectPrefabName;
    public float prefabSpawnTime;

    public string[] GetStrings()
    {
        return ImportantStrings;
    }

    public void UseSkill(int skillNumber, EntityCommandBuffer ecb, Animator animator, Entity target, Entity user, ref SharedSkillData sharedSkillData)
    {
        // getting nessesary components

        animator.Play(animationName.ToString());
        
        
        
        
        ecb.AddComponent(user, new UsingSkillData { skillNumber = skillNumber, timePassed = 0, target = target});
    }
}
