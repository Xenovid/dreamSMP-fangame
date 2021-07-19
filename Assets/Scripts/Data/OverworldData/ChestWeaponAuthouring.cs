using Unity.Entities;
using UnityEngine;
using Unity.Collections;
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
              Entities.ForEach(( ChestWeaponAuthouring chestWeapon) =>{
                     Entity entity = GetPrimaryEntity(chestWeapon);
                     Weapon newWeapon = WeaponConversionSystem.WeaponInfoToWeapon(chestWeapon.weaponInfo);
                     DstEntityManager.AddComponentData(entity, new ChestWeaponData{weapon = newWeapon});
              });
       }
       
    
}
