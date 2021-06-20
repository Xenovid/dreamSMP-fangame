using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Scenes;
using System;
using System.IO;
using System.Collections;

public class TitleScreenSystem : SystemBase
{
    int currentSaveFile;
    public event StartGameEventHandler StartGame;
    public event EventHandler StartNewGame;
    private titleMenuSelectables currentSelection;
    private SceneSystem sceneSystem;
    private SceneSectionData test;
    private SettingsMenuSystem settingsMenuSystem;
    SaveAndLoadSystem saveAndLoadSystem;
    private bool isSelected;
    private bool isInSettings;
    private bool isLinked;


    protected override void OnStartRunning()
    {
        
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        saveAndLoadSystem = World.GetOrCreateSystem<SaveAndLoadSystem>();
        base.OnStartRunning();

        currentSelection = titleMenuSelectables.Start;
        AudioManager.playSong("menuMusic");

        settingsMenuSystem = World.GetOrCreateSystem<SettingsMenuSystem>();

        InputGatheringSystem.currentInput = CurrentInput.ui;
    }

  

    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<TitleMenuTag>()
        .WithNone<OptionMenuTag>()
        .ForEach((in UIDocument UIDoc) => {
            VisualElement root = UIDoc.rootVisualElement;
            if(root == null){
                Debug.Log("root wasn't found");
            }
            else{
                
                //references to the ui
                VisualElement startButton = root.Q<VisualElement>("Start");
                VisualElement continueButton = root.Q<VisualElement>("Continue");
                VisualElement optionsButton = root.Q<VisualElement>("Options");
                VisualElement creditsButton = root.Q<VisualElement>("Credits");
                VisualElement exitButton = root.Q<VisualElement>("exit");
                VisualElement titleBackground =root.Q<VisualElement>("title_background");
                VisualElement creditsBackground = root.Q<VisualElement>("credits_background");
                VisualElement fileSelectBackground = root.Q<VisualElement>("file_select");

                if(! (File.Exists(Application.persistentDataPath + "/save1" + "/SavePointData") || File.Exists(Application.persistentDataPath + "/save2" + "/SavePointData"))){
                    continueButton.RemoveFromClassList("not_selected");
                    continueButton.AddToClassList("unselectable");
                }
                switch(currentSelection){
                    case(titleMenuSelectables.Start):
                        if(input.goselected){
                            SetSingleton<OverworldAtmosphereData>(new OverworldAtmosphereData{songName = "CalmMountain"});
                            AudioManager.playSound("menuselect");
                                    StartNewGame?.Invoke(this, EventArgs.Empty);
                                    AudioManager.playSong("CalmMountain");
                                    InputGatheringSystem.currentInput = CurrentInput.overworld;
                                    sceneSystem.UnloadScene(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
                                    AudioManager.stopSong("menuMusic");
                                    isLinked = false;
                                    //Enabled = false;
                                }
                        /*else if(input.moveup){
                            AudioManager.playSound("menuchange");
                            currentSelection = titleMenuSelectables.Exit;

                            exitButton.RemoveFromClassList("not_selected");
                            exitButton.AddToClassList("selected");

                            startButton.RemoveFromClassList("selected");
                            startButton.AddToClassList("not_selected");

                        }*/
                        else if(input.movedown){
                            // only allows to select the continue button if there is a save
                            if(File.Exists(Application.persistentDataPath + "/save1" + "/SavePointData") || File.Exists(Application.persistentDataPath + "/save2" + "/SavePointData")){
                                AudioManager.playSound("menuchange");
                                currentSelection = titleMenuSelectables.Continue;

                                continueButton.RemoveFromClassList("not_selected");
                                continueButton.AddToClassList("selected");

                                startButton.RemoveFromClassList("selected");
                                startButton.AddToClassList("not_selected");
                            }
                            else{
                                AudioManager.playSound("menuchange");
                                currentSelection = titleMenuSelectables.Options;

                                optionsButton.RemoveFromClassList("not_selected");
                                optionsButton.AddToClassList("selected");

                                startButton.RemoveFromClassList("selected");
                                startButton.AddToClassList("not_selected");
                            }
                        }
                        break;
                    case(titleMenuSelectables.Continue):
                        if(isSelected){
                            if(input.goback){
                                AudioManager.playSound("menuback");
                                UnSelectFile(fileSelectBackground.Q<VisualElement>("save_file" + currentSaveFile.ToString()));
                                isSelected = false;
                                fileSelectBackground.visible = false;
                                titleBackground.visible =true;
                            }
                            else if(input.goselected){
                                StartGame?.Invoke(this, new StartGameEventArgs{saveFileNumber = currentSaveFile});
                                AudioManager.playSound("menuselect");
                                InputGatheringSystem.currentInput = CurrentInput.overworld;
                                sceneSystem.UnloadScene(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
                                AudioManager.stopSong("menuMusic");
                                isLinked = false;
                                isSelected = false;
                                currentSelection = titleMenuSelectables.Start;
                            }
                            else if((input.moveup && File.Exists(Application.persistentDataPath + "/save" + (currentSaveFile - 1).ToString() + "/SavePointData")) && currentSaveFile > 1){
                                AudioManager.playSound("menuchange");
                                UnSelectFile(fileSelectBackground.Q<VisualElement>("save_file" + currentSaveFile.ToString()));
                                currentSaveFile--;
                                SelectFile(fileSelectBackground.Q<VisualElement>("save_file" + currentSaveFile.ToString()));
                            }
                            else if((input.movedown && File.Exists(Application.persistentDataPath + "/save" + (currentSaveFile + 1).ToString() + "/SavePointData")) && currentSaveFile < 2){
                                AudioManager.playSound("menuchange");
                                UnSelectFile(fileSelectBackground.Q<VisualElement>("save_file" + currentSaveFile.ToString()));
                                currentSaveFile++;
                                SelectFile(fileSelectBackground.Q<VisualElement>("save_file" + currentSaveFile.ToString()));
                            }
                        }
                        else if(input.goselected){
                            AudioManager.playSound("menuselect");
                            UpdateSaveFileUI(fileSelectBackground);
                                
                            
                            fileSelectBackground.visible = true;
                            titleBackground.visible = false;
                            isSelected = true;
                        }
                        else if(input.moveup){
                            AudioManager.playSound("menuchange");
                            currentSelection = titleMenuSelectables.Start;

                            startButton.RemoveFromClassList("not_selected");
                            startButton.AddToClassList("selected");

                            continueButton.RemoveFromClassList("selected");
                            continueButton.AddToClassList("not_selected");
                        }
                        else if(input.movedown){
                            AudioManager.playSound("menuchange");
                            currentSelection = titleMenuSelectables.Options;

                            optionsButton.RemoveFromClassList("not_selected");
                            optionsButton.AddToClassList("selected");

                            continueButton.RemoveFromClassList("selected");
                            continueButton.AddToClassList("not_selected");
                        }
                    break;
                            case(titleMenuSelectables.Options):
                                if(isSelected){
                                    //do nothing, waiting for player to exit the settings
                                    if(!isInSettings){
                                        titleBackground.visible = true;
                                        isSelected = false;
                                        settingsMenuSystem.OnSettingsExit -= ReactivateTitle_OnSettingsExit;
                                    }
                                }
                                else if(input.goselected){
                                    AudioManager.playSound("menuselect");

                                    settingsMenuSystem.OnSettingsExit += ReactivateTitle_OnSettingsExit;

                                    titleBackground.visible = false;
                                    isInSettings = true;
                                    isSelected = true;
                                    settingsMenuSystem.ActivateMenu();
                                }
                                else if(input.moveup){
                                    if(File.Exists(Application.persistentDataPath + "/save1" + "/SavePointData") || File.Exists(Application.persistentDataPath + "/save1" + "/SavePointData")){
                                        AudioManager.playSound("menuchange");
                                        currentSelection = titleMenuSelectables.Continue;

                                        continueButton.RemoveFromClassList("not_selected");
                                        continueButton.AddToClassList("selected");

                                        optionsButton.RemoveFromClassList("selected");
                                        optionsButton.AddToClassList("not_selected");
                                    }
                                    else{
                                        AudioManager.playSound("menuchange");
                                        currentSelection = titleMenuSelectables.Start;

                                        startButton.RemoveFromClassList("not_selected");
                                        startButton.AddToClassList("selected");

                                        optionsButton.RemoveFromClassList("selected");
                                        optionsButton.AddToClassList("not_selected");
                                    }
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
                                if(isSelected){
                                    //make it so if the buttons are pressed they send to their according site
                                    if(!isLinked){
                                        Button technoYoutubeButton = root.Q<Button>("techno_youtube");
                                        Button technoTwitterButton = root.Q<Button>("techno_twitter");

                                        Button tommyYoutubeButton = root.Q<Button>("tommy_youtube");
                                        Button tommyTwitterButton = root.Q<Button>("tommy_twitter");
                                        Button tommyTwitchButton = root.Q<Button>("tommy_twitch");

                                        Button wilburYoutubeButton = root.Q<Button>("wilbur_youtube");
                                        Button wilburTwitterButton = root.Q<Button>("wilbur_twitter");
                                        Button wilburTwitchButton = root.Q<Button>("wilbur_twitch");
                                        technoYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTechno(websites.youtube));
                                        technoTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTechno(websites.twitter));

                                        tommyYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.youtube));
                                        tommyTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.twitter));
                                        tommyTwitchButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.twitch));

                                        wilburYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.youtube));
                                        wilburTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.twitter));
                                        wilburTwitchButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.twitch));

                                        isLinked = true;
                                    }
                                    if(input.goback){
                                        AudioManager.playSound("menuback");
                                        creditsBackground.visible = false;
                                        titleBackground.visible = true;
                                        isSelected = false;
                                    }
                                }
                                else if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    isSelected = true;
                                    creditsBackground.visible = true;
                                    titleBackground.visible = false;

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
                                    AudioManager.playSound("menuselect");
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
                                /*else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Start;

                                    startButton.RemoveFromClassList("not_selected");
                                    startButton.AddToClassList("selected");

                                    exitButton.RemoveFromClassList("selected");
                                    exitButton.AddToClassList("not_selected");
                                }*/
                                break;


                }
            }
            }).Run();
        }
    private void ReactivateTitle_OnSettingsExit(System.Object sender, System.EventArgs e){
        isInSettings = false;
    }
    public void SelectFile(VisualElement file){
        VisualElement background = file.Q<VisualElement>("background");
        background.RemoveFromClassList("file_not_selected");
        background.AddToClassList("file_selected");
    }
    public void UnSelectFile(VisualElement file){
        VisualElement background = file.Q<VisualElement>("background");
        background.RemoveFromClassList("file_selected");
        background.AddToClassList("file_not_selected");
    }
    public void UpdateSaveFileUI(VisualElement saveFileUI){
        bool selectedFile = false;
        for(int i = 1; i <= 2; i++){
            if(File.Exists(Application.persistentDataPath + "/save" + i.ToString() + "/SavePointData")){
                
                TemplateContainer fileContainer = saveFileUI.Q<TemplateContainer>("save_file" + i.ToString());
                VisualElement currentFile = fileContainer.Q<VisualElement>("background");

                Label currentTime = currentFile.Q<Label>("time");
                
                string savePath = Application.persistentDataPath + "/save" + i.ToString() + "/SavePointData";
                string jsonString = File.ReadAllText(savePath);
                SavePointData savePointData = JsonUtility.FromJson<SavePointData>(jsonString);

                float remainder = savePointData.timePassed;
                int hours = (int) remainder / 3600;
                remainder -= (hours * 3600);
                int minutes = (int) remainder / 60;
                remainder -= minutes * 60;
                int seconds = (int) remainder;
                                    
                currentTime.text = "Time: " + hours.ToString() + " : " + minutes.ToString() + " : " + seconds.ToString();

                Label location = currentFile.Q<Label>("location");
                location.text = savePointData.savePointName.ToString();

                if(!selectedFile){
                    currentSaveFile = i;
                    SelectFile(currentFile);
                    selectedFile = true;
                }
            }
        }
    }

}

public enum currentTitleMenu{
    MainMenu,
    OptionsMenu,
    Credits
}
public enum titleMenuSelectables{
    Start,
    Continue, 
    Options,
    Credits,
    Exit
}

public class StartGameEventArgs : EventArgs{
    public int saveFileNumber;
}
public delegate void StartGameEventHandler(object sender, StartGameEventArgs e);

public class temp : MonoBehaviour{
    public static void quitGame(){
        Application.Quit(0);
    }
}
