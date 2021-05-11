using Unity.Entities;
using UnityEngine;

public struct BattleData : IComponentData
{
    public bool isDown;
    [HideInInspector]
    public float maxUseTime;
    public float useTime;
}