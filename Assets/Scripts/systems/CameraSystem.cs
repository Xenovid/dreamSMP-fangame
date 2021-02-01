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
            .ForEach((FollowingData followingData) => {
                Transform transform = followingData.gameObjectFollowing.GetComponent<Transform>();
                Translation entityPosition = manager.GetComponentData<Translation>(followingData.entityToFollow);
                transform.position = entityPosition.Value + followingData.offset;
            }).Run();
    }
}
