using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

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
    //public NativeArray<int> damageTime;
    public FixedList64<float> keyTimes;

    public SkillType skillType;
    public FixedList32<StatusEffects> effects;
}
