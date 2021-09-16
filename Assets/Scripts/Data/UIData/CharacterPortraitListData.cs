using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
[GenerateAuthoringComponent]
public class CharacterPortraitListData : IComponentData
{
    public List<CharacterPortraitReference> characterPortraitList = new List<CharacterPortraitReference>();
}
[System.Serializable]
public struct UIAnimationInfo{
    public float initialDelay;
	public bool active;
	public float time;
    public int index;
    public float spritePerSecond;
}
[System.Serializable]
public struct CharacterPortraitReference{
    public string characterName;
    public List<CharacterPortraitData> characterPortraitList;
}
