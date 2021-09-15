using Unity.Entities;
[GenerateAuthoringComponent]
public struct ToLoadData : IComponentData
{
    [UnityEngine.HideInInspector]
    public bool waitedFrame;
    public int saveID;
}
