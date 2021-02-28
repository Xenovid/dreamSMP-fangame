using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public struct BattleInfo : IComponentData
{
    public NativeArray<int> ids;
}
