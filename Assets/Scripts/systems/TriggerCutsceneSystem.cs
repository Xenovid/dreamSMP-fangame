using Unity.Entities;
using Unity.Collections;
using UnityEngine.UIElements;
using Unity.Physics.Systems;
using Unity.Physics;
using UnityEngine;
using UnityEditor;
public class TriggerCutsceneSystem : SystemBase
{
    public StepPhysicsWorld physicsWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    UIDocument UIDoc;
    VisualTreeAsset dialogueBoxTree;

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
        dialogueBoxTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ui/TextBubble.uxml");
    }
    protected override void OnUpdate()
    {   
        var triggerEvents =  ((Simulation)physicsWorld.Simulation).TriggerEvents;
        foreach(TriggerEvent triggerEvent in triggerEvents){
            Debug.Log("there is a trigger event");
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            var rootVisualElement = UIDoc.rootVisualElement;

            EntityQuery characterStatsGroup = GetEntityQuery(typeof(CharacterStats));
            NativeArray<CharacterStats> characters = characterStatsGroup.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
            NativeArray<Entity> characterEntities = characterStatsGroup.ToEntityArray(Allocator.TempJob);

            Debug.Log("triggerevents");
            //checks if the trigger event has a player and cutscenetriggerdata
            if((GetComponentDataFromEntity<PlayerTag>().HasComponent(entityA) || GetComponentDataFromEntity<PlayerTag>().HasComponent(entityB)) && (GetComponentDataFromEntity<CutsceneTriggerTag>().HasComponent(entityB) || GetComponentDataFromEntity<CutsceneTriggerTag>().HasComponent(entityB))){

                Entities
                .WithNone<CutsceneData>()
                .WithStructuralChanges()
                .WithoutBurst()
                .ForEach((Entity entity, in CutsceneTriggerData cutsceneTriggerData) => {
                    if(entityA == entity || entityB == entity){
                        foreach(tempCharacterDialogue characterDialogue in cutsceneTriggerData.characterDialogues){
                        int i = 0;
                        foreach(CharacterStats character in characters){
                            if(character.id == characterDialogue.id){
                                DynamicBuffer<DialogueData> dialogues =  EntityManager.AddBuffer<DialogueData>(characterEntities[i]);
                                foreach(tempDialogueData tempDialogue in characterDialogue.dialogues){
                                    dialogues.Add(new DialogueData{
                                        dialogue = tempDialogue.dialogue,
                                        dialogueStartTime = tempDialogue.dialogueStartTime,
                                        dialogueEndTime = tempDialogue.dialogueChangeTime,
                                        keepDialogueUpTime = tempDialogue.keepDialogueUpTime
                                    });
                                }
                                TemplateContainer dialogueBox = dialogueBoxTree.CloneTree();
                                rootVisualElement.Add(dialogueBox);
                                ecb.AddComponent(characterEntities[i], new DialogueBoxData{dialogueBox = dialogueBox});
                                ecb.AddComponent(characterEntities[i],new CutsceneData{isReadingDialogue = true});
                            }
                            i++;
                        }
                    }
                        //remove the trigger so it doesn't repeat
                        ecb.RemoveComponent<CutsceneTriggerData>(entity);
                        ecb.AddComponent<CutsceneManagerData>(entity);
                        ecb.RemoveComponent<CutsceneTriggerTag>(entity);
                    }
                    // goes throw all of the characters that are going to have dialogue finds there equivalent charactersheets and adds them to the 
                }
                ).Run();
            }
            characters.Dispose();
            characterEntities.Dispose();
        }   
}
}
