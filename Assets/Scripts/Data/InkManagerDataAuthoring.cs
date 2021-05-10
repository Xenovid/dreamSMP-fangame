using Unity.Entities;
using Ink.Runtime;
using UnityEngine;

public class InkManagerDataAuthoring : MonoBehaviour
{
    public TextAsset inkAsset;
    public Story inkStory {get; set;}
}
public class InkManagerData : IComponentData{
    public TextAsset inkAssest;
    public Story inkStory {get; set;}
    public bool iswritingDialogue;
}
public class InkManagerConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, InkManagerDataAuthoring inkManager) => {
            //Story temp = new Story(inkManager.inkAsset.text);
            DstEntityManager.AddComponentData(entity,new InkManagerData{inkStory = new Story(inkManager.inkAsset.text), inkAssest = inkManager.inkAsset});
            InkManagerData inkData = DstEntityManager.GetComponentData<InkManagerData>(entity);
        });
    }
}

