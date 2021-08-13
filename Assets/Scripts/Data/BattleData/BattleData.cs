using Unity.Entities;
using UnityEngine;

public struct BattleData : IComponentData
{
    public bool isDown;
    [HideInInspector]
    public float maxUseTime;
    public float useTime;
    [HideInInspector]
    public bool isUsingSkill;
    [HideInInspector]
    public int DamageTaken;
    [HideInInspector]
    public bool isRecharging;
    public Stats battleStats;
}