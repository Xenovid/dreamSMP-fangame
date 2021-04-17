using Unity.Collections;
using Unity.Entities;
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
        
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            EntityQuery battleCharacterGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<Entity> battleCharacters = battleCharacterGroup.ToEntityArray(Allocator.TempJob);

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity battleManagerEntity, BattleManagerData battleManager) =>{
                  // if the player wins, give them some awards and give them some kind 
                  // if the player loses, set them to their last respawn point

                  //if a character loses or wins, say the results
                  if(battleManager.hasPlayerWon){
                    
                    foreach (Entity entity in battleCharacters){
                              ecb.RemoveComponent<BattleData>(entity);
                              if(GetComponentDataFromEntity<BattleMenuTag>().HasComponent(entity)){
                                    
                              }
                        }
                        ecb.RemoveComponent<BattleManagerData>(battleManagerEntity);
                        ecb.AddComponent<VictoryData>(battleManagerEntity);
                }
            }).Run();

            battleCharacters.Dispose();
      }
}