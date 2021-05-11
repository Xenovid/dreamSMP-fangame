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
      MovementSystem movementSystem;
      protected override void OnCreate(){
            base.OnCreate();
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            isInBattle = false;
            movementSystem = World.GetOrCreateSystem<MovementSystem>();
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
                  // once all the enemies are down, the player has won
                  if(enemyEntities.Count == enemiesDown){
                        OnBattleEnd?.Invoke(this, new OnBattleEndEventArgs{isPlayerVictor = true});
                        playerEntities.Clear();
                        enemyEntities.Clear();
                  }
                  // need to add detection for when the player loses
                  else{
                        Entities
                        .WithStructuralChanges()
                        .WithoutBurst()
                        .ForEach((ref DynamicBuffer<DamageData> damages, ref BattleData battleData ,ref CharacterStats characterStats) => {
                              for(int i = 0; i < damages.Length; i++){
                                    Debug.Log("damage dealt");
                                    characterStats.health -= damages[i].damage;
                                    damages.RemoveAt(i);
                                    i--;
                              }
                              if(characterStats.health <= 0 && !battleData.isDown)
                              {
                                    Debug.Log("should be down");   
                                    //*** need to add down animation
                                    //do others stuff for when a temporary enemy is down
                                    battleData.isDown = true;
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
      // goes through the party of the character and the enemies conneceted to the triggering enemy and adds battle data to all of them
      // also adds them to the battlesystems player and enemy lists so that other systems know who is currently in a battle
      // triggers an event to let other systems know that the battle is starting
      public void StartBattle( Entity enemy){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            // start
            InputGatheringSystem.currentInput = CurrentInput.ui;
            World.GetOrCreateSystem<BattleMenuSystem>().Enabled = true;
            // adding the nessesary components for battle
            if(!isInBattle){
                  isInBattle = true;
                  // 
                  EntityQuery characterQuery = GetEntityQuery(typeof(CharacterStats));
                  NativeArray<CharacterStats> characters = characterQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
                  NativeArray<Entity> characterEntities = characterQuery.ToEntityArray(Allocator.TempJob);
                  // adding the battle data to the players
                  Entities
                  .WithoutBurst()
                  .WithStructuralChanges()
                  .ForEach((DynamicBuffer<PlayerPartyData> players) =>
                  {
                        for(int i = 0; i < players.Length; i++){
                        int j = 0;
                              foreach(CharacterStats characterStats in characters){
                                    if(players[i].playerId == characterStats.id){
                                          playerEntities.Add(characterEntities[j]);
                                    }
                                    j++;
                              }
                        }
                  }).Run();
                  // adding battle data to the enemies
                  DynamicBuffer<EnemyBattleData> enemies = GetBuffer<EnemyBattleData>(enemy);
                  for(int i = 0; i < enemies.Length; i++){
                        int j = 0;
                        foreach(CharacterStats characterStats in characters){
                              if(enemies[i].id == characterStats.id){
                                    enemyEntities.Add(characterEntities[j]);
                              }
                              j++;
                        }
                  }
                  isInBattle = true;
                  // trigger event, should be connected to battle menu and movement 
                  OnBattleStart?.Invoke(this, System.EventArgs.Empty);
                  // wait until the trasition ends to add battle data to everyone in the battle
                  movementSystem.OnTransitionEnd += AddBattleData_OnTransitionEnd;
                  characters.Dispose();
                  characterEntities.Dispose();
            }
      }
      public void AddBattleData_OnTransitionEnd(System.Object sender, System.EventArgs e){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            foreach(Entity entity in playerEntities){
                  ecb.AddComponent<BattleData>(entity);
            }
            foreach(Entity entity in enemyEntities){
                  ecb.AddComponent<BattleData>(entity);
            }
            movementSystem.OnTransitionEnd -= AddBattleData_OnTransitionEnd;
      }
}

public class OnBattleEndEventArgs : EventArgs{
      public bool isPlayerVictor {get; set;}
}
public delegate void BattleEndEventHandler(object sender, OnBattleEndEventArgs e);
