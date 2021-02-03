using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Physics;
using UnityEngine.UIElements;

//Is used to check if the player hit a triggerzone to begin a cutscene
public class DisplayTextSystem : SystemBase
{   
    public StepPhysicsWorld physicsWorld;EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    UIDocument UIDoc;

    protected override void OnCreate(){
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();

        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();

        foreach(UIDocument UIdoc in UIDocs){
            UIDoc = UIdoc;
        }
    }
    protected override void OnUpdate()
    {   
        var triggerEvents =  ((Simulation)physicsWorld.Simulation).TriggerEvents;
        foreach(TriggerEvent triggerEvent in triggerEvents){
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
           
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            var rootVisualElement = UIDoc.rootVisualElement;
            VisualElement charaterText = rootVisualElement.Q<VisualElement>("characterText");
            Label textBoxText = rootVisualElement.Q<Label>("text");

            Entities
            .WithNone<TextBoxData>()
            .WithoutBurst()
            .ForEach((Entity entity, Text text) => {
                if(entity.Equals(entityA)){
                    ecb.AddComponent(entityA, new TextBoxData{
                        textBoxText = textBoxText
                    });
                    ecb.AddComponent(entityB, new CutsceneData{
                        isReadingDialogue = true
                    });
                    textBoxText.text = "";
                    charaterText.visible = true;
                }
                if(entity.Equals(entityB)){
                    ecb.AddComponent(entityB, new TextBoxData{
                        textBoxText = textBoxText
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


