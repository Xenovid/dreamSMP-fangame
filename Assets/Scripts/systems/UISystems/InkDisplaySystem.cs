using Unity.Entities;
using UnityEngine.UIElements;
using System.Linq;
using Ink.Runtime;
using System;
using UnityEngine;

public class InkDisplaySystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityManager.CompleteAllJobs();
        var DeltaTime = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .ForEach((InkManagerData inkManagager, ref Entity entity) => {
            if(inkManagager.inkStory == null){
                inkManagager.inkStory = new Story(inkManagager.inkAssest.text);
            }
        }).Run(); 
    }
    public void DisplayVictoryData(){
        Entity messageBoard = GetSingletonEntity<UITag>();
        DynamicBuffer<Text> texts = GetBuffer<Text>(messageBoard);
        Entities
        .WithoutBurst()
        .ForEach((InkManagerData inkManager) => {
            Debug.Log("adding text");
            inkManager.inkStory.ChoosePathString("victory");
            while(inkManager.inkStory.canContinue){
                texts.Add(new Text{text = inkManager.inkStory.Continue(), instant = true});
            }
        }).Run();
    }
    public void StartCutScene(String startPoint){

        Entity messageBoard = GetSingletonEntity<UITag>();
        DynamicBuffer<Text> texts = GetBuffer<Text>(messageBoard);
        Entities
        .WithoutBurst()
        .ForEach((InkManagerData inkManager) => {
            inkManager.inkStory.ChoosePathString(startPoint);
            int i = 0;
            while(inkManager.inkStory.canContinue){
                Text text = new Text{text = inkManager.inkStory.Continue(), dialogueSoundName = "default"};
                // text is meant to be a sound to be played
                if(inkManager.inkStory.currentTags.Contains("sound")){
                    //for some reason it has white space, so it has to be removed
                    AudioManager.playSound(String.Concat(text.text.ToString().Where(c => !Char.IsWhiteSpace(c))));
                }
                else{
                    text.textSpeed = 1f;
                    if(inkManager.inkStory.currentTags.Contains("unskipable")){
                        text.unSkipable = true;
                    }
                    if(inkManager.inkStory.currentTags.Contains("slow")){
                        text.textSpeed = .2f;
                    }
                    if(inkManager.inkStory.currentTags.Contains("instant")){
                        text.instant = true;
                    }
                    if(inkManager.inkStory.currentTags.Contains("technoblade")){
                        
                        text.dialoguePortraitName = "technoblade";
                        text.dialogueSoundName = "technoblade";
                    }
                    if(inkManager.inkStory.currentTags.Contains("bell")){
                        text.dialogueSoundName = "bell";
                    }
                    texts.Add(text);
                    i++;
                }
                
            }
        }).Run();
    }
}
