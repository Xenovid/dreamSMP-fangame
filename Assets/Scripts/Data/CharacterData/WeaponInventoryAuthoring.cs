using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class WeaponInventoryAuthoring : MonoBehaviour
{
    public WeaponInfo[] weaponInfos;
}
[System.Serializable]
public struct WeaponData : IBufferElementData{
    public Weapon weapon;
}

public class WeaponConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities
        .ForEach((WeaponInventoryAuthoring weaponInventory) => {
            Entity entity = GetPrimaryEntity(weaponInventory);
            DstEntityManager.AddBuffer<WeaponData>(entity);
            DynamicBuffer<WeaponData> WeaponInventory = DstEntityManager.GetBuffer<WeaponData>(entity);
            foreach(WeaponInfo weaponInfo in weaponInventory.weaponInfos){
                Weapon weapon = WeaponInfoToWeapon(weaponInfo);
                WeaponInventory.Add(new WeaponData{weapon = weapon});
            }
        });
    }

    public static Weapon WeaponInfoToWeapon(WeaponInfo weaponInfo){
        return new Weapon{
            power = weaponInfo.power,
            name = weaponInfo.name,
            description = weaponInfo.description,
        };
    }
    
}

[System.Serializable]
public struct WeaponInfo{
    public int power;
    public string name;
    [TextArea]
    public string description;
}
[System.Serializable]
public struct Weapon {
    public int power;
    public FixedString32 name;
    public FixedString128 description;
}
