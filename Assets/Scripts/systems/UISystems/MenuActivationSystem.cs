using Unity.Entities;
using Unity.Scenes;
using UnityEngine.InputSystem;
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
        .WithAll<PauseMenuSubSceneTag>()
        .ForEach((Entity ent) => {
            pauseMenuSubScene = ent;
        }).Run();


        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((in OverworldInputData input) => {
            if(input.escape && !loadedAMenu){
                // load pause menu
                loadedAMenu = true;
                sceneSystem.LoadSceneAsync(pauseMenuSubScene);
                InputGatheringSystem.currentInput = CurrentInput.ui;
            }
            else{
                loadedAMenu = false;
            }
        }).Run();
    }
  }
