using Unity.Entities;
using UnityEngine;
using System;

public class OverWorldHealingSystem : SystemBase
{
    public event EventHandler OnHealthChange;
    protected override void OnUpdate()
    {
        bool isHealthChanged = false;
        Entities
        .WithNone<BattleData>()
        .ForEach((ref CharacterStats characterStats, ref DynamicBuffer<HealingData> healings) => {
            for(int i = 0; i < healings.Length; i++){
                characterStats.health += healings[i].healing;
                if(characterStats.health > characterStats.maxHealth){
                    characterStats.health = characterStats.maxHealth;
                }
                healings.RemoveAt(i);
                i--;
                isHealthChanged = true;
            }
        }).Run();
        if(isHealthChanged){
                OnHealthChange?.Invoke(this, EventArgs.Empty);
        }
    }
}
