using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Physics;
using UnityEngine.UIElements;
using UnityEngine;

//Is used to check if the player hit a triggerzone to begin a cutscene
public class DisplayTextSystem : SystemBase
{   
    public StepPhysicsWorld physicsWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    UIDocument UIDoc;

    protected override void OnCreate(){
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();

    }

    protected override void OnStartRunning(){
        base.OnStartRunning();

        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDoc = UIDocs[0];
    }
    protected override void OnUpdate()
    {   
        EntityManager.CompleteAllJobs();
        var triggerEvents =  ((Simulation)physicsWorld.Simulation).TriggerEvents;
        foreach(TriggerEvent triggerEvent in triggerEvents){
            if(UIDoc == null){
                Debug.Log("UIDocument not found");
            }
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            var rootVisualElement = UIDoc.rootVisualElement;
            VisualElement charaterText = rootVisualElement.Q<VisualElement>("characterText");
            Label textBoxText = rootVisualElement.Q<Label>("text");

            Entities
            .WithNone<TextBoxData>()
            .WithoutBurst()
            .ForEach((ref Entity entity, ref DynamicBuffer<Text> text) => {
                if(entity.Equals(entityA)){
                    ecb.AddComponent(entityA, new TextBoxData{
                    });
                    ecb.AddComponent(entityB, new CutsceneData{
                        isReadingDialogue = true
                    });
                    textBoxText.text = "";
                    charaterText.visible = true;
                }
                if(entity.Equals(entityB)){
                    ecb.AddComponent(entityB, new TextBoxData{
                    });
                    ecb.AddComponent(entityA, new CutsceneData{
                        isReadingDialogue = true
                    });
                    textBoxText.text = "";
                    charaterText.visible = true;
                }
            }
            ).Run();
        }
    }   
}


