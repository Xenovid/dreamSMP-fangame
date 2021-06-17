using Unity.Entities;
using Unity.Physics;
using System;
using Unity.Physics.Systems;
public class SaveTriggerSystem : SystemBase
{
    StepPhysicsWorld physicsWorld;
    public event SavePointEventHandler SavePointAlert;

    protected override void OnStartRunning()
    {
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;

        if(input.select){
            EntityQuery playerQuery = GetEntityQuery(typeof(PlayerTag), typeof(MovementData));
            MovementData playerMovment = playerQuery.GetSingleton<MovementData>();


            foreach(TriggerEvent triggerEvent in triggerEvents)
            {
                
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                if (HasComponent<SavePointTag>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
                {
                    string savePointName = GetComponent<SavePointTag>(entityA).saveName.ToString();
                    SavePointAlert?.Invoke(this, new SavePointEventArg{savePointName = savePointName});
                    
                }
                else if (HasComponent<SavePointTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
                {
                    string savePointName = GetComponent<SavePointTag>(entityB).saveName.ToString();
                    SavePointAlert?.Invoke(this, new SavePointEventArg{savePointName = savePointName});
                }
            }
        }
    }
}

public delegate void SavePointEventHandler(Object sender, SavePointEventArg e);
public class SavePointEventArg : EventArgs{
    public string savePointName;
}
