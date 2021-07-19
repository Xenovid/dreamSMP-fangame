using Unity.Entities;
using Unity.Collections;
using UnityEngine;
public class CutsceneBattleDataTagAuthoring : MonoBehaviour{
    public string battleName;
}
public struct CutsceneBattleDataTag : IComponentData
{
    public FixedString32 name;
}

public class CutsceneBattleDataConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((CutsceneBattleDataTagAuthoring cutsceneData) =>{
            Entity entity = GetPrimaryEntity(cutsceneData);
            DstEntityManager.AddComponentData(entity, new CutsceneBattleDataTag{name = cutsceneData.battleName});
        });
    }
}
