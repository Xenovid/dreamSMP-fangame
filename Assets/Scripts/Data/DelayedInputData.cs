using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct DelayedInputData : IComponentData
{
    [HideInInspector]
    public bool isSelectPressed;
    [HideInInspector]
    public bool wasSelectPressed;

    [HideInInspector]
    public bool wasUpPressed;
    [HideInInspector]
    public bool isUpPressed;

    [HideInInspector]
    public bool wasDownPressed;
    [HideInInspector]
    public bool isDownPressed;

    [HideInInspector]
    public bool wasRightPressed;
    [HideInInspector]
    public bool isRightPressed;

    [HideInInspector]
    public bool wasLeftPressed;
    [HideInInspector]
    public bool isLeftPressed;

    [HideInInspector]
    public bool isBackPressed;
    [HideInInspector]
    public bool wasBackPressed;

    [HideInInspector]
    public bool isEscapePressed;
    [HideInInspector]
    public bool wasEscapePressed;
}
