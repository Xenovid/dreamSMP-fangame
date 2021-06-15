using Unity.Entities;
using UnityEngine;
public class ChestArmorAuthouring : MonoBehaviour{
    public ArmorInfo armorInfo;
}
public struct ChestArmorData : IComponentData
{
    public Armor armor;
}
public class ChestArmorConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach(( ChestArmorAuthouring chestArmor) =>{
            Entity entity = GetPrimaryEntity(chestArmor);
            Armor newArmor = new Armor{
                defense = chestArmor.armorInfo.defense,
                name = chestArmor.armorInfo.name,
                description = chestArmor.armorInfo.description,
            };
            DstEntityManager.AddComponentData(entity, new ChestArmorData{armor = newArmor});
        });
    }
}
