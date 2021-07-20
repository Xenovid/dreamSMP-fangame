using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public struct Skill{
    public FixedString32 name;
    public FixedString128 description;
    public int damageIncrease;
    public int cost;
    public float waitTime; 
    public SkillTrackData trackData;

}
