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
    private SceneSectionData test;

    private Entity titleSubScene;
    private Entity optionsSubScene;
    private Entity creditsSubScene;

    protected override void OnStartRunning()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        base.OnStartRunning();

        currentSelection = titleMenuSelectables.Start;

        Entities
        .WithoutBurst()
        .WithAll<OptionsSubSceneTag>()
        .ForEach((Entity ent) => {
            optionsSubScene = ent;
        }).Run();
        Entities
        .WithoutBurst()
        .WithAll<CreditsSubSceneTag>()
        .ForEach((Entity ent) => {
            creditsSubScene = ent;
        }).Run();
        Entities
        .WithoutBurst()
        .WithAll<TitleSubSceneTag>()
        .ForEach((Entity ent) => {
            titleSubScene = ent;
        }).Run();

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
                                    sceneSystem.UnloadScene(titleSubScene);
                                    sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.WorldSubScene.SceneGUID);

                                }
                                else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Exit;

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
                                    sceneSystem.LoadSceneAsync(optionsSubScene);
                                    sceneSystem.UnloadScene(titleSubScene);
                                    currentSelection = titleMenuSelectables.Start;

                                    startButton.RemoveFromClassList("not_selected");
                                    startButton.AddToClassList("selected");

                                    optionsButton.RemoveFromClassList("selected");
                                    optionsButton.AddToClassList("not_selected");
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
                                    sceneSystem.LoadSceneAsync(creditsSubScene);
                                    sceneSystem.UnloadScene(titleSubScene);
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

                                    creditsButton.RemoveFromClassList("selected");
                                    creditsButton.AddToClassList("not_selected");

                                    exitButton.RemoveFromClassList("not_selected");
                                    exitButton.AddToClassList("selected");
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
