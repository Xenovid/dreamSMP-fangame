using Unity.Entities;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class CameraConvertionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Camera camera, AudioListener audio,  UniversalAdditionalCameraData universe) => {
            AddHybridComponent(universe);
                AddHybridComponent(camera);
                AddHybridComponent(audio);
        });
    }
}
