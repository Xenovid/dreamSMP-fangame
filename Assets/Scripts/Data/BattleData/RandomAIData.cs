using Unity.Entities;

[GenerateAuthoringComponent]
public class RandomAIData : IComponentData
{
    public RandomAttack[] attacks;
}

[System.Serializable]
public struct RandomAttack
{
    public float chance;
    public float damage;
    public string attackAnimation;
    public float useTime;
}