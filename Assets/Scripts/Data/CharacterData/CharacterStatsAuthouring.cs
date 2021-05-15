using Unity.Entities;
using Unity.Collections;
using UnityEngine;


public class CharacterStatsAuthouring : MonoBehaviour{
    public float maxHealth;
    public float health;
    public float recoverTime;
    public int attackMultiplier;
    public int resources;
    public int maxResources;
    public ResourceType resourceType;
    public int id;
    public string characterName;
    public WeaponInfo equipedWeapon;
    public ArmorInfo equipedArmor;
    public CharmInfo equipedCharm;
}

public struct CharacterStats : IComponentData
{
    public FixedString64 characterName;
    public float maxHealth;
    public float health;
    public float recoverTime;
    public int attackMultiplier;
    public int id;
    public Weapon equipedWeapon;
    public Armor equipedArmor;
    public Charm equipedCharm;
    public int resources;
    public ResourceType resourceType;
    public int maxResources;
}


public class CharacterStatsConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((CharacterStatsAuthouring characterStat) => {
            var entity = GetPrimaryEntity(characterStat);
            DstEntityManager.AddComponentData(entity, new CharacterStats
            {
                health = characterStat.health,
                maxHealth = characterStat.maxHealth,
                recoverTime = characterStat.recoverTime,
                attackMultiplier = characterStat.attackMultiplier,
            //character.equipedWeapon = characterStats.equipedWeapon;
                id = characterStat.id,
                characterName = characterStat.characterName,
                equipedWeapon =  WeaponConversionSystem.WeaponInfoToWeapon(characterStat.equipedWeapon),
                equipedArmor = ArmorConversionSystem.ArmorInfoToArmor(characterStat.equipedArmor),
                equipedCharm = CharmConversionSystem.CharmInfoToCharm(characterStat.equipedCharm),
                resources = characterStat.resources,
                resourceType = characterStat.resourceType,
                maxResources = characterStat.maxResources
            });
        });
    }
}

public enum ResourceType{
    blood,
    mana
}

