using Unity.Entities;

public struct EnemySelectorData : IComponentData
{
    public bool isSelected;
    public int enemyId;
    public bool isDead;
}
