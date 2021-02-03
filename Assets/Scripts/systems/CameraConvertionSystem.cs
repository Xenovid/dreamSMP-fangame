using Unity.Entities;
using UnityEngine;

public class CameraConvertionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Camera camera, AudioListener audio) => {
                AddHybridComponent(camera);
                AddHybridComponent(audio);
        });
    }
}
