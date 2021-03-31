using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public struct SubSceneData : IComponentData
{
    public bool shouldBeLoaded;
}
