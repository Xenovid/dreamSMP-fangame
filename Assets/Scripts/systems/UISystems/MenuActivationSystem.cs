using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

public class MenuActivationSystem : SystemBase
{
    private SceneSystem sceneSystem;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    private Entity pauseMenuSubScene;
    private bool loadedAMenu = true;
    

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        loadedAMenu = true;

        Entities
        .WithoutBurst()
        .WithAll<PauseMenuSubSceneTag>()
        .ForEach((Entity ent) =>{
            pauseMenuSubScene = ent;
        }).Run();
    }

  

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<PlayerTag>()
        .WithNone<stopInputTag>()
        .ForEach((Entity entity, in DelayedInputData input) => {
            if(input.isEscapePressed && !input.wasEscapePressed && !loadedAMenu){
                // load pause menu
                loadedAMenu = true;
                    sceneSystem.LoadSceneAsync(pauseMenuSubScene);
                ecb.AddComponent<stopInputTag>(entity);
            }
            else{
                loadedAMenu = false;
            }
        }).Run();
    }
  }
