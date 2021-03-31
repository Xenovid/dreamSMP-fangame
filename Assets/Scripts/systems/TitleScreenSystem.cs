using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class TitleScreenSystem : SystemBase
{
    private currentTitleMenu currentMenu;
    private titleMenuSelectables currentSelection;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        currentMenu = currentTitleMenu.MainMenu;
        currentSelection = titleMenuSelectables.Start;
    }

  

    protected override void OnUpdate()
    {


        Entities
        .WithoutBurst()
        .ForEach((in UIDocument UIDoc, in TitleMenuTag titleMenuTag, in UIInputData input) => {
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

                switch(currentMenu){
                    case currentTitleMenu.MainMenu:
                        Debug.Log("test");
                        switch(currentSelection){
                            case(titleMenuSelectables.Start):
                                if(input.goselected){
                                    Debug.Log("changing scenes");
                                    SceneManager.LoadSceneAsync("testScene");
                                }
                                else if(input.moveup){
                                    currentSelection = titleMenuSelectables.Exit;
                                    Debug.Log("moving buttons");

                                    exitButton.RemoveFromClassList("not_selected");
                                    exitButton.AddToClassList("selected");

                                    startButton.RemoveFromClassList("selected");
                                    startButton.AddToClassList("not_selected");

                                }
                                else if(input.movedown){
                                    currentSelection = titleMenuSelectables.Options;

                                    optionsButton.RemoveFromClassList("not_selected");
                                    optionsButton.AddToClassList("selected");

                                    startButton.RemoveFromClassList("selected");
                                    startButton.AddToClassList("not_selected");
                                }
                                break;
                            case(titleMenuSelectables.Options):
                                if(input.goselected){
                                    currentMenu = currentTitleMenu.OptionsMenu;
                                }
                                else if(input.moveup){
                                    currentSelection = titleMenuSelectables.Start;

                                    startButton.RemoveFromClassList("not_selected");
                                    startButton.AddToClassList("selected");

                                    optionsButton.RemoveFromClassList("selected");
                                    optionsButton.AddToClassList("not_selected");
                                }
                                else if(input.movedown){
                                    currentSelection = titleMenuSelectables.Credits;

                                    creditsButton.RemoveFromClassList("not_selected");
                                    creditsButton.AddToClassList("selected");

                                    optionsButton.RemoveFromClassList("selected");
                                    optionsButton.AddToClassList("not_selected");
                                }
                                break;
                            case(titleMenuSelectables.Credits):
                                if(input.goselected){
                                    currentMenu = currentTitleMenu.Credits;
                                }
                                else if(input.moveup){
                                    currentSelection = titleMenuSelectables.Options;

                                    optionsButton.RemoveFromClassList("not_selected");
                                    optionsButton.AddToClassList("selected");

                                    creditsButton.RemoveFromClassList("selected");
                                    creditsButton.AddToClassList("not_selected");

                                }
                                else if(input.movedown){
                                    currentSelection = titleMenuSelectables.Exit;

                                    exitButton.RemoveFromClassList("not_selected");
                                    exitButton.AddToClassList("selected");

                                    creditsButton.RemoveFromClassList("selected");
                                    creditsButton.AddToClassList("not_selected");
                                }
                                break;
                            case(titleMenuSelectables.Exit):
                                if(input.goselected){
                                    Application.Quit();
                                }
                                else if(input.moveup){
                                    currentSelection = titleMenuSelectables.Credits;

                                    creditsButton.RemoveFromClassList("not_selected");
                                    creditsButton.AddToClassList("selected");

                                    exitButton.RemoveFromClassList("selected");
                                    exitButton.AddToClassList("not_selected");

                                }
                                else if(input.movedown){
                                    currentSelection = titleMenuSelectables.Start;

                                    startButton.RemoveFromClassList("not_selected");
                                    startButton.AddToClassList("selected");

                                    exitButton.RemoveFromClassList("selected");
                                    exitButton.AddToClassList("not_selected");
                                }
                                break;
                        }
                        break;

                    case currentTitleMenu.OptionsMenu:
                        
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
