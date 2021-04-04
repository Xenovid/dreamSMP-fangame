using Unity.Entities;
using Unity.Transforms;
using UnityEngine.UIElements;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class DialogueSystem : SystemBase
{ 
    UIDocument UIDoc;
    Camera camera = new Camera();


    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate(){
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        Entities
        .WithoutBurst()
        .WithAll<CutSceneUITag>()
        .ForEach((UIDocument uiDocument) => {
            UIDoc = uiDocument;
        }).Run();

        Entities
        .WithoutBurst()
        .WithAll<PlayerCamerTag>()
        .ForEach((Camera cam) => {
            camera = cam;
        }).Run();
    }
  
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        
        EntityQuery CutsceneManagerGroup = GetEntityQuery(typeof(CutsceneManagerData));
        NativeArray<CutsceneManagerData> cutsceneManagers = CutsceneManagerGroup.ToComponentDataArray<CutsceneManagerData>(Allocator.Temp);
        CutsceneManagerData cutsceneManager = new CutsceneManagerData();


        VisualElement root = UIDoc.rootVisualElement;

        bool isThereACutscene = false;
        foreach(CutsceneManagerData cutsceneManager1 in cutsceneManagers){
            cutsceneManager = cutsceneManagers[0];
            isThereACutscene = true;
        }

        
        Entities
        .WithoutBurst()
        .ForEach((Entity entity,DialogueBoxData dialogueBoxData, ref CutsceneData cutsceneData,in Translation translation, in DynamicBuffer<DialogueData> dialogues, in CharacterName character) => {
            //remove anything related to a cutscene if there is a cutscene
            Vector3 characterPositonOnScreen = camera.WorldToScreenPoint(new Vector3(translation.Value.x, translation.Value.y, translation.Value.z));
            Vector2 newPosition = new Vector2(characterPositonOnScreen.x, characterPositonOnScreen.y);
            Debug.Log(camera.WorldToScreenPoint(new Vector3(translation.Value.x, translation.Value.y, translation.Value.z)));
            if(!isThereACutscene){
                Debug.Log("deleting cutsceneData");
                ecb.RemoveComponent<CutsceneData>(entity);
                //dialogues.Clear();
                root.Remove(dialogueBoxData.dialogueBox);
                ecb.RemoveComponent<DialogueBoxData>(entity);
            }
            //do anything you need to do for a cutscene if there is a cutscene
            else{
                DialogueData currentDialogue = new DialogueData();
                for(int i = 0; i < dialogues.Length; i++){
                    //Debug.Log("dialogue keep time:" +dialogues[i].keepDialogueUpTime.ToString() + "cutscene total time:" + cutsceneManager.totalTime.ToString());
                    //Debug.Log(dialogues[i].keepDialogueUpTime <= cutsceneManager.totalTime);
                    if(dialogues[i].dialogueStartTime <= cutsceneManager.totalTime){
                        //Debug.Log("current dialogue is:" + dialogues[i].dialogue.ToString());
                        currentDialogue = dialogues[i];
                    }
                }
                //Debug.Log(cutsceneManager.totalTime);
                if(currentDialogue.dialogueStartTime > cutsceneManager.totalTime){
                    dialogueBoxData.dialogueBox.visible = false;
                }
                else if(currentDialogue.dialogueEndTime > cutsceneManager.totalTime){
                    dialogueBoxData.dialogueBox.visible = true;
                    //find out which letter it is at
                    float timePerCharacter = (currentDialogue.dialogueEndTime- currentDialogue.dialogueStartTime)/ currentDialogue.dialogue.Length;
                    float timePassFromStart = cutsceneManager.totalTime - currentDialogue.dialogueStartTime;
                    int numberOfCharacters = Mathf.FloorToInt(timePassFromStart / timePerCharacter);
                    string textToDisplay = currentDialogue.dialogue.ToString().Substring(0, numberOfCharacters);

                    Label bubbleText = dialogueBoxData.dialogueBox.Q<Label>("bubbleText");
                    //bubbleText.layout.Set(newPosition.x, newPosition.y, bubbleText.layout.width, bubbleText.layout.height);
                    bubbleText.style.left = newPosition.x;//newPosition.x;
                    bubbleText.style.top = -newPosition.y + camera.pixelHeight; //newPosition.y;
                    Debug.Log(bubbleText.style.top);
                    if(bubbleText != null){
                        bubbleText.text = character.name +  ":" + textToDisplay;
                    }
                    else{
                        Debug.Log("didn't find bubbleText");
                    }
                    
                    //display that number of characters
                }
                else if(currentDialogue.keepDialogueUpTime > cutsceneManager.totalTime){
                    dialogueBoxData.dialogueBox.visible = true;
                    Label bubbleText = dialogueBoxData.dialogueBox.Q<Label>("bubbleText");
                    bubbleText.style.left = newPosition.x;//newPosition.x;
                    bubbleText.style.top = -newPosition.y + camera.pixelHeight;//newPosition.y;
                    if(bubbleText != null){
                        bubbleText.text = character.name + ":" + currentDialogue.dialogue.ToString();
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
