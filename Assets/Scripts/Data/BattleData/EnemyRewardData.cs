using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[GenerateAuthoringComponent]
public class EnemyRewardData : IComponentData
{
    public int EXP;
    public int gold;
    public EnemyItemData itemData;
}
[System.Serializable]
public struct EnemyItemData{
    public ItemInfo item;
    public float chance;
}
