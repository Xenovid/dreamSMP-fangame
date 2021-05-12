using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CameraData : IComponentData
{
    public Transform cameraTransform;
    public GameObject cameraObject;
}
