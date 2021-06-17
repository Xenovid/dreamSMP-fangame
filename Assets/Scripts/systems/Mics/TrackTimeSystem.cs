using Unity.Entities;

public class TrackTimeSystem : SystemBase
{
    EntityQuery timeQuery;
    protected override void OnCreate()
    {
        timeQuery = GetEntityQuery(typeof(TimePassedData));
    }
    protected override void OnUpdate()
    {
        if(timeQuery.CalculateEntityCount() == 0){
            EntityManager.CreateEntity(typeof(TimePassedData));
        }
        TimePassedData time = timeQuery.GetSingleton<TimePassedData>();
        time.timePassed += Time.DeltaTime;
        timeQuery.SetSingleton<TimePassedData>(time);
    }
}
