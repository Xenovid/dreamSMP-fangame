using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct DelayedInputData : IComponentData
{
    [HideInInspector]
    public bool isSelectPressed;
    [HideInInspector]
    public bool wasSelectPressed;
}
