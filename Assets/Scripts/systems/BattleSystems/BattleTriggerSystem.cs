using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

public class BattleTriggerSystem : SystemBase
{
    public StepPhysicsWorld physicsWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    public VisualElement battleUI;
    public UIDocument UIDoc;

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
      }

  

    protected override void OnUpdate()
    {
        
        EntityManager.CompleteAllJobs();

        // finds all the items that caused trigger events
        var triggerEvents =  ((Simulation)physicsWorld.Simulation).TriggerEvents;

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        EntityQuery characterStatsGroup = GetEntityQuery(ComponentType.ReadOnly<CharacterStats>());
        EntityManager.CompleteAllJobs();

        foreach(TriggerEvent triggerEvent in triggerEvents){
            var rootVisualElement = UIDoc.rootVisualElement;

            battleUI = rootVisualElement.Q<VisualElement>("BattleUI");
            // the entities from the trigger event
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            //checks if the player hit an entity with battle data on it, and if so triggers a battle
            if(GetComponentDataFromEntity<PlayerTag>().HasComponent(entityA) && GetComponentDataFromEntity<BattleTriggerData>().HasComponent(entityB)){
                AudioManager.playSong("tempBattleMusic");
                DynamicBuffer<EnemyBattleData> temp = GetBuffer<EnemyBattleData>(entityB);
                NativeArray<EnemyBattleData> enemyBattleDatas = temp.ToNativeArray(Allocator.Temp);
                EntityManager.CompleteAllJobs();

                //adds the nessesary data for the player to be considered in the battle
                EntityManager.AddComponent<UIInputData>(entityA);
                EntityManager.AddComponent<BattleManagerTag>(entityB);

                //gets the lists for who is fighting in the battle and gives them the data that will allow them to be detected in the battlesystem
                EntityManager.CompleteAllJobs();
                Entities
                .ForEach((int entityInQueryIndex,ref Entity entity, in CharacterStats characterStats) => {

                    // the lists of players and the enemyies that will be fought
                    DynamicBuffer<EnemyBattleData> enemyBattleDatas = GetBuffer<EnemyBattleData>(entityB);
                    DynamicBuffer<PlayerPartyData> players = GetBuffer<PlayerPartyData>(entityA);

                    foreach(PlayerPartyData player in players){
                        if(characterStats.id == player.playerId){
                            ecb.AddComponent<BattleData>(entityInQueryIndex ,entity);
                        }
                    }

                    foreach(EnemyBattleData enemyBattleData in enemyBattleDatas){
                        if(characterStats.id == enemyBattleData.id){
                            ecb.AddComponent<BattleData>(entityInQueryIndex ,entity);
                        }
                    }
                    }).Schedule();
                    battleUI.visible =true;
                    Debug.Log("battleui should be visable");
                    enemyBattleDatas.Dispose();
                    EntityManager.RemoveComponent<BattleTriggerData>(entityB);
            }
            else if(GetComponentDataFromEntity<PlayerTag>().HasComponent(entityB) && GetComponentDataFromEntity<BattleTriggerData>().HasComponent(entityA)){
                AudioManager.playSong("tempBattleMusic");
                DynamicBuffer<EnemyBattleData> temp = GetBuffer<EnemyBattleData>(entityA);
                NativeArray<EnemyBattleData> enemyBattleDatas = temp.ToNativeArray(Allocator.Temp);
                EntityManager.CompleteAllJobs();

                //adds the nessesary data for the player to be considered in the battle
                EntityManager.AddComponent<UIInputData>(entityB);
                EntityManager.AddComponent<BattleManagerTag>(entityA);

                //gets the lists for who is fighting in the battle and gives them the data that will allow them to be detected in the battlesystem
                EntityManager.CompleteAllJobs();
                Entities
                .ForEach((int entityInQueryIndex,ref Entity entity, in CharacterStats characterStats) => {

                    // the lists of players and the enemyies that will be fought
                    DynamicBuffer<EnemyBattleData> enemyBattleDatas = GetBuffer<EnemyBattleData>(entityB);
                    DynamicBuffer<PlayerPartyData> players = GetBuffer<PlayerPartyData>(entityA);

                    foreach(PlayerPartyData player in players){
                        if(characterStats.id == player.playerId){
                            ecb.AddComponent<BattleData>(entityInQueryIndex ,entity);
                        }
                    }

                    foreach(EnemyBattleData enemyBattleData in enemyBattleDatas){
                        if(characterStats.id == enemyBattleData.id){
                            ecb.AddComponent<BattleData>(entityInQueryIndex ,entity);
                        }
                    }
                    }).Schedule();
                    battleUI.visible =true;
                    Debug.Log("battleui should be visable");
                    enemyBattleDatas.Dispose();
                    EntityManager.RemoveComponent<BattleTriggerData>(entityB);
            }
            m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
            
        }
    }
}
