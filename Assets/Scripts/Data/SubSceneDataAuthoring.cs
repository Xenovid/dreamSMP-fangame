using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Scenes;

public class SubSceneDataAuthoring : MonoBehaviour
{
    public string subsceneName;
    public bool shouldBeLoaded;
}

public struct SubSceneData : IComponentData{
    public FixedString512 subsceneName;
    public bool shouldBeLoaded;
}

public class SubsceneDataConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((SubSceneDataAuthoring subsceneData) =>{
            var entity = GetPrimaryEntity(subsceneData);
            DstEntityManager.AddComponentData(entity, new SubSceneData{ subsceneName = subsceneData.subsceneName, shouldBeLoaded = subsceneData.shouldBeLoaded});
        });
    }
}
