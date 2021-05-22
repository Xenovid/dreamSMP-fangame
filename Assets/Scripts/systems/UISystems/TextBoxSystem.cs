using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;
using UnityEngine.UIElements;

public class TextBoxSystem : SystemBase
{
    public event EventHandler OnDisplayFinished;
    bool isDisplaying;
    protected override void OnStartRunning()
    {
        
    }
    protected override void OnUpdate()
    {
        UIInputData input = GetSingleton<UIInputData>();
        Entities
        .WithoutBurst()
        .WithAll<OverworldUITag>()
        .ForEach((UIDocument UIDoc, DynamicBuffer<Text> texts) =>{
                if(!texts.IsEmpty && !isDisplaying){
                    InputGatheringSystem.currentInput = CurrentInput.ui;
                    VisualElement root = UIDoc.rootVisualElement;
                    VisualElement characterText = root.Q<VisualElement>("TextBoxUI");
                    Label textBoxText = root.Q<Label>("TextBoxText");
                    VisualElement charaterImage = root.Q<VisualElement>("CharacterImage");

                    characterText.visible = true;
                    textBoxText.text = texts[0].text.ToString();
                    isDisplaying = true;
                }
                if(isDisplaying){
                    VisualElement root = UIDoc.rootVisualElement;
                    VisualElement characterText = root.Q<VisualElement>("TextBoxUI");
                    Label textBoxText = root.Q<Label>("TextBoxText");
                    VisualElement charaterImage = root.Q<VisualElement>("CharacterImage");
                    if(input.goselected){
                        if(texts.Length == 1){
                            texts.RemoveAt(0);
                            isDisplaying = false;
                            characterText.visible = false;
                            textBoxText.text = "";
                            InputGatheringSystem.currentInput = CurrentInput.overworld;
                            OnDisplayFinished?.Invoke(this, EventArgs.Empty);
                        }
                        else{
                            texts.RemoveAt(0);
                            textBoxText.text = texts[0].text.ToString();
                        }
                    }

                }
        }).Run();
    }
}



    /*
    old text box system
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private float charTime = .5f;
    Label textBoxText;
    VisualElement charaterText;
    VisualElement charaterImage;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument), typeof(OverworldUITag));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDocument UIDoc = UIDocs[0];

        var rootVisualElement = UIDoc.rootVisualElement;
        charaterText = rootVisualElement.Q<VisualElement>("TextBoxUI");
        textBoxText = rootVisualElement.Q<Label>("TextBoxText");
        charaterImage = rootVisualElement.Q<VisualElement>("CharacterImage");
    }
    
    protected override void OnUpdate()
    { /*
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityManager.CompleteAllJobs();
        var DeltaTime = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .WithNone<DialogueData,VictoryData>()
        .ForEach((ref TextBoxData textBoxData, ref Entity entity, ref DynamicBuffer<Text> text, in ImagesData images) => {
            if(textBoxData.currentPage >= text.Length){
                    charaterText.visible = false;
                    ecb.RemoveComponent<TextBoxData>(entity);
                    InputGatheringSystem.currentInput = CurrentInput.overworld;
            }
            else if(input.goselected)
            {
                if (textBoxData.isFinishedPage)
                {
                    textBoxData.timeFromLastChar = 0.0f;
                    textBoxData.currentPage += 1;
                    textBoxData.isFinishedPage = false;
                    textBoxData.currentChar = 0;
                }
                else
                {
                    string temp = text[textBoxData.currentPage].text.ToString();
                    textBoxData.currentChar = temp.Length - 1;
                    textBoxData.isFinishedPage = true;
                    textBoxText.text = temp;
                }
            }
            else
            {
                charaterText.visible = true;
                if (!textBoxData.isFinishedPage)
                {
                    textBoxData.timeFromLastChar += DeltaTime;
                }
                textBoxData.timeFromLastChar += DeltaTime;
                while (textBoxData.timeFromLastChar >= charTime && !textBoxData.isFinishedPage){
                    string textstring = text[textBoxData.currentPage].text.ToString();
                    if(textBoxData.currentChar == 0){
                        textBoxText.text = "";
                        if(images.images[textBoxData.currentPage] == null)
                        {
                            charaterImage.style.width = 0;
                        }
                        else
                        {
                            charaterImage.style.width = charaterImage.contentRect.height;
                            charaterImage.style.backgroundImage = images.images[textBoxData.currentPage];
                        }
                    }
                    textBoxText.text += textstring[textBoxData.currentChar];
                    textBoxData.currentChar++;
                    textBoxData.timeFromLastChar -= charTime;
                    if(textBoxData.currentChar >= textstring.Length){
                        textBoxData.isFinishedPage = true;
                    }
                }
            }
        }).Run();
    }

    
}*/