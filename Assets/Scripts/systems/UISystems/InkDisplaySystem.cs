using Unity.Entities;
using UnityEngine.UIElements;
using Ink.Runtime;
using System;
using System.Linq;
using UnityEngine;
using Unity.Collections;

public class InkDisplaySystem : SystemBase
{
    PauseSystem pauseSystem;
    UISystem uISystem;
    //public event EventHandler OnCutsceneFinish;
    public event EventHandler OnVictoryDisplayFinish;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    EntityQuery characterBaseAnimationQuery;
    EntityQuery characterEyeAnimationQuery;
    EntityQuery characterMouthAnimationQuery;
    EntityQuery messageBoardQuery;
    BattleSystem battleSystem;
    EntityQuery inkQuery;
    // prevents glitch were you can continue story without making a choice
    public bool DisplayingChoices;
    protected override void OnCreate()
    {
        pauseSystem = World.GetOrCreateSystem<PauseSystem>();
        uISystem = World.GetOrCreateSystem<UISystem>();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        characterBaseAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterBaseTag));
        characterEyeAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterEyeTag));
        characterMouthAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterMouthTag));
        messageBoardQuery = GetEntityQuery(typeof(UITag));
        inkQuery = GetEntityQuery(typeof(InkManagerData));
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        Button textBoxUI = uISystem.textBoxUI;
        VisualElement overworldOverlay = uISystem.overworldOverlay;
        var DeltaTime = Time.DeltaTime;

        if(characterBaseAnimationQuery.CalculateEntityCount() == 0 && textBoxUI != null){
            Entity characterBaseAnimationEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<CharacterBaseTag>(characterBaseAnimationEntity);
            EntityManager.AddComponentObject(characterBaseAnimationEntity, new UIAnimationData{
                visualElement = textBoxUI.Q<VisualElement>("character_base"),
                spritePerSecond = .2f,
                active = false
            });
        }
        if(characterEyeAnimationQuery.CalculateEntityCount() == 0 && textBoxUI != null){
            Entity characterEyeAnimationEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<CharacterEyeTag>(characterEyeAnimationEntity);
            EntityManager.AddComponentObject(characterEyeAnimationEntity, new UIAnimationData{
                visualElement = textBoxUI.Q<VisualElement>("character_eyes"),
                spritePerSecond = .1f,
                initialDelay = 5f,
                active = false
            });
        }
        if(characterMouthAnimationQuery.CalculateEntityCount() == 0 && textBoxUI != null){
            Entity characterMouthAnimationEntity = EntityManager.CreateEntity( EntityManager.CreateArchetype(typeof(UIAnimationData), typeof(CharacterMouthTag)));
            UIAnimationData uIAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterMouthAnimationEntity);
            uIAnimation = new UIAnimationData();
            uIAnimation.visualElement = textBoxUI.Q<VisualElement>("character_mouth");
            uIAnimation.active = false;
            uIAnimation.spritePerSecond = .2f;
            EntityManager.SetComponentData(characterMouthAnimationEntity, uIAnimation);
        }

        Entity characterMouthEntity = characterMouthAnimationQuery.GetSingletonEntity();

        UIAnimationData characterMouthAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterMouthEntity);

        
        UIInputData input = GetSingleton<UIInputData>();
        if(inkQuery.CalculateEntityCount() > 0){
            Entity inkEntity = inkQuery.GetSingletonEntity();
            
            
            InkManagerData inkManager = EntityManager.GetComponentObject<InkManagerData>(inkEntity);
            if(inkManager.inkStory == null || !inkManager.instantiated){
                inkManager.instantiated = true;
                EntityManager.SetComponentData(inkEntity, inkManager);
                // setting up story
                inkManager.inkStory = new Story(inkManager.inkAssest.text);
                inkManager.inkStory.BindExternalFunction("playSong", (string name) => {
                    AudioManager.playSong(name);
                });
                inkManager.inkStory.BindExternalFunction("playSound", (string name) => {
                    AudioManager.playSound(name);
                });
                inkManager.inkStory.BindExternalFunction("displayPortrait", (string name) => {
                    DisplayPortriat(name);
                });
                inkManager.inkStory.BindExternalFunction("setTextSound", (string name) => {
                    SetDialogueSound(name);
                });
            }
            
        }

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((ref Text text,  ref TextBoxData textBoxData) =>{ 
            if(textBoxData.isDisplaying){
                Label textBoxText = textBoxUI.Q<Label>("TextBoxText");
                // ask ink to do the next thing for the story
                if(input.goselected && textBoxData.isFinishedPage){
                    uISystem.ResetFocus();
                    ContinueStory();
                }
                else if(input.goselected && !text.unSkipable){
                    characterMouthAnimation.active = false;
                    textBoxData.currentChar = text.text.Length - 1;
                    textBoxData.isFinishedPage = true;
                    textBoxText.text = text.text.ToString();
                    characterMouthAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterMouthAnimation.sprites[0]);
                }
                else if(!textBoxData.isFinishedPage){
                    textBoxData.timeFromLastChar += DeltaTime * text.textSpeed;
                    while (textBoxData.timeFromLastChar >= textBoxData.textSpeed && !textBoxData.isFinishedPage){
                        string textString = text.text.ToString();
                        textBoxData.currentChar++;
                        if(textString[textBoxData.currentChar -1] != ' '){
                            AudioManager.playDialogue(text.dialogueSoundName.ToString());
                        }
                        
                        
                        if(textBoxData.currentChar >= textString.Length){
                            textBoxData.isFinishedPage = true;
                            characterMouthAnimation.active = false;
                            if(characterMouthAnimation.sprites != null){
                                characterMouthAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterMouthAnimation.sprites[0]);
                            }
                        }
                        else{
                            textBoxText.text = textString.Substring(0,textBoxData.currentChar);
                        }
                        // not setting it to 0 since it'll get rid extra time
                        textBoxData.timeFromLastChar -= textBoxData.textSpeed;
                    }
                }
            }
        }).Run();


        EntityManager.SetComponentData(characterMouthEntity, characterMouthAnimation);
    }
    public void SetDialogueSound(string name){
        Entity messageBoard =  messageBoardQuery.GetSingletonEntity();
        Text text = GetComponent<Text>(messageBoard);
        text.dialogueSoundName = name;
        EntityManager.SetComponentData(messageBoard, text);
    }
    public void UpdateTextBox(){
        
        Button textBoxUI = uISystem.textBoxUI;
        VisualElement overworldOverlay = uISystem.overworldOverlay;
        Label textBoxText = textBoxUI.Q<Label>("TextBoxText");

        Entity characterBaseEntity = characterBaseAnimationQuery.GetSingletonEntity();
        Entity characterEyesEntity = characterEyeAnimationQuery.GetSingletonEntity();
        Entity characterMouthEntity = characterMouthAnimationQuery.GetSingletonEntity();

        UIAnimationData characterBaseAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterBaseEntity);
        UIAnimationData characterEyeAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterEyesEntity);
        UIAnimationData characterMouthAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterMouthEntity);

        UIInputData input = GetSingleton<UIInputData>();
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((  ref TextBoxData textBoxData, in Text text) =>{ 
            if(text.isEnabled && !textBoxData.isDisplaying){
                pauseSystem.Pause();
                overworldOverlay.visible = false;
                textBoxUI.visible = true;
                textBoxUI.Focus();
                characterMouthAnimation.active = true;
                //ensures that there isn't any buttons blocking the view
                VisualElement playerChoiceUI = textBoxUI.Q<VisualElement>("player_choices");
                playerChoiceUI.Clear();
                

                InputGatheringSystem.currentInput = CurrentInput.ui;
                Label textBoxText = textBoxUI.Q<Label>("TextBoxText");
                
                textBoxData.timeFromLastChar = 0f;
                textBoxData.currentChar = 0;
                if(text.instant){
                    textBoxText.text = text.text.ToString();
                    textBoxData.isFinishedPage = true;
                }
                else{
                    textBoxData.isFinishedPage = false;
                }
                //DisplayPortriat(text.dialoguePortraitName.ToString());
                textBoxData.isDisplaying = true;
            }
            else{
                textBoxData.timeFromLastChar = 0f;
                    textBoxData.currentChar = 0;
                    if(text.instant){
                        characterMouthAnimation.active = false;
                        textBoxText.text = text.text.ToString();
                        textBoxData.isFinishedPage = true;
                    }
                    else{
                        characterMouthAnimation.active = true;
                        textBoxData.isFinishedPage = false;
                    }
                    
            }
        }).Run();
        EntityManager.SetComponentData(characterMouthEntity, characterMouthAnimation);
    }
    public void DisplayVictoryData(){
        DisplayPortriat("default");
        Entity messageBoard = GetSingletonEntity<UITag>();
        Text text = GetComponent<Text>(messageBoard);
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((InkManagerData inkManager) => {
            inkManager.inkStory.SwitchFlow("victory");
            inkManager.inkStory.ChoosePathString("victory");
            ContinueStory();
            
        }).Run();
    }
    public void StartCutScene(String startPoint){
        DisplayingChoices = false;
        InputGatheringSystem.currentInput = CurrentInput.ui;
        uISystem.overworldOverlay.visible = false;
        Entity messageBoard = GetSingletonEntity<UITag>();
        Text text = GetComponent<Text>(messageBoard);
        Entities
        .WithoutBurst()
        .ForEach((InkManagerData inkManager) => {
            inkManager.inkStory.ChoosePathString(startPoint);
        }).Run();
        ContinueStory();
    }
    public void ContinueStory(){
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((InkManagerData inkManager) => {
            if(DisplayingChoices){
                uISystem.textBoxUI.focusable = false;
            }
            else{
                uISystem.textBoxUI.focusable = true;
            }
            if(inkManager.inkStory.canContinue && !DisplayingChoices){
                inkManager.inkStory.Continue();
                if(inkManager.inkStory.currentTags.Contains("battle")){
                    //initiate battle
                    DisableTextboxUI();
                    //AudioManager.playSong("tempBattleMusic");
                    EntityQuery cutsceneBattleQuery = GetEntityQuery(typeof(CutsceneBattleDataTag));
                    NativeArray<CutsceneBattleDataTag> battleDataTags = cutsceneBattleQuery.ToComponentDataArray<CutsceneBattleDataTag>(Allocator.Temp);
                    NativeArray<Entity> battleDataEntities = cutsceneBattleQuery.ToEntityArray(Allocator.Temp);
                    
                    string battleName = new string(inkManager.inkStory.currentText.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                    for(int i = 0; i < battleDataTags.Length; i++){
                        if(battleDataTags[i].name == battleName){
                            battleSystem.StartBattle(battleDataEntities[i]);
                            inkManager.inkStory.SwitchFlow("battle");
                            break;
                        } 
                    }
                    battleDataTags.Dispose();
                    battleDataEntities.Dispose();
                }
                else if(inkManager.inkStory.currentTags.Contains("playable")){
                    // initiate playable
                    pauseSystem.Pause();
                    DisableTextboxUI();
                    uISystem.overworldOverlay.visible = false;

                    EntityQuery playableQuery = GetEntityQuery(typeof(PlayableData));
                    NativeArray<Entity> playableEntities = playableQuery.ToEntityArray(Allocator.Temp);

                    string playableName = new string(inkManager.inkStory.currentText.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                    foreach(Entity entity in playableEntities){
                        PlayableData playableData = EntityManager.GetComponentObject<PlayableData>(entity);

                        if(playableData.name == playableName){
                            CameraData cameraData = GetSingleton<CameraData>();
                            cameraData.currentState = CameraState.FreeForm;
                            SetSingleton(cameraData);
                            
                            EntityPlayableManager.instance.PlayPlayable(playableData.index);
                        }
                    }

                    playableEntities.Dispose();
                }
                else{
                    Entity messageBoard = GetSingletonEntity<UITag>();
                    Text text = GetComponent<Text>(messageBoard);
                    text.text = inkManager.inkStory.currentText;
                    text.isEnabled = true;
                    text.textSpeed = 1f;
                    // text is meant to be a sound to be played
                    text.textSpeed = 1f;
                    if(inkManager.inkStory.currentTags.Contains("unskipable")){
                        text.unSkipable = true;
                    }
                    else{
                        text.unSkipable = false;
                    }
                    if(inkManager.inkStory.currentTags.Contains("slow")){
                        text.textSpeed = .2f;
                    }
                    if(inkManager.inkStory.currentTags.Contains("instant")){
                        text.instant = true;
                    }
                    else{
                        text.instant = false;
                    }
                    SetComponent(messageBoard, text);
                    UpdateTextBox();
                    
                }
            }
            else if(!DisplayingChoices){
                // story segment done
                InputGatheringSystem.currentInput = CurrentInput.overworld;
                if(inkManager.inkStory.currentFlowName == "victory"){
                    OnVictoryDisplayFinish?.Invoke(this, EventArgs.Empty);
                    inkManager.inkStory.SwitchToDefaultFlow();
                }
                if(inkManager.inkStory.currentFlowName != "battle"){
                    CameraData cameraData = GetSingleton<CameraData>();
                    cameraData.currentState = CameraState.FollingPlayer;
                    SetSingleton(cameraData);
                    ResetTextBox();
                }
                
                DisableTextboxUI();
            }    
        }).Run();
    }
    public void DisableTextboxUI(){
        pauseSystem.UnPause();
        uISystem.textBoxUI.visible = false;
        uISystem.overworldOverlay.visible = true;
        Entities
        .WithoutBurst()
        .ForEach((  ref TextBoxData textBoxData, ref Text text) =>{ 
            textBoxData.isDisplaying = false;
            textBoxData.isFinishedPage = false;
            text.isEnabled = false;
        }).Run();
    }
    public void DisplayPortriat(string dialoguePortraitName){
        Entity characterBaseEntity = characterBaseAnimationQuery.GetSingletonEntity();
        Entity characterEyesEntity = characterEyeAnimationQuery.GetSingletonEntity();
        Entity characterMouthEntity = characterMouthAnimationQuery.GetSingletonEntity();

        UIAnimationData characterBaseAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterBaseEntity);
        UIAnimationData characterEyeAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterEyesEntity);
        UIAnimationData characterMouthAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterMouthEntity);

        CharacterPortraitData characterPortraitData = GetPortriatList(dialoguePortraitName);
                    // updating the portrait
        characterBaseAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterPortraitData.portraits[0]);
        characterEyeAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterPortraitData.eyeAnimations[0]);
        characterMouthAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterPortraitData.mouthAnimations[0]);

        characterBaseAnimation.sprites = characterPortraitData.portraits;
        characterEyeAnimation.sprites = characterPortraitData.eyeAnimations;
        characterMouthAnimation.sprites = characterPortraitData.mouthAnimations;

        characterBaseAnimation.active = characterBaseAnimation.sprites.Length > 1;
        characterEyeAnimation.active = characterEyeAnimation.sprites.Length > 1;
        characterMouthAnimation.active = characterMouthAnimation.sprites.Length > 1;
    }
    public CharacterPortraitData GetPortriatList(string name){
        if(name == "") name = "default";
        Entity characterPortriatHolderEntity = GetSingletonEntity<CharacterPortraitHolderTag>();
        CharacterPortraitListData allPortraitsList = EntityManager.GetComponentObject<CharacterPortraitListData>(characterPortriatHolderEntity);

        foreach(CharacterPortraitData characterPortraitData in allPortraitsList.characterPortraitList){
            if(characterPortraitData.name == name){
                return characterPortraitData;
            }
        }
        Debug.Log("name not found");
        if(name != "default"){
            return GetPortriatList(default);
        }
        Debug.Log("defualt not found");
        return new CharacterPortraitData();
        
    }
    public void ContinueText(){
        VisualElement textBoxUI = uISystem.textBoxUI;
        VisualElement overworldOverlay = uISystem.overworldOverlay;

        Entity characterMouthEntity = characterMouthAnimationQuery.GetSingletonEntity();

        UIAnimationData characterMouthAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterMouthEntity);

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((ref TextBoxData textBoxData, in Text text) =>{
            Label textBoxText = textBoxUI.Q<Label>("TextBoxText");
            VisualElement characterImage = textBoxUI.Q<VisualElement>("CharacterImage");
            if(textBoxData.isFinishedPage){
                uISystem.ResetFocus();
                ContinueStory();
            }
            else if(!text.unSkipable){
                textBoxData.currentChar = text.text.Length - 1;
                textBoxData.isFinishedPage = true;
                characterMouthAnimation.active = false;
                if(characterMouthAnimation.sprites != null){
                    characterMouthAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterMouthAnimation.sprites[0]);
                }
                textBoxText.text = text.text.ToString();
            }
        }).Run();
        EntityManager.SetComponentData(characterMouthEntity, characterMouthAnimation);
    }
    public void DisplayChoices(Button[] choices){
        DisplayingChoices = true;
        ResetTextBox();
        pauseSystem.Pause();
        Entity characterBaseEntity = characterBaseAnimationQuery.GetSingletonEntity();
        Entity characterEyesEntity = characterEyeAnimationQuery.GetSingletonEntity();
        Entity characterMouthEntity = characterMouthAnimationQuery.GetSingletonEntity();

        UIAnimationData characterBaseAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterBaseEntity);
        UIAnimationData characterEyeAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterEyesEntity);
        UIAnimationData characterMouthAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterMouthEntity);

        CharacterPortraitData characterPortraitData = GetPortriatList("technoblade");
        // updating the portrait
        characterBaseAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterPortraitData.portraits[0]);
        characterEyeAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterPortraitData.eyeAnimations[0]);
        characterMouthAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterPortraitData.mouthAnimations[0]);

        characterBaseAnimation.sprites = characterPortraitData.portraits;
        characterEyeAnimation.sprites = characterPortraitData.eyeAnimations;
        characterMouthAnimation.sprites = characterPortraitData.mouthAnimations;

        characterBaseAnimation.active = characterBaseAnimation.sprites.Length > 1;
        characterEyeAnimation.active = characterEyeAnimation.sprites.Length > 1;
        characterMouthAnimation.active = false;

        Button textBoxUI = uISystem.textBoxUI;
        textBoxUI.visible = true;
        InputGatheringSystem.currentInput = CurrentInput.ui;
        VisualElement playerChoiceUI = textBoxUI.Q<VisualElement>("player_choices");
        playerChoiceUI.Clear();
        foreach(Button choice in choices){
            choice.focusable = true;
            choice.Focus();
            playerChoiceUI.Add(choice);
        }
        Entities
        .WithoutBurst()
        .ForEach((  ref TextBoxData textBoxData, ref Text text) =>{ 
            text.isEnabled = false;
        }).Run();

        EntityManager.SetComponentData(characterBaseEntity, characterBaseAnimation);
        EntityManager.SetComponentData(characterEyesEntity, characterEyeAnimation);
        EntityManager.SetComponentData(characterMouthEntity, characterMouthAnimation);
    }
    public void ResetTextBox(){
        Entities
        .WithoutBurst()
        .ForEach((  ref TextBoxData textBoxData, ref Text text) =>{ 
            text.dialogueSoundName = "default";
            Label textBoxText = uISystem.textBoxUI.Q<Label>("TextBoxText");
            textBoxText.text = "";
            DisplayPortriat("default");
        }).Run();
    }
}
