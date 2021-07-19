using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class VFXConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {

        Entities
        .ForEach((VisualEffect visualEffect) =>
        {
            //AddHybridComponent(visualEffect);
        });
    }
    
}
