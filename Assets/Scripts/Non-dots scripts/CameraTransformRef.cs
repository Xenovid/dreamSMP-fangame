using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransformRef : MonoBehaviour
{
    //holds the transform that can be interacted with in the entity world

    public static CameraTransformRef instance;
    //don't need a reference to transform since it will already have a reference by default
    public new Transform transform;
    public Camera currentCamera;
    private void Start() {
        instance = this;
    }
}
