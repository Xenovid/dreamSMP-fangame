using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
[GenerateAuthoringComponent]
public class CharacterPortraitData : IComponentData
{
    [HideInInspector]
    public List<Sprite> portraits = new List<Sprite>();
}
