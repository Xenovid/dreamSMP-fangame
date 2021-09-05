using Unity.Entities;
using System.Collections;
using System;
using UnityEngine.Playables;
using Unity.Collections;
using UnityEngine;
public class PlayableTriggerDataAuthoring :  MonoBehaviour {
    [SerializeReference]
    private string playableName;
    public int index;
}

public class PlayableTriggerData : IComponentData
{
    public int index;
    
    public bool isTriggered;
}

public class PlayableTriggerConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PlayableTriggerDataAuthoring playableTriggerDataAuthoring) =>{
            Entity entity = GetPrimaryEntity(playableTriggerDataAuthoring);
            DstEntityManager.AddComponentData(entity, new PlayableTriggerData{index = playableTriggerDataAuthoring.index, isTriggered = false});
            /*if(playableTriggerDataAuthoring.playableManager.playableAssets.Contains(playableTriggerDataAuthoring.playableAsset)){
                int index = playableTriggerDataAuthoring.playableManager.playableAssets.LastIndexOf(playableTriggerDataAuthoring.playableAsset);
                DstEntityManager.AddComponentData(entity, new PlayableTriggerData{assetNumber = index, isTriggered = false});
            }   
            else{
                int index = playableTriggerDataAuthoring.playableManager.playableAssets.Count;
                playableTriggerDataAuthoring.playableManager.playableAssets.Add(playableTriggerDataAuthoring.playableAsset);
                DstEntityManager.AddComponentData(entity, new PlayableTriggerData{assetNumber = index, isTriggered = false});
            }*/
        });
    }
}
