using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
[System.Serializable]
public struct PlayerSaveData
{
    public AnimationSaveData animationSaveData;
    public ItemData[] itemInventory;
    //public SkillData[] skills;
    public CharacterStats characterStats;
    public MovementData movementData;
    public float3 trasition;

}
