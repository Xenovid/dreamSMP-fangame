using Unity.Entities;
using UnityEngine;

public struct BattleData : IComponentData
{
    public selectables selected;
    public float useTime;
    public int targetingId;
    public float damage;
}
