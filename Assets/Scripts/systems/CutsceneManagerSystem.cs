using Unity.Entities;

public class CutsceneManagerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float DeltaTime = Time.DeltaTime;
        Entities.ForEach((ref CutsceneManagerData cutsceneManager) => {
            cutsceneManager.totalTime += DeltaTime;
        }).Schedule();
    }
}
