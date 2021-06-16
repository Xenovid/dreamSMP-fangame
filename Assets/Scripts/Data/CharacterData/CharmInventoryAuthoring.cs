using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CharmInventoryAuthoring : MonoBehaviour
{
    public CharmInfo[] charmInfos;
}
[System.Serializable]
public struct CharmData : IBufferElementData{
    public Charm charm;
}
public class CharmConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((CharmInventoryAuthoring charmInventory) =>{
            Entity entity = GetPrimaryEntity(charmInventory);
            DstEntityManager.AddBuffer<CharmData>(entity);
            DynamicBuffer<CharmData> CharmInventory = DstEntityManager.GetBuffer<CharmData>(entity);
            foreach(CharmInfo charmInfo in charmInventory.charmInfos){
                Charm charm = new Charm{
                    name = charmInfo.name,
                    description = charmInfo.description
                };
                CharmInventory.Add(new CharmData{charm = charm});
            }
        });
    }

    public static Charm CharmInfoToCharm(CharmInfo charmInfo){
        return new Charm{
            name = charmInfo.name,
            description = charmInfo.description
        };
    }
}

[System.Serializable]
public struct CharmInfo{
    public string name;
    public string description;
}
[System.Serializable]
public struct Charm{
    //add features
    public FixedString32 name;
    public FixedString128 description;
}