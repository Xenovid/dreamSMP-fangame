using Unity.Entities;
using UnityEngine;

public class InventorySystem : SystemBase
{
    protected override void OnUpdate()
    {
        
    }
    public void AddWeapon(string weaponName){
        Entity caravan = GetSingletonEntity<CaravanTag>();
        DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);

        Entity allWeaponsHolder = GetSingletonEntity<AllItemTypesTag>();
        DynamicBuffer<WeaponData> allWeapons = GetBuffer<WeaponData>(allWeaponsHolder);

        foreach(WeaponData weaponData in allWeapons){
            if(weaponData.weapon.name == weaponName){
                Debug.Log("weapon found");
                weaponInventory.Insert(0, new WeaponData{weapon = weaponData.weapon});
                break;
            }
        }
        
    }
    public void AddItem(string itemName){
        DynamicBuffer<ItemData> items = EntityManager.GetBuffer<ItemData>(GetSingletonEntity<PlayerTag>());
    }
}
