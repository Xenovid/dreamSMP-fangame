using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BattleTriggerData : IComponentData
{
    public bool isRepeatable;
}
