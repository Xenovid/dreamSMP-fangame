using Unity.Entities;
using UnityEngine;
using UnityEngine.Playables;
[GenerateAuthoringComponent]
public class PlayableData : IComponentData
{
    public string name;
    public int index;
}
