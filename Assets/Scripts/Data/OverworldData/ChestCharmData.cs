using Unity.Entities;
using UnityEngine;
public class ChestCharmAuthouring : MonoBehaviour{
    public CharmInfo charmInfo;
}

public struct ChestCharmData : IComponentData
{
    public Charm charm;
}
public class ChestCharmConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ChestCharmAuthouring chestCharm) =>{
            Charm newCharm = new Charm{
                name = chestCharm.charmInfo.name,
                description = chestCharm.charmInfo.description
            };
        });
    }
}
