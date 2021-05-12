using Unity.Entities;
using UnityEngine.UIElements;
using Ink.Runtime;
using System;
using UnityEngine;

public class InkDisplaySystem : SystemBase
{
    public event EventHandler OnWritingFinished;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private float charTime = .5f;
    Label textBoxText;
    VisualElement charaterText;
    VisualElement charaterImage;
    bool willSwitchInput;

    UIDocument UIDoc;
    protected override void OnCreate()
    {
        base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument), typeof(OverworldUITag));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDoc = UIDocs[0];

    }

    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

        var root = UIDoc.rootVisualElement;
        if(root == null){
        }
        else{
        charaterText = root.Q<VisualElement>("TextBoxUI");
        textBoxText = root.Q<Label>("TextBoxText");
        charaterImage = root.Q<VisualElement>("CharacterImage");
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityManager.CompleteAllJobs();
        var DeltaTime = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .ForEach((InkManagerData inkManagager, ref TextBoxData textBoxData, ref Entity entity) => {
            if(inkManagager.inkStory == null){
                    inkManagager.inkStory = new Story(inkManagager.inkAssest.text);
            }
            if(!inkManagager.iswritingDialogue){
            }
            else if(!inkManagager.inkStory.canContinue && textBoxData.isFinishedPage && input.goselected){
                textBoxData.currentSentence = "";
                textBoxData.isFinishedPage = false;
                textBoxData.currentChar = 0;
                
                inkManagager.iswritingDialogue = false;
                charaterText.visible = false;
                InputGatheringSystem.currentInput = willSwitchInput ? CurrentInput.overworld : CurrentInput.ui;
                OnWritingFinished?.Invoke(this, EventArgs.Empty);
            }
            else if(input.goselected)
            {
                if (textBoxData.isFinishedPage)
                {
                    textBoxData.timeFromLastChar = 0.0f;
                    textBoxData.isFinishedPage = false;
                    textBoxData.currentChar = 0;
                }
                else
                {
                    textBoxData.currentChar = textBoxData.currentSentence.Length - 1;
                    textBoxData.isFinishedPage = true;
                    textBoxText.text = textBoxData.currentSentence.ToString();
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
                    if(textBoxData.currentChar == 0){
                        textBoxData.currentSentence = inkManagager.inkStory.Continue();
                        textBoxText.text = "";
                        charaterImage.style.width = 0;
                    }
                    string textString = textBoxData.currentSentence.ToString();
                    textBoxData.currentChar++;
                    if(textBoxData.currentChar >= textString.Length){
                        textBoxData.isFinishedPage = true;
                    }
                    else{
                        textBoxText.text = textString.Substring(0,textBoxData.currentChar);
                    }
                    textBoxData.timeFromLastChar -= charTime;
                }
            }
        }).Run();
        }
    }
    public void DisplayVictoryData(){
        willSwitchInput= false;
        Entities
            .WithoutBurst()
            .ForEach((InkManagerData inkManager) => {
                inkManager.iswritingDialogue = true;
                inkManager.inkStory.ChoosePathString("victory");
            }).Run();
    }
    public void StartCutScene(String startPoint){
        willSwitchInput = true;
            Entities
            .WithoutBurst()
            .ForEach((InkManagerData inkManager) => {
                inkManager.iswritingDialogue = true;
                inkManager.inkStory.ChoosePathString(startPoint);
            }).Run();
    }
}
