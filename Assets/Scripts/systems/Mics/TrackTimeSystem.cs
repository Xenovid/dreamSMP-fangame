using Unity.Entities;
[AlwaysUpdateSystem]
public class TrackTimeSystem : SystemBase
{
    EntityQuery timeQuery;
    protected override void OnCreate()
    {
        timeQuery = GetEntityQuery(typeof(SavePointData));
    }
    protected override void OnUpdate()
    {
        if(timeQuery.CalculateEntityCount() == 0){
            EntityManager.CreateEntity(typeof(SavePointData));
        }
        SavePointData time = timeQuery.GetSingleton<SavePointData>();
        time.timePassed += Time.DeltaTime;
        timeQuery.SetSingleton<SavePointData>(time);
    }
}
