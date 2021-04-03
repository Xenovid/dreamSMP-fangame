using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Scenes;

public class TitleSubSceneReferences : MonoBehaviour
{
    public static TitleSubSceneReferences Instance { get; private set;}

    public SubScene titleSubScene;

    public SubScene OptionSubScene;

    public SubScene CreditsSubScene;

    private void Awake() {
          Instance = this;
    }
}
