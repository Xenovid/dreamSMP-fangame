using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class WeaponInventoryAuthoring : MonoBehaviour
{
    public WeaponInfo[] weaponInfos;
}

public struct WeaponData : IBufferElementData{
    public Weapon weapon;
}

public class WeaponConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, WeaponInventoryAuthoring weaponInventory) => {
            DstEntityManager.AddBuffer<WeaponData>(entity);
            DynamicBuffer<WeaponData> WeaponInventory = DstEntityManager.GetBuffer<WeaponData>(entity);
            foreach(WeaponInfo weaponInfo in weaponInventory.weaponInfos){
                Weapon weapon = new Weapon{
                    power = weaponInfo.power,
                    name = weaponInfo.name,
                    description = weaponInfo.description,
                    useTime = weaponInfo.useTime
                };
                WeaponInventory.Add(new WeaponData{weapon = weapon});
            }
        });
    }
}

[System.Serializable]
public struct WeaponInfo{
    public int power;
    public string name;
    [TextArea]
    public string description;
    public float useTime;
}
[System.Serializable]
public struct Weapon{
    public int power;
    public FixedString32 name;
    public FixedString128 description;
    public float useTime;
}
