using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CutsceneInputSystem : SystemBase
{
      EntityManager entityManager;
      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
      EntityQuery textBoxGroup;
      protected override void OnCreate()
      {
            base.OnCreate();

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            m_EndSimulationEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
      }

      protected override void OnUpdate()
      {
        EntityCommandBuffer ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        var textBoxGroup = entityManager.CreateEntityQuery(
          ComponentType.ReadWrite<Text>(),
          ComponentType.ReadWrite<TextBoxData>()
        );
        NativeArray<Entity> entities = textBoxGroup.ToEntityArray(Allocator.Temp);
        NativeArray<TextBoxData> textBoxDatas = textBoxGroup.ToComponentDataArray<TextBoxData>(Allocator.Temp);
        DynamicBuffer<Text> text;
        TextBoxData textBoxData;
        //there should only be one entity with a textbox and text at a given time
        text = EntityManager.GetBuffer<Text>(entities[0]);
        textBoxData = textBoxDatas[0];
        

        Entities.ForEach((ref Entity entity, in CutsceneData cutsceneData, in SelectionInputData selection) => {


          if(cutsceneData.isReadingDialogue){
              if(selection.isSelectedOrBack == 1){
                if(textBoxData.isFinishedPage){
                  textBoxData.currentPage += 1;
                  textBoxData.isFinishedPage = false;
                  textBoxData.currentChar = 0;
                  textBoxDatas[0] = textBoxData;
                  if(textBoxData.currentPage >= text.Length){
                    ecb.RemoveComponent<CutsceneData>(entity);
                  }
                }
              }
          }
        }).Run();
        textBoxGroup.CopyFromComponentDataArray<TextBoxData>(textBoxDatas);
        textBoxDatas.Dispose();
        entities.Dispose();
      }
}
