using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class BattleRewardSystem : SystemBase
{
      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

      protected override void OnStartRunning(){
            base.OnStartRunning();

            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
      }

      protected override void OnUpdate()
      {
            float deltaTime = Time.DeltaTime;
        
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            EntityQuery battleCharacterGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<Entity> battleCharacters = battleCharacterGroup.ToEntityArray(Allocator.TempJob);

            EntityQuery beforeBattleGroup = GetEntityQuery(typeof(BeforeBattleData));
            NativeArray<Entity> beforeBattleDatas = beforeBattleGroup.ToEntityArray(Allocator.TempJob);

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity battleManagerEntity, BattleManagerData battleManager) =>{
                  // if the player wins, give them some awards and give them some kind 
                  // if the player loses, set them to their last respawn point

                  //if a character loses or wins, say the results
                  if(battleManager.hasPlayerWon){
                        battleManager.translationTimePast += deltaTime;
                        foreach (Entity entity in battleCharacters){
                              ecb.RemoveComponent<BattleData>(entity);
                                
                        }
                    ecb.RemoveComponent<BattleManagerData>(battleManagerEntity);
                        for(int i = 0; i < beforeBattleDatas.Length; i++)
                        {
                            BeforeBattleData beforeBattleData = GetComponent<BeforeBattleData>(beforeBattleDatas[i]);
                            beforeBattleData.shouldChangeBack = true;
                            ecb.AddComponent<VictoryData>(beforeBattleDatas[i]);
                            ecb.AddComponent<TextBoxData>(beforeBattleDatas[i]);
                            SetComponent<BeforeBattleData>(beforeBattleDatas[i], beforeBattleData);
                        }
                  }

            }).Run();


            beforeBattleDatas.Dispose();
            battleCharacters.Dispose();
      }
}