using Unity.Entities;
using UnityEngine;
public class ChestItemAuthouring : MonoBehaviour{
    public ItemInfo item;
}
public struct ChestItemData : IComponentData
{
    public Item item;
}
public class ChestItemConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        
        Entities.ForEach((ChestItemAuthouring chestItem) => {
            Entity entity = GetPrimaryEntity(chestItem);
            Item newItem = new Item{
                itemType = chestItem.item.itemType,
                name = chestItem.item.name,
                description = chestItem.item.description,
                useTime = chestItem.item.useTime,
                strength = chestItem.item.strength
            };
            EntityManager.AddComponentData(entity, new ChestItemData{
                item = newItem
            });
        });
    }
}
