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
            //EntityQuery PlayersGroup = GetEntityQuery(ComponentType.ReadOnly<PlayerPartyData>());
            EntityQuery characterStatsGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<Entity> battleManagers = BattleManagerGroup.ToEntityArray(Allocator.Temp);
            //BattleManagerTag battleManager;

            
            
            NativeArray<Entity> BattleManagers = BattleManagerGroup.ToEntityArray(Allocator.Temp);
            NativeArray<CharacterStats> characterStatsList = characterStatsGroup.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
            NativeArray<Entity> characterEntities = characterStatsGroup.ToEntityArray(Allocator.TempJob);
            foreach(Entity ent in battleManagers) {
                BattleManagerData battleManager = 
            }
            Entities
            .WithStructuralChanges()
            .WithoutBurst()
            .ForEach((ref BattleData battleData ,ref CharacterStats characterStats) => {
                  switch(battleData.selected){
                        case selectables.attack:
                              Debug.Log("attacking with character");
                              int i = 0;                
                              foreach(CharacterStats character in characterStatsList){
                                    if( character.id.Equals(battleData.targetingId)){
                                    ecb.AddComponent(characterEntities[i], new DamageData { damage = 1 });
                                          if(character.health <= 0){
                                                foreach(Entity ent in battleManagers){
                                                      Debug.Log("attempting to remove tag");
                                                      EntityManager.RemoveComponent<BattleManagerTag>(ent);
                                                      Debug.Log("tag removed");
                                                }
                                          }
                                    }
                                    i++;
                              }
                              break;
                        case selectables.items:

                              break;
                        case selectables.run:

                              break;
                        case selectables.none:
                              Debug.Log("doing nothing");
                              break;
                }
            }).Run();
            EntityManager.CompleteAllJobs();
            characterEntities.Dispose();
            characterStatsList.Dispose();
            battleManagers.Dispose();
            //BattleManagerGroup.Dispose();
      }
}
