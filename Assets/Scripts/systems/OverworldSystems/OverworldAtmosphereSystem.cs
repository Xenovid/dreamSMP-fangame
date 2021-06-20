using Unity.Entities;
using UnityEngine;
[AlwaysUpdateSystem]
public class OverworldAtmosphereSystem : SystemBase
{
    EntityQuery atmosphereQuery;
    protected override void OnStartRunning()
    {
        atmosphereQuery = GetEntityQuery(typeof(OverworldAtmosphereData));
    }
    protected override void OnUpdate()
    {
        if(atmosphereQuery.CalculateChunkCount() == 0){
            EntityManager.CreateEntity(typeof(OverworldAtmosphereData));
        }
    }
}
