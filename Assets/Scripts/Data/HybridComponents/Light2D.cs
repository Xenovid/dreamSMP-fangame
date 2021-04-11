using Unity.Entities;
using UnityEngine.Experimental.Rendering.Universal;

public class Light2DConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Light2D light2D) =>{
            AddHybridComponent(light2D);
        });
    }
}
