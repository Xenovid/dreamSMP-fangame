using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CutsceneData : IComponentData
{
    public string cutsceneName;
}
