using Unity.Entities;
using UnityEngine;
[GenerateAuthoringComponent]
public class ChestAnimationData : IComponentData
{
    public string openAnimationName;
    public string closedAnimationName;
}
