using Unity.Entities;
using Unity.Collections;
using UnityEngine;


public class CharacterStatsAuthouring : MonoBehaviour{
    public float maxHealth;
    public float health;
    public float recoverTime;
    public int attackMultiplier;
    public Weapon equipedWeapon; 
    public int id;
    public string characterName;
}

public struct CharacterStats : IComponentData
{
    public FixedString64 characterName;
    public float maxHealth;
    public float health;
    public float recoverTime;
    public int attackMultiplier;
    public int id;
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
                characterName = characterStat.characterName

            });
        });
    }
}


