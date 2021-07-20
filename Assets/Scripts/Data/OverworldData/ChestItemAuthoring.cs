using Unity.Entities;
using Unity.Collections;
using UnityEngine;
public class ChestItemAuthoring : MonoBehaviour{
    public string itemName;
}
public struct ChestItemData : IComponentData
{
    public FixedString32 itemName;
}
public class ChestItemConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities
        .ForEach((ChestItemAuthoring chestItem) => {
            Entity entity = GetPrimaryEntity(chestItem);
            DstEntityManager.AddComponentData(entity, new ChestItemData{
                itemName = chestItem.itemName
            });
        });
    }

}
