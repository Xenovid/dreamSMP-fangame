using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class TextBoxSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private float charTime = 1.0f;
    Label textBoxText;
    VisualElement charaterText;

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
        charaterText = rootVisualElement.Q<VisualElement>("characterText");
        textBoxText = rootVisualElement.Q<Label>("text");
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityManager.CompleteAllJobs();
        var DeltaTime = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .ForEach((ref TextBoxData textBoxData, ref Entity entity, ref DynamicBuffer<Text> text) => {
            textBoxData.timeFromLastChar += DeltaTime;
            Debug.Log(textBoxData.currentPage);
            Debug.Log(text.Length);
            if(textBoxData.currentPage >= text.Length){
                    charaterText.visible = false;
                    ecb.RemoveComponent<TextBoxData>(entity);
                    ecb.RemoveComponent<Text>(entity);
            }
            while(textBoxData.timeFromLastChar >= charTime && !textBoxData.isFinishedPage){
                string textstring = text[textBoxData.currentPage].text.ToString();
                if(textBoxData.currentChar == 0){
                    textBoxText.text = "";
                }
                textBoxText.text += textstring[textBoxData.currentChar];
                textBoxData.currentChar++;
                textBoxData.timeFromLastChar -= charTime;
                if(textBoxData.currentChar >= textstring.Length){
                    textBoxData.isFinishedPage = true;
                }
            }
        }).Run();
    }
}
