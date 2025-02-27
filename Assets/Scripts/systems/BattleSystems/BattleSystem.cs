using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
using System;

public class BattleSystem : SystemBase
{
      PauseSystem pauseSystem;
      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
      public bool isInBattle;
      public event BattleEndEventHandler OnBattleEnd;
      public event EventHandler OnBattleStart;
      public List<Entity> playerEntities = new List<Entity>();
      public List<Entity> enemyEntities = new List<Entity>();
      TransitionSystem transitionSystem;
      protected override void OnCreate(){
            base.OnCreate();
            pauseSystem = World.GetOrCreateSystem<PauseSystem>();
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            isInBattle = false;
            transitionSystem = World.GetOrCreateSystem<TransitionSystem>();
      }

      protected override void OnUpdate()
      {
            EntityManager.CompleteAllJobs();
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

           
            EntityQuery characterStatsGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            EntityQuery PlayerParty = GetEntityQuery(ComponentType.ReadWrite<PlayerPartyData>());
            
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
                        .ForEach((Animator animator, Entity entity, ref DynamicBuffer<DamageData> damages, ref BattleData battleData,ref CharacterStats characterStats, in AnimationData animationData) => {
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
                                                      if(HasComponent<BasicBattleAudioData>(entity)){
                                                            AudioManager.playSound(GetComponent<BasicBattleAudioData>(entity).hitSoundName.ToString());
                                                      }
                                                      label.AddToClassList("message_white");
                                                break;
                                          }
                                          headsUpUI.UI.Q<VisualElement>("messages").Add(label);
                                          
                                          Message message = new Message{timePassed = 0, label = label, direction = random.Value.NextFloat2Direction()};
                                          headsUpUI.messages.Add(message);
                                    }
                                    switch(damages[i].statusEffect){
                                          case StatusEffect.bleeding:
                                                if(HasComponent<BleedingData>(entity)){
                                                      BleedingData bleedingData = GetComponent<BleedingData>(entity);
                                                      bleedingData.level++;
                                                      SetComponent(entity, bleedingData);
                                                }
                                                else ecb.AddComponent(entity, new BleedingData{level = 1});
                                          break;
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
                                    //animator.Play(animationData.characterDownAnimationName);
                              }
                        }).Run();
                        Entities
                        .WithStructuralChanges()
                        .WithoutBurst()
                        .WithAll<AnimationData>()
                        .WithNone<DownTag>()
                        .ForEach((Animator animator , HeadsUpUIData headsUpUI, Entity entity,ref TechnoData techno, ref RandomData random, ref DynamicBuffer<DamageData> damages, ref BattleData battleData,ref CharacterStats characterStats ) => {
                              AnimationData animationData = EntityManager.GetComponentObject<AnimationData>(entity);
                              DynamicBuffer<HealingData> healings = GetBuffer<HealingData>(entity);
                              for(int i = 0; i < damages.Length; i++){
                                    
                                    Label label = new Label();
                                    label.text = damages[i].damage.ToString();
                                    switch( damages[i].type){
                                          case damageType.bleeding:
                                                label.AddToClassList("message_red");
                                          break;
                                          case damageType.physical:
                                                if(HasComponent<BasicBattleAudioData>(entity)){
                                                      AudioManager.playSound(GetComponent<BasicBattleAudioData>(entity).hitSoundName.ToString());
                                                }
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
                                    animator.Play(animationData.characterDownAnimationName);
                              }
                        }).Run();
                  }
            }
            characterEntities.Dispose();
            characterStatsList.Dispose();
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
                  .WithAll<PlayerTag>()
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
                              playerEntities.Add(players[i].player);
                        }

                  }).Run();
                  // adding battle data to the enemies
                  DynamicBuffer<EnemyBattleData> enemies = GetBuffer<EnemyBattleData>(enemy);
                  //always adds the enemies by how they are list in the enemybattledata
                  foreach(EnemyBattleData enemyBattleData in enemies){
                        if(HasComponent<CharacterStats>(enemyBattleData.enemyEntity)){
                              enemyEntities.Add(enemyBattleData.enemyEntity);
                              Translation translation = GetComponent<Translation>(enemyBattleData.enemyEntity);
                              // so that it can be moved back after battle
                              ecb.AddComponent(enemyBattleData.enemyEntity, new BeforeBattleData{previousLocation = translation.Value});
                              if(HasComponent<BattleTriggerData>(enemyBattleData.enemyEntity)){
                                    ecb.RemoveComponent<BattleTriggerData>(enemyBattleData.enemyEntity);
                              }
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
            Entities.WithStructuralChanges().WithAll<DownTag>().ForEach((Entity entity) => {EntityManager.RemoveComponent<DownTag>(entity);}).Run();
            OnBattleEnd?.Invoke(this, new OnBattleEndEventArgs{isPlayerVictor = isPlayerVictor});
      }
      public void AddBattleData_OnTransitionEnd(System.Object sender, System.EventArgs e){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            foreach(Entity entity in playerEntities){
                  EntityManager.AddComponent<BattleData>(entity);
                  if(HasComponent<PhysicsCollider>(entity)){
                        PhysicsCollider collider = GetComponent<PhysicsCollider>(entity);
                        collider.Value.Value.Filter = CollisionFilter.Zero;
                        SetComponent<PhysicsCollider>(entity, collider);
                  }
            }
            foreach(Entity entity in enemyEntities){
                  EntityManager.AddComponentData(entity, new BattleData{maxUseTime = 1, isRecharging = true});
                  if(HasComponent<PhysicsCollider>(entity)){
                        PhysicsCollider collider = GetComponent<PhysicsCollider>(entity);
                        collider.Value.Value.Filter = CollisionFilter.Zero;
                        SetComponent<PhysicsCollider>(entity, collider);
                  }
            }
            Entities
            .WithoutBurst()
            .WithNone<BattleData, BattleBackgroundTag>()
            .ForEach((SpriteRenderer sprite) => {
                  Color newColor = sprite.color;
                  newColor.a = 0;
                  sprite.color = newColor;
            }).Run();
             Entities
            .WithoutBurst()
            .ForEach((TilemapRenderer tilemap) => {
                  tilemap.enabled = false;
            }).Run();
            pauseSystem.BattlePause();
            transitionSystem.OnTransitionEnd -= AddBattleData_OnTransitionEnd;
      }
}

public class OnBattleEndEventArgs : EventArgs{
      public bool isPlayerVictor {get; set;}
}
public delegate void BattleEndEventHandler(object sender, OnBattleEndEventArgs e);
