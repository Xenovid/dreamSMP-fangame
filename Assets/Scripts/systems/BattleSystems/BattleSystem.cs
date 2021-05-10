using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using System;

public class BattleSystem : SystemBase
{

      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
      public bool isInBattle;
      public event EventHandler OnBattleEnd;
      public event EventHandler OnBattleStart;
      protected override void OnCreate(){
            base.OnCreate();
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            isInBattle = false;
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

            if(isInBattle){
                  //from the main enemy, get the rest of the enemies
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
                        OnBattleEnd?.Invoke(this, new OnBattleEndEventArgs{isPlayerVictor = true});
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

      public void StartBattle(Entity[] players, Entity[] enemys){
            //!change to record more data
            if(!isInBattle){
                  isInBattle = true;
                  OnBattleStart?.Invoke(this, new OnBattleStartArgs{players = players, enemys = enemys});
            }
      }
}

public class OnBattleEndEventArgs : EventArgs{
      public bool isPlayerVictor {get; set;}
}
public class OnBattleStartArgs : EventArgs{
      public Entity[] players;
      public Entity[] enemys;
}
