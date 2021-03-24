using Unity.Entities;

public class CutsceneManagerSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float DeltaTime = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, ref CutsceneManagerData cutsceneManager) => {
            cutsceneManager.totalTime += DeltaTime;
            if(cutsceneManager.dialogueLength < cutsceneManager.totalTime){
                EntityManager.RemoveComponent<CutsceneManagerData>(entity);
            }
        }).Run();
    }
}
