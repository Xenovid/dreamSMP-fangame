using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BattleData : IComponentData
{
    public selectables selected;
    public float recoveryTimer;
    public int targetingId;
}
