using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;
using UnityEngine.UIElements;

public class TextBoxWithAudioSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private float charTime = 1.0f;
    Label textBoxText;
    VisualElement charaterText;
    VisualElement charaterImage;

    protected override void OnCreate(){
        base.OnCreate();
        
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnStartRunning(){
        base.OnStartRunning();

        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDocument UIDoc = UIDocs[0];
        var rootVisualElement = UIDoc.rootVisualElement;
        charaterText = rootVisualElement.Q<VisualElement>("TextBoxUI");
        textBoxText = rootVisualElement.Q<Label>("TextBoxText");
        charaterImage = rootVisualElement.Q<VisualElement>("CharacterImage");
        if(charaterImage == null){
            Debug.Log("didn't find image");
        }
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityManager.CompleteAllJobs();
        var DeltaTime = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .WithNone<DialogueData>()
        .ForEach((ref TextBoxData textBoxData, ref Entity entity, ref DynamicBuffer<Text> text, in AudioInfo audioInfo, in ImagesData images) => {
            /*textBoxData.timeFromLastChar += DeltaTime;
            if(textBoxData.currentPage >= text.Length){
                    charaterText.visible = false;
                    ecb.RemoveComponent<TextBoxData>(entity);
                    ecb.RemoveComponent<Text>(entity);
            }
            else{
                while(textBoxData.timeFromLastChar >= charTime && !textBoxData.isFinishedPage){
                    string textstring = text[textBoxData.currentPage].text.ToString();
                    if(textBoxData.currentChar == 0){
                        textBoxText.text = "";
                        charaterImage.style.backgroundImage = images.images[textBoxData.currentPage];;
                        AudioManager.playDialogue(audioInfo.audioName, textBoxData.currentPage);
                    }
                    textBoxText.text += textstring[textBoxData.currentChar];
                    textBoxData.currentChar++;
                    textBoxData.timeFromLastChar -= charTime;
                    if(textBoxData.currentChar >= textstring.Length){
                        textBoxData.isFinishedPage = true;
                    }
                }
            }*/
        }).Run();
    }
}

