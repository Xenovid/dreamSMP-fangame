using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Scenes;

public class SubSceneReferences : MonoBehaviour
{
    public static SubSceneReferences Instance { get; private set;}

    public subSceneInfo[] subScenes;
    public SubScene pauseMenuSubScene;
    public SubScene WorldSubScene;

    private void Awake() {
        Instance = this;
    }

}

[System.Serializable]
public struct subSceneInfo{
    public SubScene subScene;
    public string name;
}
