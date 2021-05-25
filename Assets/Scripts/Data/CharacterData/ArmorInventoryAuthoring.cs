using Unity.Entities;
using UnityEngine;
using Unity.Collections;

public class ArmorInventoryAuthoring : MonoBehaviour
{
    public ArmorInfo[] armorInfos;
}

public struct ArmorData : IBufferElementData{
    public Armor armor;
}

public class ArmorConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach(( ArmorInventoryAuthoring armorInventory) => {
            Entity entity = GetPrimaryEntity(armorInventory);
            DstEntityManager.AddBuffer<ArmorData>(entity);
            DynamicBuffer<ArmorData> ArmorInventory = DstEntityManager.GetBuffer<ArmorData>(entity);
            foreach(ArmorInfo armorInfo in armorInventory.armorInfos){
                Armor armor = new Armor{
                    defense = armorInfo.defense,
                    name = armorInfo.name,
                    description = armorInfo.description,
                };
                ArmorInventory.Add(new ArmorData{armor = armor});
            }
        });
    }
    
    public static Armor ArmorInfoToArmor(ArmorInfo armorInfo){
        return new Armor{
            defense = armorInfo.defense,
            name = armorInfo.name,
            description = armorInfo.description,
        };
    }
}
[System.Serializable]
public struct ArmorInfo{
    public string name;
    public int defense;
    public string description;
}
[System.Serializable]
public struct Armor{
    public FixedString32 name;
    public int defense;
    public FixedString128 description;
}
