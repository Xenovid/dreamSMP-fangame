using Unity.Entities;

[GenerateAuthoringComponent]
public struct CutsceneTriggerTag : IComponentData
{
    public bool isTriggered;
}
