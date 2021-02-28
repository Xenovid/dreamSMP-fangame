using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class TextBoxSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private float charTime = .1f;
    Label textBoxText;
    VisualElement charaterText;
    IMGUIContainer charaterImage;

    protected override void OnCreate(){
        base.OnCreate();
        
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnStartRunning(){
        base.OnStartRunning();

        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDocument UIDoc = UIDocs[0];
        if(UIDoc == null){
            Debug.Log("doc not found");
        }
        var rootVisualElement = UIDoc.rootVisualElement;
        charaterText = rootVisualElement.Q<VisualElement>("characterText");
        textBoxText = rootVisualElement.Q<Label>("text");
        charaterImage = rootVisualElement.Q<IMGUIContainer>("charaterImage");
    }
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        EntityManager.CompleteAllJobs();
        var DeltaTime = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .WithNone<AudioInfo>()
        .ForEach((ref TextBoxData textBoxData, ref Entity entity, ref DynamicBuffer<Text> text, in ImagesData images) => {
            textBoxData.timeFromLastChar += DeltaTime;
            if(textBoxData.currentPage >= text.Length){
                    charaterText.visible = false;
                    ecb.RemoveComponent<TextBoxData>(entity);
                    ecb.RemoveComponent<Text>(entity);
            }
            else{
                while(textBoxData.timeFromLastChar >= charTime && !textBoxData.isFinishedPage){
                    string textstring = text[textBoxData.currentPage].text.ToString();
                    if(textBoxData.currentChar == 0){
                        charaterImage.style.backgroundImage = images.images[textBoxData.currentPage];
                        textBoxText.text = "";
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
}
