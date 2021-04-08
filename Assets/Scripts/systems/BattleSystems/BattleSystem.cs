using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class BattleSystem : SystemBase
{
      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
      protected override void OnCreate(){
            base.OnCreate();
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      }

      protected override void OnUpdate()
      {

            EntityManager.CompleteAllJobs();
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            EntityQuery BattleManagerGroup = GetEntityQuery(ComponentType.ReadWrite<BattleManagerTag>());
            EntityQuery characterStatsGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            EntityQuery PlayerParty = GetEntityQuery(ComponentType.ReadWrite<PlayerPartyData>());


            NativeArray<Entity> battleManagers = BattleManagerGroup.ToEntityArray(Allocator.Temp);
            
            NativeArray<CharacterStats> characterStatsList = characterStatsGroup.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
            NativeArray<Entity> characterEntities = characterStatsGroup.ToEntityArray(Allocator.TempJob);

            //int playersDown = 0;
            int enemiesDown = 0;

            if(battleManagers.Length > 0){
                  //from each battle manager get the ids of all the enemies, makes it easier to know what team is what
                  DynamicBuffer<EnemyBattleData> enemyIds = GetBuffer<EnemyBattleData>(battleManagers[0]);

                  //checks the number of enemies down to check if the battle should be down
                  foreach(CharacterStats character in characterStatsList){
                        for(int i = 0; i < enemyIds.Length; i++){
                              if(character.id == enemyIds[i].id && character.health <= 0){
                                    enemiesDown++;
                              }
                        }
                  }
                  if(enemyIds.Length == enemiesDown){
                        Entities
                        .WithoutBurst()
                        .ForEach((Entity entity, BattleManagerData battleManager) => {
                              battleManager.hasPlayerWon = true;
                        }).Run();
                        Debug.Log("all enemies down");
                  }
                  else{
                        Entities
                        .WithStructuralChanges()
                        .WithoutBurst()
                        .ForEach((ref BattleData battleData ,ref CharacterStats characterStats) => {
                              //-- need to add some kind of detection to tell if a team lost
                              //switches based on what a character seletcted
                              switch(battleData.selected){
                                    //when the selectable is attacking, find enemy that is selected to be attacked and put data on it to take damage
                                    case selectables.attack:
                                          int i = 0;
                                          //going through each possible character with battle data                
                                          foreach(CharacterStats character in characterStatsList){
                                                //if the the id of the character matches the target, deal damage to the character
                                                if( character.id.Equals(battleData.targetingId)){
                                                      EntityManager.AddComponentObject(characterEntities[i], new DamageData{damage = battleData.damage});
                                                }
                                                i++;
                                          }
                                          break;
                                    //for when the selected action is items
                                    case selectables.items:

                                          break;
                                    // for when the selected action is running
                                    case selectables.run:

                                          break;
                                    // when nothing is selected
                                    case selectables.none:
                                          break;
                        }
                        }).Run();
                  }
            }

            EntityManager.CompleteAllJobs();
            characterEntities.Dispose();
            characterStatsList.Dispose();
            battleManagers.Dispose();
            m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
      }
}
