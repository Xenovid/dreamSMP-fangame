using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class BattleSystem : SystemBase
{

      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
      public bool isInBattle;
      public event BattleEndEventHandler OnBattleEnd;
      public event EventHandler OnBattleStart;
      public List<Entity> playerEntities = new List<Entity>();
      public List<Entity> enemyEntities = new List<Entity>();
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
                  //checks the number of enemies down to check if the battle should be down
                  foreach (Entity entity in enemyEntities)
                  {
                        CharacterStats character = GetComponent<CharacterStats>(entity);
                        if(character.health <= 0){
                              enemiesDown++;
                        }
                  }
                  if(enemyEntities.Count == enemiesDown){
                        OnBattleEnd?.Invoke(this, new OnBattleEndEventArgs{isPlayerVictor = true});
                        playerEntities.Clear();
                        enemyEntities.Clear();
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

      public void StartBattle( Entity enemy){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            InputGatheringSystem.currentInput = CurrentInput.ui;
            World.GetOrCreateSystem<BattleMenuSystem>().Enabled = true;
            // adding the nessesary components for battle
            if(!isInBattle){
                  isInBattle = true;
                  EntityQuery characterQuery = GetEntityQuery(typeof(CharacterStats));
                  NativeArray<CharacterStats> characters = characterQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
                  NativeArray<Entity> characterEntities = characterQuery.ToEntityArray(Allocator.TempJob);
                  Entities
                  .WithoutBurst()
                  .WithStructuralChanges()
                  .ForEach((DynamicBuffer<PlayerPartyData> players) =>
                  {
                        Debug.Log("added player");
                        for(int i = 0; i < players.Length; i++){
                        int j = 0;
                              foreach(CharacterStats characterStats in characters){
                                    if(players[i].playerId == characterStats.id){
                                          playerEntities.Add(characterEntities[j]);
                                          ecb.AddComponent<BattleData>(characterEntities[j]);
                                    }
                                    j++;
                              }
                        }
                  }).Run();
                  DynamicBuffer<EnemyBattleData> enemies = GetBuffer<EnemyBattleData>(enemy);
                  for(int i = 0; i < enemies.Length; i++){
                        int j = 0;
                        Debug.Log("added enemy");
                        foreach(CharacterStats characterStats in characters){
                              if(enemies[i].id == characterStats.id){
                                    enemyEntities.Add(characterEntities[j]);
                                    ecb.AddComponent<BattleData>(characterEntities[j]);
                              }
                              j++;
                        }
                  }
                  isInBattle = true;
                  OnBattleStart?.Invoke(this, System.EventArgs.Empty);
                  characters.Dispose();
                  characterEntities.Dispose();
            }
      }
}

public class OnBattleEndEventArgs : EventArgs{
      public bool isPlayerVictor {get; set;}
}
public delegate void BattleEndEventHandler(object sender, OnBattleEndEventArgs e);
