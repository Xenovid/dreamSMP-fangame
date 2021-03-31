using Unity.Entities;
using Unity.Scenes;
using UnityEngine.UIElements;
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
    Entities.ForEach((Entity entity, SubScene scene ) =>{
    });
  }
}