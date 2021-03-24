using Unity.Entities;
using UnityEngine;

public struct BattleData : IComponentData
{
    public selectables selected;
    public float recoveryTimer;
    public int targetingId;
}
