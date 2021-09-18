using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
[GenerateAuthoringComponent]
public class LevelUpRewardData : IComponentData
{
    public int startingLevel = 1;
    public List<LevelReward> levelRewards;
}
[System.Serializable]
public struct LevelReward{
    public int healthBonus;
    public int attackBonus;
    public int defenceBonus;
    public int pointsBonus;
    public int skillUnlockIndex;
}
