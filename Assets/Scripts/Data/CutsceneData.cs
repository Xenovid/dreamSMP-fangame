using Unity.Entities;
using UnityEngine;

public struct CutsceneData : IComponentData
{
    public bool isReadingDialogue;
    
    public int currentDialogue;
    public int currentChar;

    public float totalTime;
    public float timeWaited;
}
