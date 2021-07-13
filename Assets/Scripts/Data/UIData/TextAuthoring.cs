using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TextAuthoring : MonoBehaviour{
        public TextInfo[] textInfos;
}
public struct TextInfo{
        public string text;
        public bool instant;
        public string dialogueSoundName;
}
public struct Text : IBufferElementData
{
        public FixedString32 dialoguePortraitName;
        public FixedString512 text;
        public bool instant;
        public bool unSkipable;
        public float textSpeed;
        public FixedString32 dialogueSoundName;
}


class TextConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((TextAuthoring textAuthoring) => {
                var entity = GetPrimaryEntity(textAuthoring);
                DstEntityManager.AddBuffer<Text>(entity);
                //DynamicBuffer<Text> texts = DstEntityManager.GetBuffer<Text>(entity);
                /*foreach(TextInfo textInfo in textAuthoring.textInfos){
                        texts.Add(new Text{text = textInfo.text, instant = textInfo.instant});
                }*/
        });
    }
}

