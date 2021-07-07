using Unity.Entities;
using UnityEngine.UIElements;
using Ink.Runtime;
using System;
using UnityEngine;

public class InkDisplaySystem : SystemBase
{
    public event EventHandler OnWritingFinished;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    UIDocument UIDoc;
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
                texts.Add(new Text{text = inkManager.inkStory.Continue()});
                if(inkManager.inkStory.currentTags.Count > 0){
                    Debug.Log("has tags");
                }
                else{
                    Debug.Log("doesn't have tags");
                }
            }
        }).Run();
    }
    public void StartCutScene(String startPoint){;
        Entity messageBoard = GetSingletonEntity<UITag>();
        DynamicBuffer<Text> texts = GetBuffer<Text>(messageBoard);
        CharacterPortraitData characterPortraits = EntityManager.GetComponentObject<CharacterPortraitData>(messageBoard);
        Entities
        .WithoutBurst()
        .ForEach((InkManagerData inkManager) => {
            inkManager.inkStory.ChoosePathString(startPoint);
            int i = 0;
            characterPortraits.portraits.Clear();
            while(inkManager.inkStory.canContinue){
                Text text = new Text{text = inkManager.inkStory.Continue()};
                characterPortraits.portraits.Add(Resources.Load<Sprite>("CharacterPortraits/Default"));
                if(inkManager.inkStory.currentTags.Contains("instant")){
                    text.instant = true;
                }
                if(inkManager.inkStory.currentTags.Contains("technoblade")){
                    Debug.Log("should do techno portriat");
                    characterPortraits.portraits[i] = Resources.Load<Sprite>("CharacterPortraits/TechnoDefault");
                }
                texts.Add(text);
                i++;
            }
        }).Run();
    }

}
