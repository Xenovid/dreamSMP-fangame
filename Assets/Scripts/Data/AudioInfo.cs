using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class AudioInfo : IComponentData
{
    public string audioName;
}
