using Unity.Entities;
using UnityEngine;

public class EnemyBattleAISystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithAll<EnemySelectorUI>()
            .ForEach((ref BattleData battleData) =>
        {
            EnemyAiTypes temp = EnemyAiTypes.random;
            //have something to remember the ai type and do actions accordingly
            // have a list of battle attacks that you can choose
            //ai 1, normal timers, but randomly tick the timer down, and randomly choose an attack
            //ai 2, normal timer, randomly tick the timer down, but intellegently pick a random attack
            //ai 3, use a machine learning algorithm

            switch (temp)
            {
                case EnemyAiTypes.random:

                    battleData.useTime -= Random.value;
                    break;
            }
            
        }).Run();
    }
}

public enum EnemyAiTypes
{
    random,
    inteligentRandom,
    machine
}
