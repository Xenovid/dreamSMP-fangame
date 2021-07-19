using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
[System.Serializable]
public struct SkillTrackData
{
    public FixedList128<SkillPrefabInstatiationData> prefabTrack;
    public FixedList64<SkillDamageTrackData> damageTrack;
    public FixedList128<AnimationTrackData> animationTrack;
}
[System.Serializable]
public struct SkillPrefabInstatiationInfo{
    public string prefabName;
    public float instatiationTime;
}
[System.Serializable]
public struct SkillPrefabInstatiationData
{
    public FixedString32 prefabName;
    public float instatiationTime;
}
[System.Serializable]
public struct SkillDamageTrackData{
    public int damage;
    public float time;
}
public struct AnimationTrackData{
    public FixedString32 animationName;
    public float time;
}
