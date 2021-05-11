using Unity.Entities;
using Unity.Physics;
using UnityEngine.UIElements;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

public class VictoryScreenSystem : SystemBase
{
    UIDocument UIDoc;
    bool isPrintingVictoryData;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    InkDisplaySystem inkDisplaySystem;
    
    protected override void OnStartRunning(){
        // getting the endsinclation system for a entity component buffer later on
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        // getting the inkDisplaySystem to watch for when its done writing
        inkDisplaySystem = World.GetExistingSystem<InkDisplaySystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
    }
    
    

}
