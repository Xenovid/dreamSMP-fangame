using Unity.Entities;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class AdditionalCameraData : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((UniversalAdditionalCameraData universe) =>
        {
            //AddHybridComponent(universe);
        });        
    }
}
