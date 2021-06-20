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
        foreach(TriggerEvent triggerEvent in triggerEvents)
        {
                
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (HasComponent<SavePointTag>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
            {
                if(input.select){
                    string savePointName = GetComponent<SavePointTag>(entityA).saveName.ToString();
                    SavePointAlert?.Invoke(this, new SavePointEventArg{savePointName = savePointName});
                    Entities
                    .WithStructuralChanges()
                    .ForEach((ref CharacterStats character) => {
                        character.health = character.maxHealth;
                    }).Run();
                    OverworldUITag overworld = GetSingleton<OverworldUITag>();
                    overworld.isVisable = false;
                    SetSingleton<OverworldUITag>(overworld);
                }
                else{
                    OverworldUITag overworld = GetSingleton<OverworldUITag>();
                    overworld.isNextToInteractive = true;
                    SetSingleton<OverworldUITag>(overworld);
                }
            }
            else if (HasComponent<SavePointTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
            {
                if(input.select){
                    Entities
                    .WithStructuralChanges()
                    .ForEach((ref CharacterStats character) => {
                        character.health = character.maxHealth;
                    }).Run();
                    string savePointName = GetComponent<SavePointTag>(entityB).saveName.ToString();
                    SavePointAlert?.Invoke(this, new SavePointEventArg{savePointName = savePointName});
                    OverworldUITag overworld = GetSingleton<OverworldUITag>();
                    overworld.isVisable = false;
                    SetSingleton<OverworldUITag>(overworld);
                }
                else{
                    OverworldUITag overworld = GetSingleton<OverworldUITag>();
                    overworld.isNextToInteractive = true;
                    SetSingleton<OverworldUITag>(overworld);
                }
            }
        }
    }
}

public delegate void SavePointEventHandler(Object sender, SavePointEventArg e);
public class SavePointEventArg : EventArgs{
    public string savePointName;
}
