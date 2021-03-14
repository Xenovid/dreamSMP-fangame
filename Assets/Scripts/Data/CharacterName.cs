using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CharacterName : IComponentData
{
    public string name;
}
