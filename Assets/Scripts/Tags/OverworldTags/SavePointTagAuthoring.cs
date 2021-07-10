using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class SavePointTagAuthoring : MonoBehaviour{
    public string saveName;
}

public struct SavePointTag : IComponentData
{
    public FixedString128 saveName;
}

public class SavePointTagConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((SavePointTagAuthoring savePointTag) => {
            Entity entity = GetPrimaryEntity(savePointTag);
            DstEntityManager.AddComponentData(entity, new SavePointTag{saveName = savePointTag.saveName});
        });
    }
}
