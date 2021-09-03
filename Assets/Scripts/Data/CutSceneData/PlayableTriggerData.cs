using Unity.Entities;
using UnityEngine.Playables;
using UnityEngine;
[GenerateAuthoringComponent]
public class PlayableTriggerData : IComponentData
{
    public PlayableAsset playableAsset;
    public bool isTriggered;
}
