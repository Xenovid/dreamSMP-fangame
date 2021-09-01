using Unity.Entities;
[GenerateAuthoringComponent]
public struct AlphaTranslationData : IComponentData
{
    public float timePassed;
    public float a;
    public float b;
}
