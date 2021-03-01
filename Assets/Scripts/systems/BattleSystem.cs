using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class BattleSystem : SystemBase
{
      protected override void OnUpdate()
      {

            EntityManager.CompleteAllJobs();
            EntityQuery BattleManagerGroup = GetEntityQuery(ComponentType.ReadWrite<BattleManagerTag>());
            //EntityQuery PlayersGroup = GetEntityQuery(ComponentType.ReadOnly<PlayerPartyData>());
            EntityQuery characterStatsGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<Entity> battleManagers = BattleManagerGroup.ToEntityArray(Allocator.Temp);
            BattleManagerTag battleManager;

            
            
            NativeArray<Entity> BattleManagers = BattleManagerGroup.ToEntityArray(Allocator.Temp);
            NativeArray<CharacterStats> characterStatsList = characterStatsGroup.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
            
            Entities
            .WithStructuralChanges()
            .WithoutBurst()
            .ForEach((ref BattleData battleData ,ref CharacterStats characterStats) => {
                  switch(battleData.selected){
                        case selectables.attack:
                              Debug.Log("attacking with character");
                              foreach(CharacterStats character in characterStatsList){
                                    if( character.id.Equals(battleData.targetingId)){
                                          Debug.Log("found character");
                                          if(character.health <= 0){
                                                foreach(Entity ent in battleManagers){
                                                      Debug.Log("attempting to remove tag");
                                                      EntityManager.RemoveComponent<BattleManagerTag>(ent);
                                                      Debug.Log("tag removed");
                                                }
                                          }
                                    }
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
            characterStatsList.Dispose();
            battleManagers.Dispose();
            //BattleManagerGroup.Dispose();
      }
}
