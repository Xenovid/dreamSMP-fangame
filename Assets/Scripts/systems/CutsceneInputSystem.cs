using Unity.Entities;
using UnityEngine;

public class CutsceneInputSystem : SystemBase
{
      protected override void OnUpdate()
      {
        EntityQuery textBoxGroup = GetEntityQuery(typeof(Text), typeof(TextBoxData));
        Nativ textBoxEntities = textBoxGroup.ToEntityArray;

        Entities.ForEach((in CutsceneData cutsceneData, in SelectionInputData selection) => {
        if(cutsceneData.isReadingDialogue){
            
        }
        })
      }
}
