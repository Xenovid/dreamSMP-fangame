using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class CameraSystem : SystemBase
{
    private EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((CameraData cameraData) => {
                Transform transform = cameraData.camera.GetComponent<Transform>();
                Translation playerPosition = manager.GetComponentData<Translation>(cameraData.player);
                transform.position = playerPosition.Value + cameraData.offset;
            }).Run();
    }
}
