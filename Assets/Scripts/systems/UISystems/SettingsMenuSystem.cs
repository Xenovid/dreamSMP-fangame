using Unity.Entities;
using UnityEngine.UIElements;
using Unity.Scenes;
using System;
using UnityEngine;

public class SettingsMenuSystem : SystemBase
{
    SceneSystem sceneSystem;
    public event EventHandler OnSettingsExit;
    public event EventHandler OnTitleReturn;

    VisualElement background;
    VisualElement volumeButton;
    VisualElement volumeControls;
    VisualElement otherButton;
    VisualElement otherControls;
    protected override void OnStartRunning()
      {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
      }
    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();


        Entities
        .WithStructuralChanges()
        .WithoutBurst()
        .ForEach((UIDocument UIDoc, ref SettingsUIData settingsUIData) => {
            if(settingsUIData.isActive){
                VisualElement root = UIDoc.rootVisualElement;
                Slider volumeSlider = root.Q<Slider>("volume_slider");
                if(!settingsUIData.isSetUp){
                    volumeSlider.value = AudioManager.volume;
                    settingsUIData.isSetUp = true;
                }
                else if(!settingsUIData.isSelected){
                    if(input.goback){
                        // go back to the previous menu
                        settingsUIData.isActive = false;
                        background.visible = false;
                        OnSettingsExit?.Invoke(this, System.EventArgs.Empty);
                        volumeControls.visible = false;
                        otherControls.visible = false;
                    }
                    else{
                        switch(settingsUIData.currentTab){
                            case SettingsTab.volume:
                                if(input.goselected){
                                    settingsUIData.isSelected = true;
                                }
                                else if(input.moveright){
                                    settingsUIData.currentTab++;
                                    SelectButton(otherButton);
                                    UnSelectButton(volumeButton);
                                    Debug.Log("other button should be selected");
                                    volumeControls.visible = false;
                                    otherControls.visible = true;
                                }
                            break;
                            case SettingsTab.other:
                                if(input.goselected){
                                    if(settingsUIData.isOnTitleScreen){
                                        //since going to the title screen is the only option here currently, there is no reason to go into it while in the title screen
                                    }
                                    else{
                                        SelectItem(otherControls.Q<Label>("return_text"));
                                        settingsUIData.isSelected = true;
                                    }
                                }
                                else if(input.moveleft){
                                    settingsUIData.currentTab--;
                                    UnSelectButton(otherButton);
                                    SelectButton(volumeButton);
                                    volumeControls.visible = true;
                                    otherControls.visible = false;
                                }
                            break;
                    }
                    }
                        
                }
                else{
                        switch(settingsUIData.currentTab){
                            case SettingsTab.volume:
                                if(input.goback){
                                    settingsUIData.isSelected = false;
                                }
                                else if(input.moveright){
                                    AudioManager.playSound("menuchange");
                                    if(volumeSlider.value + .1 < volumeSlider.highValue){
                                        volumeSlider.value = volumeSlider.value + .1f;
                                    }
                                    else{
                                        volumeSlider.value = volumeSlider.highValue;
                                    }
                                }
                                else if(input.moveleft){
                                    AudioManager.playSound("menuchange");
                                    if(volumeSlider.value - .1 > volumeSlider.lowValue){
                                        volumeSlider.value -= .1f;
                                    }
                                    else{
                                        volumeSlider.value = volumeSlider.lowValue;
                                    }
                                }
                                AudioManager.changeVolume(volumeSlider.value); 
                            break;
                            case SettingsTab.other:
                                if(input.goback){
                                    UnSelectItem(otherControls.Q<Label>("return_text"));
                                    settingsUIData.isSelected = false;
                                }
                                else if(input.goselected){
                                    OnTitleReturn?.Invoke(this, EventArgs.Empty);
                                    sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
                                    sceneSystem.UnloadScene(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
                                    AudioManager.playSong("menuMusic");
                                }
                            break;
                        }
                    
                }
                
            }
        }).Run();
        
    }
    private void SelectButton(VisualElement button){
        button.RemoveFromClassList("option_unselected");
        button.AddToClassList("option_selected");
    }
    private void UnSelectButton(VisualElement button){
        button.RemoveFromClassList("option_selected");
        button.AddToClassList("option_unselected");
    }
    private void SelectTab(VisualElement tab){
    }
    private void UnSelectTab(VisualElement tab){

    }
    private void SelectItem(Label item){
        item.RemoveFromClassList("item_unselected");
        item.RemoveFromClassList("item_selected");
    }
    private void UnSelectItem(Label item){
        item.RemoveFromClassList("item_selected");
        item.RemoveFromClassList("item_unselected");
    }
    
    public void ActivateMenu(){
        //initializing all the ui references
        Entities
        .WithoutBurst()
        .ForEach((UIDocument UIDoc, ref SettingsUIData settingsUIData) => {
            if(!settingsUIData.isSetUp){
                
                VisualElement root = UIDoc.rootVisualElement;
                background = root.Q<VisualElement>("settings_background");
                volumeButton = root.Q<VisualElement>("volume_button");
                volumeControls =root.Q<VisualElement>("volume_controls");
                
                otherButton = root.Q<VisualElement>("other");
                otherControls = root.Q<VisualElement>("other_controls");
                root.Q<Slider>("volume_slider").SetEnabled(false);
            }
            settingsUIData.isSelected = false;
            volumeControls.visible = true;
            background.visible = true;
            settingsUIData.isActive = true;
        }).Run();
    }
}
public enum SettingsTab{
        volume,
        other
        }

