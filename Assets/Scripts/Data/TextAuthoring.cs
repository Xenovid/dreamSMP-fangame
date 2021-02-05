using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TextAuthoring : MonoBehaviour{
        public string[] text;
}
public struct Text : IBufferElementData
{
        public FixedString512 text;
}

class TextConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((TextAuthoring textAuthoring) => {
                var entity = GetPrimaryEntity(textAuthoring);
                DstEntityManager.AddBuffer<Text>(entity);
                DynamicBuffer<Text> texts = DstEntityManager.GetBuffer<Text>(entity);
                foreach(string str in textAuthoring.text){
                        FixedString512 temp = str;
                        texts.Add(new Text{text = temp});
                }
        });
    }
}

