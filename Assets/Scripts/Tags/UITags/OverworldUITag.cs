using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct OverworldUITag : IComponentData
{
    [HideInInspector]
    public bool isNextToInteractive;
    [HideInInspector]
    public bool wasNextToInteractive;
    // not hide in the inspector since I want to default to true
    public bool isVisable;
}
