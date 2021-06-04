using Unity.Entities;
using UnityEngine;

public class DirectorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .WithAll<PlayOnAwakeTag>()
        .ForEach((Entity entity) => {
            Debug.Log("hellllll ya");
            //directorData.director.Play();
            EntityManager.RemoveComponent<PlayOnAwakeTag>(entity);
        }).Run();
    }
}
