using Unity.Entities;
using Unity.Collections;
using UnityEngine;


public class CharacterStatsAuthouring : MonoBehaviour{
    public float maxHealth;
    public float health;
    public float maxPoints;
    public float points;

    public Stats baseStats;
    public string characterName;
    public WeaponInfo equipedWeapon;
    public ArmorInfo equipedArmor;
    public CharmInfo equipedCharm;
}
[System.Serializable]
public struct CharacterStats : IComponentData
{
    public float maxHealth;
    public float maxPoints;
    public float health;
    public float points;
    public Stats baseStats;
    public FixedString64 characterName;
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
            DstEntityManager.AddBuffer<DamageData>(entity);
            DstEntityManager.AddComponentData(entity, new CharacterStats
            {
                maxHealth = characterStat.maxHealth,
                health = characterStat.health,
                maxPoints = characterStat.maxPoints,
                points = characterStat.points,
                baseStats = characterStat.baseStats,
                characterName = characterStat.characterName,
                equipedWeapon =  WeaponConversionSystem.WeaponInfoToWeapon(characterStat.equipedWeapon),
                equipedArmor = ArmorConversionSystem.ArmorInfoToArmor(characterStat.equipedArmor),
                equipedCharm = CharmConversionSystem.CharmInfoToCharm(characterStat.equipedCharm)
            });
        });
    }
}

