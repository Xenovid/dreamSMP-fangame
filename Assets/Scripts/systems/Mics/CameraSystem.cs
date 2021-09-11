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
        EntityQuery LeadPlayerQuery = GetEntityQuery(typeof(LeadPlayerTag));
        CameraData cameraData = GetSingleton<CameraData>();
        Entity cameraDataEntity = GetSingletonEntity<CameraData>();

        switch(cameraData.currentState){
            case CameraState.FollingPlayer:
                if(LeadPlayerQuery.CalculateEntityCount() > 0){
                    Entity leadPlayerTag = GetSingletonEntity<LeadPlayerTag>();
                    Translation playerTranslation = GetComponent<Translation>(leadPlayerTag);
                    CameraTransformRef.instance.transform.position = playerTranslation.Value;
                }
            break;
            case CameraState.FreeForm:
                if(HasComponent<CameraTransitionData>(cameraDataEntity)){
                    CameraTransitionData cameraTransitionData = GetSingleton<CameraTransitionData>();
                    cameraTransitionData.timePassed = Time.DeltaTime;
                    math.lerp(cameraTransitionData.oldPosition,cameraTransitionData.newPosition, cameraTransitionData.timePassed/ cameraTransitionData.duration);
                    if(cameraTransitionData.timePassed > cameraTransitionData.duration){
                        EntityManager.RemoveComponent<CameraTransitionData>(cameraDataEntity);
                    }
                    else{
                        SetSingleton<CameraTransitionData>(cameraTransitionData);
                    }
                    
                }
            break;
        }
        /*
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
            }).Run();*/
        
    }
}
