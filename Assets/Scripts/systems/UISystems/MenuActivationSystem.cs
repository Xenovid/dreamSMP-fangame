using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

public class MenuActivationSystem : SystemBase
{
    private SceneSystem sceneSystem;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private bool loadedAMenu;
    

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        loadedAMenu = true;
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
                Debug.Log("hello");
                // load pause menu
                loadedAMenu = true;
                sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.pauseMenuSubScene.SceneGUID);
                ecb.AddComponent<stopInputTag>(entity);
            }
            loadedAMenu = false;
        }).Run();

    }
  }
