using Unity.Entities;
using UnityEngine.Rendering;
using UnityEngine;

public class VolumeConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Volume volume) =>
        {
            AddHybridComponent(volume);
        });
    }
}
