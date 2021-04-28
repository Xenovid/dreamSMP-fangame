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

            
            
      }

      protected override void OnUpdate()
      {
        Entities
            .WithoutBurst()
            .WithAll<PauseMenuSubSceneTag>()
            .ForEach((Entity ent) => {
                pauseMenuSubScene = ent;
            }).Run();
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((in UIDocument UIDoc,in PauseMenuTag pausMenuTag) =>{
                  VisualElement root = UIDoc.rootVisualElement;
                  if(root == null){
                
                  }
                  else{
                        Label titleScreenButton = root.Q<Label>("TitleScreenButton");
                        Label resumeButton = root.Q<Label>("ResumeButton");
                        switch(currentSelection){
                              case PauseMenuSelectables.Resume:
                                    if(input.goselected || input.goback){
                                        root.visible = false;
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
                                          EntityManager.World.GetExistingSystem<TitleScreenSystem>().Enabled = true;
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
}

public enum PauseMenuSelectables{
      Resume,
      Options,
      Title
}
