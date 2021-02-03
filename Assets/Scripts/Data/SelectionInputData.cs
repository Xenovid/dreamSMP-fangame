using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SelectionInputData : IComponentData
{
    public int isSelectedOrBack;
}
