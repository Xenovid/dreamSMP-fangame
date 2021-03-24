using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CutsceneTriggerData : IComponentData
{
    public tempCharacterDialogue[] characterDialogues;
    public float cutsceneLength;
    public string dialogueAudioName;
}

[System.Serializable]
public struct tempCharacterDialogue{
    public int id;
    public tempDialogueData[] dialogues;
}

[System.Serializable]
public struct tempDialogueData{
    public string dialogue;
    public float dialogueStartTime;
    public float dialogueChangeTime;
    public float keepDialogueUpTime;
}
