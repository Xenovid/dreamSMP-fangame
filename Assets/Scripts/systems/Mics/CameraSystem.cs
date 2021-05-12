using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(MovementSystem))]
public class CameraSystem : SystemBase
{
    private EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    protected override void OnUpdate()
    {
        
        EntityQuery playergroup = GetEntityQuery(typeof(PlayerTag), typeof(Translation));
        NativeArray <Translation> playerTranslations = playergroup.ToComponentDataArray<Translation>(Allocator.Temp);
        Translation playerTranslation = playerTranslations[0];
        playerTranslations.Dispose();
            Entities
            .WithoutBurst()
            .ForEach((CameraData cameraData, ref Translation translation) => {
                //Transform transform = cameraData.cameraObject.GetComponent<Transform>();
                //Debug.Log(transform.position);
                //cameraData.cameraTransform.position = new Vector3(100, 100, 100);
                if (InputGatheringSystem.currentInput == CurrentInput.overworld)
                {
                    translation.Value = playerTranslation.Value + new float3(0, 0, -5);
                }
            }).Run();
        
    }
}
