using Unity.Entities;
using Unity.Scenes;
using Unity.Collections;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class titleSubSceneLoader : ComponentSystem
{
  private SceneSystem sceneSystem;
  protected override void OnCreate()
  {
    sceneSystem = World.GetOrCreateSystem<SceneSystem>();
  }

    

  protected override void OnUpdate()  
  {
    EntityQuery SubSceneDataGroup = GetEntityQuery(typeof(SubSceneData));
    NativeArray<SubSceneData> subSceneDatas = SubSceneDataGroup.ToComponentDataArray<SubSceneData>(Allocator.TempJob);

    Entities
    .ForEach((Entity entity, SubScene scene ) =>{
      //EntityManager.RemoveComponent<RequestSceneLoaded>(entity);
      //sceneSystem.UnloadScene(scene.SceneGUID, SceneSystem.UnloadParameters.DontRemoveRequestSceneLoaded);
      foreach(SubSceneData subSceneData in subSceneDatas){
        if(subSceneData.subsceneName.ToString() == scene.SceneName && !subSceneData.shouldBeLoaded){
          Debug.Log("elllo");
          sceneSystem.UnloadScene(entity);
        }
      }
    });

    subSceneDatas.Dispose();
  }
}