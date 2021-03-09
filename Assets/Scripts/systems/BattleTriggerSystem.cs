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
            base.OnStartRunning();
            EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));  
            UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
            UIDoc = UIDocs[0];
      }

  

    protected override void OnUpdate()
    {
        
        EntityManager.CompleteAllJobs();

        var triggerEvents =  ((Simulation)physicsWorld.Simulation).TriggerEvents;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        EntityQuery characterStatsGroup = GetEntityQuery(ComponentType.ReadOnly<CharacterStats>());
        EntityManager.CompleteAllJobs();

        foreach(TriggerEvent triggerEvent in triggerEvents){
            var rootVisualElement = UIDoc.rootVisualElement;
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            if(GetComponentDataFromEntity<PlayerTag>().HasComponent(entityA) && GetComponentDataFromEntity<BattleTriggerData>().HasComponent(entityB)){
                DynamicBuffer<EnemyBattleData> temp = GetBuffer<EnemyBattleData>(entityB);
                NativeArray<EnemyBattleData> enemyBattleDatas = temp.ToNativeArray(Allocator.Temp);
                EntityManager.CompleteAllJobs();
                EntityManager.AddComponent<BattleData>(entityA);
                EntityManager.AddComponent<UIInputData>(entityA);
                EntityManager.AddComponent<BattleManagerTag>(entityB);

                EntityManager.CompleteAllJobs();
                Entities
                .ForEach((int entityInQueryIndex,ref Entity entity, in CharacterStats characterStats) => {
                    DynamicBuffer<EnemyBattleData> enemyBattleDatas = GetBuffer<EnemyBattleData>(entityB);
                    foreach(EnemyBattleData enemyBattleData in enemyBattleDatas){
                        if(characterStats.id == enemyBattleData.enemyid){
                            ecb.AddComponent<BattleData>(entityInQueryIndex ,entity, new BattleData{selected = selectables.none});
                        }
                    }
                    }).Schedule();
                    battleUI.visible =true;
                    enemyBattleDatas.Dispose();
                    EntityManager.RemoveComponent<BattleTriggerData>(entityB);
            }
            else if(GetComponentDataFromEntity<PlayerTag>().HasComponent(entityB) && GetComponentDataFromEntity<BattleTriggerData>().HasComponent(entityB)){
                DynamicBuffer<EnemyBattleData> temp = GetBuffer<EnemyBattleData>(entityA);
                NativeArray<EnemyBattleData> enemyBattleDatas = temp.ToNativeArray(Allocator.Temp);

                EntityManager.CompleteAllJobs();
                EntityManager.AddComponent<BattleData>(entityB);
                EntityManager.AddComponent<UIInputData>(entityB);
                EntityManager.AddComponent<BattleManagerTag>(entityA);

                EntityManager.CompleteAllJobs();
                Entities
                .ForEach((int entityInQueryIndex, ref Entity entity,in CharacterStats characterStats) => {
                    foreach(EnemyBattleData enemyBattleData in enemyBattleDatas){
                        if(characterStats.id == enemyBattleData.enemyid){
                            ecb.AddComponent<BattleData>(entityInQueryIndex ,entity, new BattleData{selected = selectables.none});
                        }
                    }
                    }).Schedule();
                    enemyBattleDatas.Dispose();
                    battleUI.visible =true;
                    EntityManager.RemoveComponent<BattleTriggerData>(entityA);
                }
                
            }
            m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
            
        }
}
