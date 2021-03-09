using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public struct DialogueData : IBufferElementData
{
    public FixedString512 dialogue;
    public float dialogueStartTime;
    public float dialogueEndTime;
    public float keepDialogueUpTime;
}
