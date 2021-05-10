using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TextBoxData : IComponentData
{
    public int currentChar;
    public bool isFinishedPage;
    public float timeFromLastChar;
    [HideInInspector]
    public FixedString128 currentSentence;
}
