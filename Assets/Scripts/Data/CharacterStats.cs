using Unity.Entities;
using Unity.Collections;
using UnityEngine;
/*
public class CharacterStatsAuthouring : MonoBehaviour{
    public float health;
    public float recoverTime;
    public int attackMultiplier;
    public Weapon equipedWeapon; 
    public int id;
    public string charname;
}
*/
[GenerateAuthoringComponent]
public struct CharacterStats : IComponentData
{
    public float health;
    public float recoverTime;
    public int attackMultiplier;
    public Weapon equipedWeapon;
    public int id;
}
/*
public class CharacterStatsConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((CharacterStatsAuthouring characterStats) => {
            var entity = GetPrimaryEntity(characterStats);
            DstEntityManager.AddComponent<CharacterStats>(entity);
            CharacterStats character = DstEntityManager.GetComponentData<CharacterStats>(entity);
            character.health = characterStats.health;
            character.recoverTime = characterStats.recoverTime;
            character.attackMultiplier = characterStats.attackMultiplier;
            character.equipedWeapon = characterStats.equipedWeapon;
            character.id = characterStats.id;
            character.name = characterStats.charname;
        });
    }
}

*/

public struct Weapon{
    public WeaponType weaponType;
    public float attack;
    public float attackSpeed;
}
public enum WeaponType{
    axe,
    sword
}
