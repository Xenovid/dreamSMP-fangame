using Unity.Entities;

public struct BleedingData : IComponentData
{
    public int level;
    public float timeFromLastDamageTick;
}
