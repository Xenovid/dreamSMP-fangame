using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class BattleOffsetDataAuthoring : MonoBehaviour
{
    public Transform offset;
}
public struct BattleOffsetData : IComponentData{
    public float3 offset;
}
public class BattleOffsetConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((BattleOffsetDataAuthoring battleOffsetDataAuthoring) => {
            Entity entity = GetPrimaryEntity(battleOffsetDataAuthoring);
            DstEntityManager.AddComponentData(entity, new BattleOffsetData{offset = battleOffsetDataAuthoring.offset.transform.localPosition});
        });
    }
}


