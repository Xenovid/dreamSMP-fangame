using Unity.Collections;
using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class CutsceneInputSystem : SystemBase
{
      EntityManager entityManager;
      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
      EntityQuery textBoxGroup;
      Label textBoxText;

      protected override void OnCreate()
      {
        base.OnCreate();

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
          m_EndSimulationEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        
      }
      protected override void OnStartRunning()
      {
        base.OnStartRunning();
        /*EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDocument UIDoc = UIDocs[0];
        var rootVisualElement = UIDoc.rootVisualElement;
        textBoxText = rootVisualElement.Q<Label>("TextBoxText");*/
      }

      protected override void OnUpdate()
      {
        /*
        EntityCommandBuffer ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityManager.CompleteAllJobs();
        var textBoxGroup = entityManager.CreateEntityQuery(
          ComponentType.ReadWrite<Text>(),
          ComponentType.ReadWrite<TextBoxData>()
        );
        NativeArray<Entity> entities = textBoxGroup.ToEntityArray(Allocator.Temp);
        NativeArray<TextBoxData> textBoxDatas = textBoxGroup.ToComponentDataArray<TextBoxData>(Allocator.Temp);
        DynamicBuffer<Text> text;
        TextBoxData textBoxData;

        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

        Entities
        .WithoutBurst()
        .WithNone<DialogueData>()
        .ForEach((ref Entity entity, in CutsceneData cutsceneData) => {
          //there should only be one entity with a textbox and text at a given time
          text = EntityManager.GetBuffer<Text>(entities[0]);
          textBoxData = textBoxDatas[0];
          string textString = text[textBoxData.currentPage].text.ToString();
          int textLength = textString.Length;
          if(cutsceneData.isReadingDialogue){
              //when the player presses the selected button, it should either go to the next page or finish writing the current one
              if(input.goselected){
                if(textBoxData.isFinishedPage){
                  textBoxData.currentPage += 1;
                  textBoxData.isFinishedPage = false;
                  textBoxData.currentChar = 0;
                  textBoxDatas[0] = textBoxData;
                  textBoxGroup.CopyFromComponentDataArray<TextBoxData>(textBoxDatas);
                  if(textBoxData.currentPage >= text.Length){
                    ecb.RemoveComponent<CutsceneData>(entity);
                  }
                }
                else{
                  textBoxData.currentChar = textLength - 1;
                  textBoxData.isFinishedPage = true;
                  textBoxText.text = textString;
                }
              }
              textBoxDatas[0] = textBoxData;
          }
        }).Run();
        textBoxGroup.CopyFromComponentDataArray<TextBoxData>(textBoxDatas);
        textBoxDatas.Dispose();
        entities.Dispose();
        */
      }
      
}
