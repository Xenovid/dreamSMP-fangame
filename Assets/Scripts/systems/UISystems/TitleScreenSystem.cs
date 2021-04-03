using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Scenes;
using System.Collections;

public class TitleScreenSystem : SystemBase
{
    private titleMenuSelectables currentSelection;
    private SceneSystem sceneSystem;

    protected override void OnStartRunning()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        base.OnStartRunning();

        currentSelection = titleMenuSelectables.Start;
    }

  

    protected override void OnUpdate()
    {


        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<TitleMenuTag>()
        .WithNone<OptionMenuTag>()
        .ForEach((in UIDocument UIDoc, in UIInputData input) => {
            VisualElement root = UIDoc.rootVisualElement;
            if(root == null){
                Debug.Log("root wasn't found");
            }
            else{
                //references to the ui
                VisualElement startButton = root.Q<VisualElement>("Start");
                VisualElement optionsButton = root.Q<VisualElement>("Options");
                VisualElement creditsButton = root.Q<VisualElement>("Credits");
                VisualElement exitButton = root.Q<VisualElement>("exit");
                        switch(currentSelection){
                            case(titleMenuSelectables.Start):
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    sceneSystem.UnloadScene(TitleSubSceneReferences.Instance.titleSubScene.SceneGUID);
                                    EntityManager.CompleteAllJobs();
                                    SceneManager.LoadSceneAsync("testScene");
                                }
                                else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Exit;
                                    Debug.Log("moving buttons");

                                    exitButton.RemoveFromClassList("not_selected");
                                    exitButton.AddToClassList("selected");

                                    startButton.RemoveFromClassList("selected");
                                    startButton.AddToClassList("not_selected");

                                }
                                else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Options;

                                    optionsButton.RemoveFromClassList("not_selected");
                                    optionsButton.AddToClassList("selected");

                                    startButton.RemoveFromClassList("selected");
                                    startButton.AddToClassList("not_selected");
                                }
                                break;
                            case(titleMenuSelectables.Options):
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    sceneSystem.LoadSceneAsync(TitleSubSceneReferences.Instance.OptionSubScene.SceneGUID);
                                    sceneSystem.UnloadScene(TitleSubSceneReferences.Instance.titleSubScene.SceneGUID);
                                }
                                else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Start;

                                    startButton.RemoveFromClassList("not_selected");
                                    startButton.AddToClassList("selected");

                                    optionsButton.RemoveFromClassList("selected");
                                    optionsButton.AddToClassList("not_selected");
                                }
                                else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Credits;

                                    creditsButton.RemoveFromClassList("not_selected");
                                    creditsButton.AddToClassList("selected");

                                    optionsButton.RemoveFromClassList("selected");
                                    optionsButton.AddToClassList("not_selected");
                                }
                                break;
                            case(titleMenuSelectables.Credits):
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    sceneSystem.LoadSceneAsync(TitleSubSceneReferences.Instance.CreditsSubScene.SceneGUID);
                                    sceneSystem.UnloadScene(TitleSubSceneReferences.Instance.titleSubScene.SceneGUID);
                                }
                                else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Options;

                                    optionsButton.RemoveFromClassList("not_selected");
                                    optionsButton.AddToClassList("selected");

                                    creditsButton.RemoveFromClassList("selected");
                                    creditsButton.AddToClassList("not_selected");

                                }
                                else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Exit;

                                    exitButton.RemoveFromClassList("not_selected");
                                    exitButton.AddToClassList("selected");

                                    creditsButton.RemoveFromClassList("selected");
                                    creditsButton.AddToClassList("not_selected");
                                }
                                break;
                            case(titleMenuSelectables.Exit):
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    Application.Quit();
                                }
                                else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Credits;

                                    creditsButton.RemoveFromClassList("not_selected");
                                    creditsButton.AddToClassList("selected");

                                    exitButton.RemoveFromClassList("selected");
                                    exitButton.AddToClassList("not_selected");

                                }
                                else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Start;

                                    startButton.RemoveFromClassList("not_selected");
                                    startButton.AddToClassList("selected");

                                    exitButton.RemoveFromClassList("selected");
                                    exitButton.AddToClassList("not_selected");
                                }
                                break;


                }
            }
            }).Run();
        }
}

public enum currentTitleMenu{
    MainMenu,
    OptionsMenu,
    Credits
}
public enum titleMenuSelectables{
    Start, 
    Options,
    Credits,
    Exit
}
public class temp : MonoBehaviour{
    public static void quitGame(){
        Application.Quit(0);
    }
}
