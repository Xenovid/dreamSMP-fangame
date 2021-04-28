using Unity.Entities;
using UnityEngine;

public class CharacterDownSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach(( SpriteRenderer sprite, ref BattleData battleData, in CharacterStats characterStats) =>
        {
            if(characterStats.health <= 0 && !battleData.isDown)
            {
                sprite.enabled = false;
                Debug.Log("should be down");   
                //*** need to add down animation

                //do others stuff for when a temporary enemy is down
            }
            battleData.isDown = characterStats.health <= 0;
        }).Run();
    }
}
