using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BattleTriggerSystem : SystemBase
{
    public StepPhysicsWorld physicsWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    public VisualElement battleUI;
    public UIDocument UIDoc;

    //private Entity lastAddedManager;
    

    VisualTreeAsset enemySelectionUITemplate;


    protected override void OnCreate(){
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
    }

      protected override void OnStartRunning()
      {
            // getting all the ui infomation to be used in the loop
            base.OnStartRunning();
            EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));  
            UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
            UIDoc = UIDocs[0];

            enemySelectionUITemplate = Resources.Load<VisualTreeAsset>("EnemyDetails");
      }



    protected override void OnUpdate()
    {
        
        EntityManager.CompleteAllJobs();
        int loopNumber = 0;
        

        // finds all the items that caused trigger events
        var triggerEvents =  ((Simulation)physicsWorld.Simulation).TriggerEvents;

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        foreach(TriggerEvent triggerEvent in triggerEvents){
            var rootVisualElement = UIDoc.rootVisualElement;
            battleUI = rootVisualElement.Q<VisualElement>("BattleUI");
            VisualElement enemySelector = rootVisualElement.Q<VisualElement>("EnemySelector");


            int enemyloopNumber = 0;

            // the entities from the trigger event
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            /*
            if(entityA == lastAddedManager || entityB == lastAddedManager){
                Debug.Log("its a me mario");
                //continue;
            }
            */

            //checks if the player hit an entity with battle data on it, and if so triggers a battle
            if(GetComponentDataFromEntity<PlayerTag>().HasComponent(entityA) && GetComponentDataFromEntity<BattleTriggerData>().HasComponent(entityB) && loopNumber < 1){
                loopNumber++;
                AudioManager.playSong("tempBattleMusic");

                //adds the nessesary data for the player to be considered in the battle
                EntityManager.AddComponent<UIInputData>(entityA);
                EntityManager.AddComponent<BattleManagerTag>(entityB);
                //lastAddedManager = entityB;

                EntityManager.AddComponentObject(entityB, new BattleManagerData{hasPlayerWon = false});
                EntityManager.CompleteAllJobs();

                List<int> addedIds = new List<int>();
                DynamicBuffer<EnemyBattleData> tempEnemy = GetBuffer<EnemyBattleData>(entityB);
                DynamicBuffer<PlayerPartyData> tempPlayers = GetBuffer<PlayerPartyData>(entityA);

                List<PlayerPartyData> players = new List<PlayerPartyData>();
                foreach(PlayerPartyData temp in tempPlayers){
                    players.Add(temp);
                }
                int playerLength = tempPlayers.Length;

                List<EnemyBattleData> enemyBattleDatas = new List<EnemyBattleData>();
                foreach(EnemyBattleData temp in tempEnemy){
                    enemyBattleDatas.Add(temp);
                }
                int enemyLength = tempEnemy.Length;


                //NativeArray<PlayerPartyData> players = tempPlayers.AsNativeArray();
                //NativeArray<EnemyBattleData> enemyBattleDatas = tempEnemy.AsNativeArray();

                //gets the lists for who is fighting in the battle and gives them the data that will allow them to be detected in the battlesystem
                Entities
                .WithStructuralChanges()
                .WithNone<EnemySelectorData, BattleData>()
                .ForEach((int entityInQueryIndex,ref Entity entity, in CharacterStats characterStats) => {

                        // the lists of players and the enemyies that will be fought
                        for(int i = 0; i < playerLength; i++){
                            if(!addedIds.Contains(characterStats.id) && characterStats.id == players[i].playerId){
                                ecb.AddComponent<BattleData>(entity);
                                addedIds.Add(characterStats.id);
                            }
                        }
                        for(int i = 0; i < enemyLength; i++){
                            if(!addedIds.Contains(characterStats.id) && characterStats.id == enemyBattleDatas[i].id){
                                
                                VisualElement enemyDetails = enemySelectionUITemplate.CloneTree();
                                enemySelector.Add(enemyDetails);
                                ecb.AddComponent<BattleData>(entity);
                                EntityManager.AddComponentObject(entity, new EnemySelectorUI{enemySelectorUI = enemyDetails});
                                if(enemyloopNumber == 0 && !addedIds.Contains(characterStats.id)){
                                    EntityManager.AddComponentData(entity, new EnemySelectorData{enemyId = characterStats.id, isSelected = true});
                                }
                                else if(!addedIds.Contains(characterStats.id)){
                                    EntityManager.AddComponentData(entity, new EnemySelectorData{enemyId = characterStats.id, isSelected = false});
                                }
                                enemyloopNumber ++;
                                addedIds.Add(characterStats.id);
                                
                            }

                        }
                        /*
                        foreach(EnemyBattleData enemyBattleData in enemyBattleDatas){
                            if(characterStats.id == enemyBattleData.id){
                                VisualElement enemyDetails = enemySelectionUITemplate.CloneTree();
                                enemySelector.Add(enemyDetails);

                                ecb.AddComponent<BattleData>(entity);
                                EntityManager.AddComponentObject(entity, new EnemySelectorUI{enemySelectorUI = enemyDetails});

                                EntityManager.AddComponentObject(entity, new EnemySelectorUI{enemySelectorUI = enemyDetails});
                                if(enemyloopNumber == 0){
                                    EntityManager.AddComponentData(entity, new EnemySelectorData{enemyId = enemyBattleData.id, isSelected = true});
                                    Debug.Log("added selction data");
                                }
                                else{
                                    EntityManager.AddComponentData(entity, new EnemySelectorData{enemyId = enemyBattleData.id, isSelected = false});
                                }
                                enemyloopNumber++;
                            }
                        }
                        */
                    }).Run();
                    //enemyBattleDatas.Dispose();
                    //players.Dispose();

                    battleUI.visible =true;
                    EntityManager.RemoveComponent<BattleTriggerData>(entityB);
                    EntityManager.RemoveComponent<PhysicsCollider>(entityB);
            }
            else if(GetComponentDataFromEntity<PlayerTag>().HasComponent(entityB) && GetComponentDataFromEntity<BattleTriggerData>().HasComponent(entityA) && loopNumber < 0){
                loopNumber++;
                AudioManager.playSong("tempBattleMusic");
                DynamicBuffer<EnemyBattleData> temp = GetBuffer<EnemyBattleData>(entityA);
                NativeArray<EnemyBattleData> enemyBattleDatas = temp.ToNativeArray(Allocator.Temp);
                EntityManager.CompleteAllJobs();

                //adds the nessesary data for the player to be considered in the battle
                EntityManager.AddComponent<UIInputData>(entityB);
                EntityManager.AddComponent<BattleManagerTag>(entityA);
                EntityManager.AddComponentObject(entityA, new BattleManagerData{hasPlayerWon = false});

                //gets the lists for who is fighting in the battle and gives them the data that will allow them to be detected in the battlesystem
                EntityManager.CompleteAllJobs();
                Entities
                .WithStructuralChanges()
                .ForEach((int entityInQueryIndex,ref Entity entity, in CharacterStats characterStats) => {

                    // the lists of players and the enemyies that will be fought
                    DynamicBuffer<EnemyBattleData> enemyBattleDatas = GetBuffer<EnemyBattleData>(entityB);
                    DynamicBuffer<PlayerPartyData> players = GetBuffer<PlayerPartyData>(entityA);

                    foreach(PlayerPartyData player in players){
                        if(characterStats.id == player.playerId){
                            ecb.AddComponent<BattleData>(entity);
                        }
                    }

                    foreach(EnemyBattleData enemyBattleData in enemyBattleDatas){
                        int i = 0;
                        if(characterStats.id == enemyBattleData.id){
                            VisualElement enemyDetails = enemySelectionUITemplate.CloneTree();
                            enemySelector.Add(enemyDetails);

                            ecb.AddComponent<BattleData>(entity);
                            EntityManager.AddComponentObject(entity, new EnemySelectorUI{enemySelectorUI = enemyDetails});

                            EntityManager.AddComponentObject(entity, new EnemySelectorUI{enemySelectorUI = enemyDetails});
                            if(i == 0){
                                EntityManager.AddComponentData(entity, new EnemySelectorData{enemyId = enemyBattleData.id, isSelected = true});
                                i++;
                            }
                            else{
                                EntityManager.AddComponentData(entity, new EnemySelectorData{enemyId = enemyBattleData.id, isSelected = false});
                            }
                        }
                    }
                    }).Run();
                    battleUI.visible =true;
                    enemyBattleDatas.Dispose();
                    EntityManager.RemoveComponent<BattleTriggerData>(entityA);
                    EntityManager.RemoveComponent<PhysicsCollider>(entityA);
                    EntityManager.CompleteAllJobs();
            }
        }
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
