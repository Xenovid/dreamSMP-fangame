using Unity.Entities;
using UnityEngine.UIElements;
using Unity.Scenes;
//using Unity.Entities.Serialization;
using Unity.Serialization;
using UnityEngine.InputSystem;
using System;
using UnityEngine;

public class SettingsMenuSystem : SystemBase
{
    SceneSystem sceneSystem;

    VisualElement background;
    VisualElement volumeButton;
    VisualElement volumeControls;
    VisualElement otherButton;
    VisualElement otherControls;
    VisualElement bindingButton;
    VisualElement bindingControls;

    Controls controlSelection;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    protected override void OnStartRunning()
      {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
      }
    protected override void OnUpdate()
    {
        /*
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

        Entities
        .WithStructuralChanges()
        .WithoutBurst()
        .ForEach((UIDocument UIDoc, ref SettingsUIData settingsUIData) => {
            if(settingsUIData.isActive && !settingsUIData.isWaiting){
                VisualElement root = UIDoc.rootVisualElement;
                Slider volumeSlider = root.Q<Slider>("volume_slider");
                if(!settingsUIData.isSetUp){
                    volumeSlider.value = AudioManager.volume;
                    settingsUIData.isSetUp = true;
                }
                else if(!settingsUIData.isSelected){
                    if(input.goback){
                        AudioManager.playSound("menuback");
                        // go back to the previous menu
                        settingsUIData.isActive = false;
                        background.visible = false;
                        OnSettingsExit?.Invoke(this, System.EventArgs.Empty);
                        settingsUIData.currentTab = SettingsTab.volume;
                        UnSelectButton(bindingButton);
                        UnSelectButton(otherButton);
                        SelectButton(volumeButton);
                        SelectTab(volumeControls);
                        UnSelectTab(bindingControls);
                        UnSelectTab(otherControls);
                        volumeControls.visible = false;
                        otherControls.visible = false;
                        bindingControls.visible = false;
                    }
                    else{
                        switch(settingsUIData.currentTab){
                            case SettingsTab.volume:
                                if(input.goselected){
                                    AudioManager.playSound("menuselect");
                                    settingsUIData.isSelected = true;
                                }
                                else if(input.moveright){
                                    AudioManager.playSound("menuchange");
                                    settingsUIData.currentTab++;
                                    SelectButton(bindingButton);
                                    UnSelectButton(volumeButton);
                                    volumeControls.visible = false;
                                    bindingControls.visible = true;
                                    Label binding = bindingControls.Q<Label>("current_overworld_select");
                                    binding.text = InputControlPath.ToHumanReadableString(InputGatheringSystem.m_InputActions.Overworld.Select.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                                }
                            break;
                            case SettingsTab.controls:
                                if(input.goselected){
                                    AudioManager.playSound("menuselect");
                                    settingsUIData.isSelected = true;
                                    HoverBinding(bindingControls.Q<Label>("current_overworld_select"));
                                }
                                else if(input.moveright){
                                    AudioManager.playSound("menuchange");
                                    settingsUIData.currentTab++;
                                    SelectButton(otherButton);
                                    UnSelectButton(bindingButton);
                                    bindingControls.visible = false;
                                    otherControls.visible = true;
                                }
                                else if(input.moveleft){
                                    AudioManager.playSound("menuchange");
                                    settingsUIData.currentTab--;
                                    SelectButton(volumeButton);
                                    UnSelectButton(bindingButton);
                                    bindingControls.visible = false;
                                    volumeControls.visible = true;
                                }
                            break;
                            case SettingsTab.other:
                                if(input.goselected){
                                    if(settingsUIData.isOnTitleScreen){
                                        //since going to the title screen is the only option here currently, there is no reason to go into it while in the title screen
                                    }
                                    else{
                                        AudioManager.playSound("menuselect");
                                        SelectItem(otherControls.Q<Label>("return_text"));
                                        settingsUIData.isSelected = true;
                                    }
                                }
                                else if(input.moveleft){
                                    AudioManager.playSound("menuchange");
                                    settingsUIData.currentTab--;
                                    UnSelectButton(otherButton);
                                    SelectButton(bindingButton);
                                    bindingControls.visible = true;
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
                                    AudioManager.playSound("menuback");
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
                            case SettingsTab.controls:
                                if(input.goback){
                                    AudioManager.playSound("menuback");
                                    settingsUIData.isSelected = false;
                                    UnSelectBinding(GetBindingEquivalent(controlSelection));
                                    controlSelection = Controls.overSelect;
                                }
                                else if(input.goselected){
                                    AudioManager.playSound("menuselect");
                                    Rebind();
                                }
                                else if(input.movedown && controlSelection < Controls.uiLeft){
                                    AudioManager.playSound("menuchange");
                                    UnSelectBinding(GetBindingEquivalent(controlSelection));
                                    controlSelection++;
                                    HoverBinding(GetBindingEquivalent(controlSelection));
                                }
                                else if(input.moveup && controlSelection != Controls.overSelect){
                                    AudioManager.playSound("menuchange");
                                    UnSelectBinding(GetBindingEquivalent(controlSelection));
                                    controlSelection--;
                                    HoverBinding(GetBindingEquivalent(controlSelection));
                                }
                            break;
                            case SettingsTab.other:
                                if(input.goback){
                                    AudioManager.playSound("menuback");
                                    UnSelectItem(otherControls.Q<Label>("return_text"));
                                    settingsUIData.isSelected = false;
                                }
                                else if(input.goselected){
                                    AudioManager.playSound("menuselect");
                                    OnTitleReturn?.Invoke(this, EventArgs.Empty);
                                    sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
                                    sceneSystem.UnloadScene(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
                                    sceneSystem.UnloadScene(SubSceneReferences.Instance.EssentialsSubScene.SceneGUID);
                                    AudioManager.playSong("menuMusic");
                                }
                            break;
                        }
                    
                }
                
            }
        }).Run();
        */
    }
    private Label GetBindingEquivalent(Controls control){
        string uiName = "";
        switch(control){
            case Controls.overSelect:
                uiName = "current_overworld_select";
            break;
            case Controls.overPause:
                uiName = "current_overworld_pause";
            break;
            case Controls.overUp:
                uiName = "current_overworld_up";
            break;
            case Controls.overRight:
                uiName = "current_overworld_right";
            break;
            case Controls.overLeft:
                uiName = "current_overworld_left";
            break;
            case Controls.overDown:
                uiName = "current_overworld_down";
            break;
            case Controls.uiSelect:
                uiName = "current_ui_select";
            break;
            case Controls.uiBack:
                uiName = "current_ui_back";
            break;
            case Controls.uiUp:
                uiName = "current_ui_up";
            break;
            case Controls.uiRight:
                uiName = "current_ui_right";
            break;
            case Controls.uiLeft:
                uiName = "current_ui_left";
            break;
            case Controls.uiDown:
                uiName = "current_ui_down";
            break;
        }
        return bindingControls.Q<Label>(uiName);
    }
    private InputAction GetInputMapEquivalent(Controls control){
        InputAction returnAction = new InputAction();
        switch(control){
            case Controls.overSelect:
                returnAction = InputGatheringSystem.m_InputActions.Overworld.Select;
            break;
            case Controls.overPause:
                returnAction = InputGatheringSystem.m_InputActions.Overworld.Pause;
            break;
            case Controls.overUp:
                returnAction = InputGatheringSystem.m_InputActions.Overworld.Move;
            break;
            case Controls.overRight:
                returnAction = InputGatheringSystem.m_InputActions.Overworld.Move;
            break;
            case Controls.overLeft:
                returnAction = InputGatheringSystem.m_InputActions.Overworld.Move;
            break;
            case Controls.overDown:
                returnAction = InputGatheringSystem.m_InputActions.Overworld.Move;
            break;
            case Controls.uiSelect:
                returnAction = InputGatheringSystem.m_InputActions.UI.UISelect;
            break;
            case Controls.uiBack:
                returnAction = InputGatheringSystem.m_InputActions.UI.UIBack;
            break;
            case Controls.uiUp:
                returnAction = InputGatheringSystem.m_InputActions.UI.UIMove;
            break;
            case Controls.uiRight:
                returnAction = InputGatheringSystem.m_InputActions.UI.UIMove;
            break;
            case Controls.uiLeft:
                returnAction = InputGatheringSystem.m_InputActions.UI.UIMove;
            break;
            case Controls.uiDown:
                returnAction = InputGatheringSystem.m_InputActions.UI.UIMove;
            break;
        }
        return returnAction;
    }
    private int GetBindingIndex(InputAction inputAction, Controls control){
        var bindingIndex = 0;
        if(inputAction == InputGatheringSystem.m_InputActions.UI.UIMove || inputAction == InputGatheringSystem.m_InputActions.Overworld.Move){
            
            // gets the binding index of the current 
            switch(control){
                case Controls.uiUp:
                    bindingIndex = inputAction.bindings.IndexOf(x => x.isPartOfComposite && x.name == "up");
                break;
                case Controls.overUp:
                    bindingIndex = inputAction.bindings.IndexOf(x => x.isPartOfComposite && x.name == "up");
                break;
                case Controls.uiDown:
                    bindingIndex = inputAction.bindings.IndexOf(x => x.isPartOfComposite && x.name == "down");
                break;
                case Controls.overDown:
                    bindingIndex = inputAction.bindings.IndexOf(x => x.isPartOfComposite && x.name == "down");
                break;
                case Controls.uiRight:
                    bindingIndex = inputAction.bindings.IndexOf(x => x.isPartOfComposite && x.name == "right");
                break;
                case Controls.overRight:
                    bindingIndex = inputAction.bindings.IndexOf(x => x.isPartOfComposite && x.name == "right");
                break;
                case Controls.uiLeft:
                    bindingIndex = inputAction.bindings.IndexOf(x => x.isPartOfComposite && x.name == "left");
                break;
                case Controls.overLeft:
                    bindingIndex = inputAction.bindings.IndexOf(x => x.isPartOfComposite && x.name == "left");
                break;
            }
        }
        return bindingIndex;    
    }
    private void Rebind(){
        InputGatheringSystem.m_InputActions.Overworld.Disable();
        InputGatheringSystem.m_InputActions.UI.Disable();

        SelectBinding(GetBindingEquivalent(controlSelection));
        InputAction selectedInputMap = GetInputMapEquivalent(controlSelection);
        var bindingIndex = GetBindingIndex(selectedInputMap, controlSelection);
        if(selectedInputMap == InputGatheringSystem.m_InputActions.UI.UIMove || selectedInputMap == InputGatheringSystem.m_InputActions.Overworld.Move){
            rebindingOperation = selectedInputMap.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            //.WithTargetBinding(bindingIndex)
            //.WithControlsExcluding(InputGatheringSystem.m_InputActions.UI.UISelect.bindings[0].effectivePath)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(callback => RebindComplete())
            .Start();
        }
        else{
            rebindingOperation = selectedInputMap.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            //.WithControlsExcluding(InputGatheringSystem.m_InputActions.UI.UISelect.bindings[0].effectivePath)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(callback => RebindComplete())
            .Start();
        }
        
    }
    private void UpdateBindings(){
        foreach(Controls control in Enum.GetValues(typeof(Controls))){
            Label binding = GetBindingEquivalent(control);
            InputAction inputAction = GetInputMapEquivalent(control);
            var bindingIndex = GetBindingIndex(inputAction, control);
            binding.text = InputControlPath.ToHumanReadableString(inputAction.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }
    private void RebindComplete(){
        Label binding = GetBindingEquivalent(controlSelection);
        var inputAction = GetInputMapEquivalent(controlSelection);
        var bindingIndex = GetBindingIndex(inputAction, controlSelection);
        binding.text = InputControlPath.ToHumanReadableString(inputAction.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        rebindingOperation.Dispose();

        HoverBinding(binding);
        InputGatheringSystem.m_InputActions.Overworld.Enable();
        InputGatheringSystem.m_InputActions.UI.Enable();
        AudioManager.playSound("menuavailable");
    }
    private void HoverBinding(Label binding){
        binding.AddToClassList("binding_hover");
        binding.RemoveFromClassList("binding_unselected");
        binding.RemoveFromClassList("binding_selected");
    }
    private void UnSelectBinding(Label binding){
        binding.RemoveFromClassList("binding_hover");
        binding.AddToClassList("binding_unselected");
    }
    private void SelectBinding(Label binding){
        binding.RemoveFromClassList("binding_hover");
        binding.AddToClassList("binding_selected");
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

                bindingButton = root.Q<VisualElement>("bindings_button");
                bindingControls = root.Q<VisualElement>("bindings_controls");
                root.Q<Slider>("volume_slider").SetEnabled(false);
                UpdateBindings();
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
        controls,
        other
        }

public enum Controls{
    overSelect,
    overPause,
    overUp,
    overDown,
    overRight,
    overLeft,
    uiSelect,
    uiBack,
    uiUp,
    uiDown,
    uiRight,
    uiLeft


}

