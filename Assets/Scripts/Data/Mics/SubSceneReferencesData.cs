using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

[GenerateAuthoringComponent]
public class SubSceneReferencesData : IComponentData
{
    public SubSceneInfo[] subSceneInfos;
}

[System.Serializable]
public struct SubSceneInfo{
    public SceneReference subScene;
    public string name;
}
