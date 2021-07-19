using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TextBoxData : IComponentData
{
    public float textSpeed;
    [HideInInspector]
    public int currentChar;
    [HideInInspector]
    public bool isFinishedPage;
    [HideInInspector]
    public float timeFromLastChar;
    [HideInInspector]
    public bool isDisplaying;
}
