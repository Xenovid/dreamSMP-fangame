using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public class CameraData : IComponentData
{
    public GameObject camera;
    public float3 offset;
    public Entity player;
}
