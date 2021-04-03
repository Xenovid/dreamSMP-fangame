using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

[GenerateAuthoringComponent]
public class SubSceneReference : IComponentData
{
    public SubScene subScene;
}
