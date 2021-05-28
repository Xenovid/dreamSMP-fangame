using Unity.Entities;
using Unity.Collections;
using UnityEngine;


public class CharacterStatsAuthouring : MonoBehaviour{
    public Stats baseStats;
    [HideInInspector]
    public Stats battleStats;
    public int id;
    public string characterName;
    public WeaponInfo equipedWeapon;
    public ArmorInfo equipedArmor;
    public CharmInfo equipedCharm;
}

public struct CharacterStats : IComponentData
{
    public float maxHealth;
    public int maxPoints;
    public float health;
    public int points;
    public Stats baseStats;
    public Stats battleStats;
    public FixedString64 characterName;
    public int id;
    public Weapon equipedWeapon;
    public Armor equipedArmor;
    public Charm equipedCharm;
}
[System.Serializable]
public struct Stats{
    
    public int attack;
    public int defense;
    public int superArmor;
}
public class CharacterStatsConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((CharacterStatsAuthouring characterStat) => {
            var entity = GetPrimaryEntity(characterStat);
            DstEntityManager.AddComponentData(entity, new CharacterStats
            {
                baseStats = characterStat.baseStats,
            //character.equipedWeapon = characterStats.equipedWeapon;
                id = characterStat.id,
                characterName = characterStat.characterName,
                equipedWeapon =  WeaponConversionSystem.WeaponInfoToWeapon(characterStat.equipedWeapon),
                equipedArmor = ArmorConversionSystem.ArmorInfoToArmor(characterStat.equipedArmor),
                equipedCharm = CharmConversionSystem.CharmInfoToCharm(characterStat.equipedCharm)
            });
        });
    }
}

