using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class TextBoxSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private float charTime = 1;

    protected override void OnCreate(){
        base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        var DeltaTime = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .ForEach((Entity entity,TextBoxData textBoxData, Text text) => {
            textBoxData.timeFromLastChar += DeltaTime;
            while(textBoxData.timeFromLastChar >= charTime){
                if(textBoxData.currentChar == 0){
                    textBoxData.textBoxText.text = "";
                }
                textBoxData.textBoxText.text += text.text[textBoxData.currentPage][textBoxData.currentChar];
                textBoxData.currentChar++;
                textBoxData.timeFromLastChar -= charTime;
                if(textBoxData.currentChar >= text.text[textBoxData.currentPage].Length){
                    textBoxData.currentChar = 0;
                    textBoxData.currentPage++;
                }
                if(textBoxData.currentPage >= text.text.Length){
                    ecb.RemoveComponent<TextBoxData>(entity);
                    ecb.RemoveComponent<Text>(entity);
                    break;
                }
            }
        }).Run();
    }
}
