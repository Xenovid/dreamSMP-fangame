using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class BattleSystem : SystemBase
{
      protected override void OnUpdate()
      {
            EntityQuery battlerInfoGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<BattleData> battleDatas = battlerInfoGroup.ToComponentDataArray<BattleData>(Allocator.Temp);
            NativeArray<CharacterStats> characterStats = battlerInfoGroup.ToComponentDataArray<CharacterStats>(Allocator.Temp); 
            Entities.ForEach((BattleInfo battleInfo) => {
                int i = 0;
                while(i < battleDatas.Length){
                      switch(battleDatas[i].selected){
                        case selectables.attack:
                              foreach(CharacterStats character in characterStats){
                                    if(character.id == battleDatas[i].targetingId){
                                          Debug.Log("charaterid found");
                                    }
                              }
                              break;
                        case selectables.items:

                              break;
                        case selectables.run:

                              break;
                        case selectables.none:

                              break;
                }
                }
            }).Schedule();
      }
}
