using Unity.Entities;
using UnityEngine;
public class ChestWeaponAuthouring : MonoBehaviour{
       public WeaponInfo weaponInfo;
}
public struct ChestWeaponData : IComponentData
{
       public Weapon weapon;
}
public class ChestWeaponConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
       Entities.ForEach((Entity entity, ChestWeaponAuthouring chestWeapon) =>{
              Weapon newWeapon = WeaponConversionSystem.WeaponInfoToWeapon(chestWeapon.weaponInfo);
              DstEntityManager.AddComponentData(entity, new ChestWeaponData{weapon = newWeapon});
       });
    }
}
