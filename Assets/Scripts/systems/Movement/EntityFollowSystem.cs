using Unity.Transforms;
using Unity.Entities;
[UpdateAfter(typeof(MovementSystem))]
public class EntityFollowSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .ForEach((ref Translation translation, in EntityFollowEntityData followData) => {
            LocalToWorld playerTranslation = EntityManager.GetComponentData<LocalToWorld>(followData.following);
            translation.Value = playerTranslation.Position + followData.offset;
        }).Run();
    }
}
