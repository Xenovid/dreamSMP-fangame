using Unity.Entities;
using UnityEngine;
public class ChestItemAuthoring : MonoBehaviour{
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
        Entities
        .ForEach((ChestItemAuthoring chestItem) => {
            
            Entity entity = GetPrimaryEntity(chestItem);
            Item newItem = ItemConversionSystem.ItemInfoToItem(chestItem.item);
            DstEntityManager.AddComponentData(entity, new ChestItemData{
                item = newItem
            });
        });
    }

}
