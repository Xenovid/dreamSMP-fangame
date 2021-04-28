using Unity.Entities;
using Unity.Collections;
//using Unity.Mathematics;
using UnityEngine;

public class EnemyBattleAISystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        EntityQuery playerPartyDataGroup = GetEntityQuery(typeof(PlayerTag));
        NativeArray<Entity> playerPartyEntity = playerPartyDataGroup.ToEntityArray(Allocator.Temp);
        DynamicBuffer<PlayerPartyData> playerPartyData = GetBuffer<PlayerPartyData>(playerPartyEntity[0]);
        playerPartyEntity.Dispose();

        int numPlayer = playerPartyData.Length;

        Entities
        .WithAll<EnemySelectorUI>()
        .WithoutBurst()
        .ForEach((ref BattleData battleData, in RandomAIData randomAIData, in AnimationData animation, in Animator animator) =>
        {
            //have something to remember the ai type and do actions accordingly
            // have a list of battle attacks that you can choose
            //ai 1, normal timers, but randomly tick the timer down, and randomly choose an attack
            //ai 2, normal timer, randomly tick the timer down, but intellegently pick a random attack
            //ai 3, use a machine learning algorithm

            float randomValue = Random.value;
            int Target = Random.Range(1, numPlayer);
            Debug.Log(battleData.isDown);
            if (battleData.useTime <= 0 && !battleData.isDown)
            {
                foreach (RandomAttack attack in randomAIData.attacks)
                {
                    if (attack.chance >= randomValue)
                    {
                        battleData.selected = selectables.attack;
                        battleData.targetingId = playerPartyData[Target - 1].playerId;
                        battleData.damage = attack.damage;

                        animator.Play(attack.attackAnimation);

                        battleData.useTime = attack.useTime;
                        battleData.maxUseTime = attack.useTime;
                        //do attack
                    }
                    else
                    {
                        randomValue -= attack.chance;
                    }
                }
            }
            else
            {
                battleData.selected = selectables.none;
                battleData.useTime -= deltaTime * Random.value;
            }
        }).Run();
    }
}
