using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

public class BattleSystem : SystemBase
{

      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
      public bool isInBattle;
      public event BattleEndEventHandler OnBattleEnd;
      public event EventHandler OnBattleStart;
      public List<Entity> playerEntities = new List<Entity>();
      public List<Entity> enemyEntities = new List<Entity>();
      TransitionSystem transitionSystem;
      protected override void OnCreate(){
            base.OnCreate();
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            isInBattle = false;
            transitionSystem = World.GetOrCreateSystem<TransitionSystem>();
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

            int enemiesDown = 0;
            int playersDown = 0;

            if(isInBattle){
                  float dt = Time.DeltaTime;
                  //from the main enemy, get the rest of the enemies
                  //checks the number of enemies down to check if the battle should be down
                  foreach (Entity entity in enemyEntities)
                  {
                        CharacterStats character = GetComponent<CharacterStats>(entity);
                        if(character.health <= 0){
                              enemiesDown++;
                        }
                  }
                  
                  foreach(Entity entity in playerEntities){
                        if(HasComponent<DownTag>(entity)){
                              playersDown++;
                        }
                  }
                  // once all the enemies are down, the player has won
                  if(enemyEntities.Count == enemiesDown){
                        EndBattle(true);
                  }
                  else if(playerEntities.Count == playersDown){
                        EndBattle(false);
                  }
                  
                  // need to add detection for when the player loses
                  else{
                        Entities
                        .WithStructuralChanges()
                        .WithoutBurst()
                        .WithNone<DownTag, TechnoData>()
                        .ForEach((Entity entity, ref DynamicBuffer<DamageData> damages, ref BattleData battleData,ref CharacterStats characterStats) => {
                              DynamicBuffer<HealingData> healings = GetBuffer<HealingData>(entity);
                              for(int i = 0; i < damages.Length; i++){
                                    if(EntityManager.HasComponent<HeadsUpUIData>(entity)){
                                          HeadsUpUIData headsUpUI = EntityManager.GetComponentObject<HeadsUpUIData>(entity);
                                          RandomData random = EntityManager.GetComponentData<RandomData>(entity);
                                          
                                          Label label = new Label();
                                          label.text = damages[i].damage.ToString();
                                          switch( damages[i].type){
                                                case damageType.bleeding:
                                                      label.AddToClassList("message_red");
                                                break;
                                                case damageType.physical:
                                                      label.AddToClassList("message_white");
                                                break;
                                          }
                                          headsUpUI.UI.Q<VisualElement>("messages").Add(label);
                                          
                                          Message message = new Message{timePassed = 0, label = label, direction = random.Value.NextFloat2Direction()};
                                          headsUpUI.messages.Add(message);
                                    }
                                    characterStats.health -= damages[i].damage;
                                    damages.RemoveAt(i);
                                    i--;
                              }
                              for(int i = 0; i < healings.Length; i++){
                                    characterStats.health += healings[i].healing;
                                    if(characterStats.health > characterStats.maxHealth){
                                          characterStats.health = characterStats.maxHealth;
                                    }
                                    healings.RemoveAt(i);
                                    i--;
                              }
                              if(characterStats.health <= 0 && !battleData.isDown)
                              { 
                                    //*** need to add down animation
                                    //do others stuff for when a temporary enemy is down
                                    ecb.AddComponent<DownTag>(entity);
                              }
                        }).Run();
                        Entities
                        .WithStructuralChanges()
                        .WithoutBurst()
                        .WithNone<DownTag>()
                        .ForEach((HeadsUpUIData headsUpUI, Entity entity,ref TechnoData techno, ref RandomData random, ref DynamicBuffer<DamageData> damages, ref BattleData battleData,ref CharacterStats characterStats) => {
                              DynamicBuffer<HealingData> healings = GetBuffer<HealingData>(entity);
                              for(int i = 0; i < damages.Length; i++){
                                    
                                    Label label = new Label();
                                    label.text = damages[i].damage.ToString();
                                    switch( damages[i].type){
                                          case damageType.bleeding:
                                                label.AddToClassList("message_red");
                                          break;
                                          case damageType.physical:
                                                label.AddToClassList("message_white");
                                          break;
                                    }
                                    headsUpUI.UI.Q<VisualElement>("messages").Add(label);
                                    
                                    Message message = new Message{timePassed = 0, label = label, direction = random.Value.NextFloat2Direction()};
                                    headsUpUI.messages.Add(message);
                                    if(characterStats.health <= 0){
                                          techno.timeFromLastDamageTick += dt;
                                          if(techno.timeFromLastDamageTick > 2){
                                                Label bloodDrain = new Label();
                                                bloodDrain.AddToClassList("message_red");
                                                techno.timeFromLastDamageTick = 0;
                                                headsUpUI.messages.Add(new Message{timePassed = 0, label = bloodDrain});
                                          }
                                          characterStats.points -=  damages[i].damage;
                                    }
                                    else{
                                          characterStats.health -= damages[i].damage;
                                          if(characterStats.health < 0){
                                                characterStats.health = 0;
                                          }
                                    }
                                    
                                    damages.RemoveAt(i);
                                    i--;
                              }
                              for(int i = 0; i < healings.Length; i++){
                                    characterStats.health += healings[i].healing;
                                    if(characterStats.health > characterStats.maxHealth){
                                          characterStats.health = characterStats.maxHealth;
                                    }
                                    healings.RemoveAt(i);
                                    i--;
                              }
                              if(characterStats.health <= 0 && characterStats.points <= 0)
                              { 
                                    characterStats.points = 0;
                                    ecb.AddComponent<DownTag>(entity);
                              }
                        }).Run();
                  }
            }

            //EntityManager.CompleteAllJobs();
            characterEntities.Dispose();
            characterStatsList.Dispose();
            battleManagers.Dispose();
            m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
      }
      // goes through the party of the character and the enemies conneceted to the triggering enemy and adds battle data to all of them
      // also adds them to the battlesystems player and enemy lists so that other systems know who is currently in a battle
      // triggers an event to let other systems know that the battle is starting
      public void StartBattle(Entity enemy){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            
            // start
            InputGatheringSystem.currentInput = CurrentInput.ui;
            World.GetOrCreateSystem<BattleMenuSystem>().Enabled = true;
            // adding the nessesary components for battle
            if(!isInBattle){
                  // making sure no one is regestered to fight already
                  playerEntities.Clear();
                  enemyEntities.Clear();
                  isInBattle = true;
                  // 
                  EntityQuery characterQuery = GetEntityQuery(typeof(CharacterStats));
                  NativeArray<CharacterStats> characters = characterQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
                  NativeArray<Entity> characterEntities = characterQuery.ToEntityArray(Allocator.TempJob);
                  // adding the battle data to the players
                  Entities
                  .WithAll<PlayerPartyTag>()
                  .ForEach((Entity entity, ref CharacterStats characterStats, in Translation translation) => {
                        // so that it can be moved back after battle
                        ecb.AddComponent(entity , new BeforeBattleData{previousLocation = translation.Value});
                  }).Schedule();
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
                  //always adds the enemies by how they are list in the enemybattledata
                  for(int i = 0; i < enemies.Length; i++){
                        int j = 0;
                        foreach(CharacterStats characterStats in characters){
                              if(enemies[i].id == characterStats.id){
                                    enemyEntities.Add(characterEntities[j]);
                                    Translation translation = GetComponent<Translation>(characterEntities[j]);
                                    // so that it can be moved back after battle
                                    ecb.AddComponent(characterEntities[j], new BeforeBattleData{previousLocation = translation.Value});
                                    if(HasComponent<BattleTriggerData>(characterEntities[j])){
                                          ecb.RemoveComponent<BattleTriggerData>(characterEntities[j]);
                                    }
                              }
                              j++;
                        }
                  }
                  isInBattle = true;
                  // trigger event, should be connected to battle menu and movement 
                  OnBattleStart?.Invoke(this, System.EventArgs.Empty);
                  // wait until the trasition ends to add battle data to everyone in the battle
                  transitionSystem.OnTransitionEnd += AddBattleData_OnTransitionEnd;
                  characters.Dispose();
                  characterEntities.Dispose();
            }
      }
      public void EndBattle(bool isPlayerVictor){
            isInBattle = false;
            Entities.ForEach((ref CharacterStats characterstats) =>{
                  if(characterstats.health <= 0){
                        characterstats.health = isPlayerVictor ? 1 : characterstats.maxHealth;
                  }
            }).ScheduleParallel();
            OnBattleEnd?.Invoke(this, new OnBattleEndEventArgs{isPlayerVictor = isPlayerVictor});
      }
      public void AddBattleData_OnTransitionEnd(System.Object sender, System.EventArgs e){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            foreach(Entity entity in playerEntities){
                  ecb.AddComponent<BattleData>(entity);
            }
            foreach(Entity entity in enemyEntities){
                  ecb.AddComponent<BattleData>(entity);
            }
            transitionSystem.OnTransitionEnd -= AddBattleData_OnTransitionEnd;
      }
}

public class OnBattleEndEventArgs : EventArgs{
      public bool isPlayerVictor {get; set;}
}
public delegate void BattleEndEventHandler(object sender, OnBattleEndEventArgs e);
