using Unity.Entities;
using UnityEngine.UIElements;
using Unity.Scenes;
using Unity.Collections;
using Unity.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuSystem : SystemBase
{
      private PauseMenuSelectables currentSelection;
      private SceneSystem sceneSystem;

      protected override void OnStartRunning()
      {
            base.OnStartRunning();
            sceneSystem = World.GetOrCreateSystem<SceneSystem>();

            currentSelection = PauseMenuSelectables.Resume;

            var queryDescription = new EntityQueryDesc{
                  None = new ComponentType[] {typeof(PauseMenuTag)},
                  All = new ComponentType[] {typeof(UIDocument)}
            };
            EntityQuery UIGroup = GetEntityQuery(queryDescription);
            UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
            foreach(UIDocument UIDoc in UIDocs){
                  UIDoc.rootVisualElement.visible = false;
            }

            
      }

      protected override void OnUpdate()
      {
            EntityQuery stopedInputGroup = GetEntityQuery(typeof(stopInputTag));
            NativeArray<Entity> stopedEntities = stopedInputGroup.ToEntityArray(Allocator.TempJob);
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithAll<PauseMenuTag>()
            .ForEach((in UIDocument UIDoc,in UIInputData input) =>{
                  VisualElement root = UIDoc.rootVisualElement;
                  if(root == null){

                  }
                  else{
                        Label titleScreenButton = root.Q<Label>("TitleScreenButton");
                        Label resumeButton = root.Q<Label>("ResumeButton");
                        switch(currentSelection){
                              case PauseMenuSelectables.Resume:
                                    if(input.goselected || input.goback){ 
                                          sceneSystem.UnloadScene(SubSceneReferences.Instance.pauseMenuSubScene.SceneGUID);
                                          foreach(Entity ent in stopedEntities){
                                                EntityManager.RemoveComponent<stopInputTag>(ent);
                                          }

                                          //unload pause menu
                                    }
                                    else if(input.movedown || input.moveup){
                                          currentSelection = PauseMenuSelectables.Title;
                                          resumeButton.RemoveFromClassList("selected");
                                          titleScreenButton.AddToClassList("selected");
                                    }
                                    break;
                              case PauseMenuSelectables.Title:
                                    if(input.goselected){
                                          //load title
                                          sceneSystem.UnloadScene(SubSceneReferences.Instance.pauseMenuSubScene.SceneGUID);
                                          sceneSystem.UnloadScene(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
                                          SceneManager.LoadSceneAsync("StartMenu");
                                    }
                                    else if(input.goback){
                                          sceneSystem.UnloadScene(SubSceneReferences.Instance.pauseMenuSubScene.SceneGUID);
                                          foreach(Entity ent in stopedEntities){
                                                EntityManager.RemoveComponent<stopInputTag>(ent);
                                          }
                                          //unload pause menu
                                    }
                                    else if(input.moveup || input.movedown){
                                          currentSelection = PauseMenuSelectables.Resume;

                                          titleScreenButton.RemoveFromClassList("selected");
                                          resumeButton.AddToClassList("selected");
                                    }
                                    break;
                        }
                        
                  }
            }).Run();
            stopedEntities.Dispose();
      }
      protected override void OnStopRunning()
      {
            base.OnStopRunning();
            var queryDescription = new EntityQueryDesc{
                  None = new ComponentType[] {typeof(PauseMenuTag)},
                  All = new ComponentType[] {typeof(UIDocument)}
            };
            EntityQuery UIGroup = GetEntityQuery(queryDescription);
            UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
            foreach(UIDocument UIDoc in UIDocs){
                  UIDoc.rootVisualElement.visible = true;
            }
      }
}

public enum PauseMenuSelectables{
      Resume,
      Options,
      Title
}
