using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
[GenerateAuthoringComponent]
public struct BattleRewardData : IComponentData
{
    public int totalEXP;
    public int totalGold;
    public FixedList512<ItemData> items;
}
