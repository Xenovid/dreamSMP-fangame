using Unity.Entities;


[GenerateAuthoringComponent]
public struct TechnoData : IComponentData
{
    public float timeFromLastDamageTick;
}
