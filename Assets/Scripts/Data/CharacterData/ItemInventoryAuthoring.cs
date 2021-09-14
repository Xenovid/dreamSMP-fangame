using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ItemInventoryAuthoring : MonoBehaviour
{
    public ItemInfo[] itemInfos;
}
[System.Serializable]
public struct ItemData : IBufferElementData{
    public Item item;
}
public class ItemConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach(( ItemInventoryAuthoring itemInventory) => {
            Entity entity = GetPrimaryEntity(itemInventory);
            DstEntityManager.AddBuffer<ItemData>(entity);
            DynamicBuffer<ItemData> ItemInventory = DstEntityManager.GetBuffer<ItemData>(entity);

            foreach(ItemInfo itemInfo in itemInventory.itemInfos){
                Item item = ItemInfoToItem(itemInfo);
                ItemInventory.Add(new ItemData{item = item});
            }
        });
    }
    public static Item ItemInfoToItem(ItemInfo itemInfo){
        Item item = new Item{
            itemType = itemInfo.itemType,
            name = itemInfo.name,
            description = itemInfo.description,
            useTime = itemInfo.useTime,
            healingAmount = itemInfo.healingAmount,
            regenerationRate = itemInfo.regenerationRate,
            regenerationDuration = itemInfo.regenerationDuration
        };
        return item;
    }
}
public enum ItemType{
    none,
    healing,
    statsboost,
    damage

}
[System.Serializable]
public struct ItemInfo{
    public ItemType itemType;
    public string name;
    public string description;
    public int healingAmount;
    public int regenerationRate;
    public float regenerationDuration;
    public float useTime;
}
[System.Serializable]
public struct Item{
    public ItemType itemType;
    public FixedString32 name;
    public FixedString128 description;
    public int healingAmount;
    public int regenerationRate;
    public float regenerationDuration;
    public float useTime;
}
