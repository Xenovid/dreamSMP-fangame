using Unity.Entities;
using Unity.Transforms;
using UnityEngine.UIElements;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class DialogueSystem : SystemBase
{ 
    UIDocument UIDoc;


    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate(){
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));  
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDoc = UIDocs[0];
    }
  
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        CutsceneManagerData cutsceneManager = new CutsceneManagerData();
        var DeltaTime = Time.DeltaTime;
        EntityQuery CutsceneManagerGroup = GetEntityQuery(typeof(CutsceneManagerData));
        NativeArray<CutsceneManagerData> cutsceneManagers = CutsceneManagerGroup.ToComponentDataArray<CutsceneManagerData>(Allocator.Temp);



        VisualElement root = UIDoc.rootVisualElement;

        bool isThereACutscene = false;
        foreach(CutsceneManagerData cutsceneManager1 in cutsceneManagers){
            cutsceneManager = cutsceneManagers[0];
            isThereACutscene = true;
        }

        
        Entities
        .WithoutBurst()
        .ForEach((Entity entity,DialogueBoxData dialogueBoxData, ref CutsceneData cutsceneData,in Translation translation, in DynamicBuffer<DialogueData> dialogues) => {
            //remove anything related to a cutscene if there is a cutscene
            if(!isThereACutscene){
                ecb.RemoveComponent<CutsceneData>(entity);
                //dialogues.Clear();
                root.Remove(dialogueBoxData.dialogueBox);
                ecb.RemoveComponent<DialogueBoxData>(entity);
            }
            //do anything you need to do for a cutscene if there is a cutscene
            else{
                DialogueData currentDialogue;
                int dialogueNumber = -1;
                for(int i = 0; i < dialogues.Length; i++){
                    if((dialogues[i].keepDialogueUpTime <= cutsceneManager.totalTime)){
                        dialogueNumber++;
                    }
                }
                Debug.Log(dialogueNumber);
                Debug.Log(cutsceneManager.totalTime);
                currentDialogue = dialogues[dialogueNumber];
                if(currentDialogue.dialogueStartTime > cutsceneManager.totalTime){
                    dialogueBoxData.dialogueBox.visible = false;
                }
                else if(currentDialogue.dialogueEndTime > cutsceneManager.totalTime){
                    dialogueBoxData.dialogueBox.visible = true;
                    //find out which letter it is at
                    float timePerCharacter = (currentDialogue.dialogueEndTime/ currentDialogue.dialogueStartTime)/ currentDialogue.dialogue.Length;
                    float timePassFromStart = cutsceneManager.totalTime - currentDialogue.dialogueStartTime;
                    int numberOfCharacters = Mathf.FloorToInt(timePassFromStart / timePerCharacter);
                    string textToDisplay = currentDialogue.dialogue.ToString().Substring(0, numberOfCharacters);
                    Debug.Log("hello");

                    Label bubbleText = dialogueBoxData.dialogueBox.Q<Label>("bubbleText");
                    if(bubbleText != null){
                        bubbleText.text = textToDisplay;
                    }
                    else{
                        Debug.Log("didn't find bubbleText");
                    }
                    
                    //display that number of characters
                }
                else if(currentDialogue.keepDialogueUpTime > cutsceneManager.totalTime){
                    dialogueBoxData.dialogueBox.visible = true;
                    Label bubbleText = dialogueBoxData.dialogueBox.Q<Label>("bubbleText");
                    if(bubbleText != null){
                        bubbleText.text = currentDialogue.dialogue.ToString();
                    }
                    else{
                        Debug.Log("didn't find bubbleText");
                    }
                    //do extra stuff when the bubble is up along with displaying all the dialgue
                }
                else{
                    dialogueBoxData.dialogueBox.visible = false;
                }
            }
            

        }).Run();
        cutsceneManagers.Dispose();
    }
}
