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
      
      private Entity pauseMenuSubScene;

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
                  //UIDoc.rootVisualElement.visible = false;
            }

            Entities
            .WithoutBurst()
            .WithAll<PauseMenuSubSceneTag>()
            .ForEach((Entity ent) =>{
                  pauseMenuSubScene = ent;
            }).Run();
            
      }

      protected override void OnUpdate()
      {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithAll<PauseMenuTag>()
            .ForEach((in UIDocument UIDoc) =>{
                  VisualElement root = UIDoc.rootVisualElement;
                  if(root == null){

                  }
                  else{
                        Label titleScreenButton = root.Q<Label>("TitleScreenButton");
                        Label resumeButton = root.Q<Label>("ResumeButton");
                        switch(currentSelection){
                              case PauseMenuSelectables.Resume:
                                    if(input.goselected || input.goback){ 
                                          AudioManager.playSound("menuchange");
                                          sceneSystem.UnloadScene(pauseMenuSubScene);
                                        InputGatheringSystem.currentInput = CurrentInput.overworld;
                                    }
                                    else if(input.movedown || input.moveup){
                                          AudioManager.playSound("menuchange");
                                          currentSelection = PauseMenuSelectables.Title;
                                          resumeButton.RemoveFromClassList("selected");
                                          titleScreenButton.AddToClassList("selected");
                                    }
                                    break;
                              case PauseMenuSelectables.Title:
                                    if(input.goselected){
                                          AudioManager.playSound("menuchange");
                                          //load title
                                          sceneSystem.UnloadScene(pauseMenuSubScene);
                                          sceneSystem.UnloadScene(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
                                          sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
                                    }
                                    else if(input.goback){
                                          AudioManager.playSound("menuchange");
                                          sceneSystem.UnloadScene(pauseMenuSubScene);
                                        InputGatheringSystem.currentInput = CurrentInput.overworld;
                                    }
                                    else if(input.moveup || input.movedown){
                                          AudioManager.playSound("menuchange");
                                          currentSelection = PauseMenuSelectables.Resume;

                                          titleScreenButton.RemoveFromClassList("selected");
                                          resumeButton.AddToClassList("selected");
                                    }
                                    break;
                        }
                        
                  }
            }).Run();
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
