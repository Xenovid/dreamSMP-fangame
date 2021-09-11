using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct CameraData : IComponentData
{
    /// <summary>
    /// whether the camera should be following the character
    /// </summary>
    public CameraState currentState;
}

public enum CameraState{
    FollingPlayer,
    FreeForm
}
