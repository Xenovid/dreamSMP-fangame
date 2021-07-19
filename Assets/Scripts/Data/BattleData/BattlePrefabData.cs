using Unity.Entities;

public struct BattlePrefabData : IComponentData
{
    public Entity target;
    public Entity user;
}
