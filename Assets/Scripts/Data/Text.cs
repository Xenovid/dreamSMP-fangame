using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class Text : IComponentData
{
        public string[] text;
}

/*
class TextConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((TextAuthoring textAuthoring) => {
                var entity = GetPrimaryEntity(textAuthoring);

                DstEntityManager.AddComponentData(entity, new Text{
                        int i = 0;
                        foreach( string str in textAuthoring.text){
                                text = textAuthoring.text
                        }
                });
        });
    }
    */