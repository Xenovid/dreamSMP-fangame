using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;

public class TextBoxSystem : SystemBase
{
    public event EventHandler OnDisplayFinished;
    public bool isDisplaying;
    UISystem uISystem;
    EntityQuery characterBaseAnimationQuery;
    EntityQuery characterEyeAnimationQuery;
    EntityQuery characterMouthAnimationQuery;

    protected override void OnStartRunning()
    {
        characterBaseAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterBaseTag));
        characterEyeAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterEyeTag));
        characterMouthAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterMouthTag));

        TextBoxData text = GetSingleton<TextBoxData>();
        text.textSpeed = text.textSpeed == 0 ? .05f : text.textSpeed;
        SetSingleton<TextBoxData>(text);
        uISystem = World.GetOrCreateSystem<UISystem>();
    }
    protected override void OnUpdate()
    {
        float DeltaTime = Time.DeltaTime;
        Button textBoxUI = uISystem.textBoxUI;
        VisualElement overworldOverlay = uISystem.overworldOverlay;
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

        Entity characterBaseEntity = characterBaseAnimationQuery.GetSingletonEntity();
        Entity characterEyesEntity = characterEyeAnimationQuery.GetSingletonEntity();
        Entity characterMouthEntity = characterMouthAnimationQuery.GetSingletonEntity();

        UIAnimationData characterBaseAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterBaseEntity);
        UIAnimationData characterEyeAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterEyesEntity);
        UIAnimationData characterMouthAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterMouthEntity);

        UIInputData input = GetSingleton<UIInputData>();
        Entities
        .WithoutBurst()
        .ForEach((DynamicBuffer<Text> texts,  ref TextBoxData textBoxData) =>{ 
                if(!texts.IsEmpty && !isDisplaying){
                    overworldOverlay.visible = false;
                    textBoxUI.visible = true;
                    textBoxUI.Focus();

                    //ensures that there isn't any buttons blocking the view
                    VisualElement playerChoiceUI = textBoxUI.Q<VisualElement>("player_choices");
                    playerChoiceUI.Clear();
                    

                    InputGatheringSystem.currentInput = CurrentInput.ui;
                    Label textBoxText = textBoxUI.Q<Label>("TextBoxText");
                    
                    textBoxData.timeFromLastChar = 0f;
                    textBoxData.currentChar = 0;
                    if(texts[0].instant){
                        textBoxText.text = texts[0].text.ToString();
                        textBoxData.isFinishedPage = true;
                    }
                    else{
                        textBoxData.currentSentence = texts[0].text.ToString();
                        textBoxData.isFinishedPage = false;
                    }
                    CharacterPortraitData characterPortraitData = GetPortriatList(texts[0].dialoguePortraitName.ToString());
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
                    isDisplaying = true;
                }
                if(isDisplaying){
                    Label textBoxText = textBoxUI.Q<Label>("TextBoxText");
                    VisualElement characterImage = textBoxUI.Q<VisualElement>("CharacterImage");
                    if(input.goselected && textBoxData.isFinishedPage){
                        if(texts.Length == 1){
                            uISystem.root.Focus();
                            uISystem.ResetFocus();
                            texts.RemoveAt(0);
                            isDisplaying = false;
                            
                            textBoxUI.visible = false;
                            overworldOverlay.visible = true;
                            textBoxText.text = "";
                            InputGatheringSystem.currentInput = CurrentInput.overworld;
                            OnDisplayFinished?.Invoke(this, EventArgs.Empty);
                        }
                        else{
                            texts.RemoveAt(0);
                            textBoxData.timeFromLastChar = 0f;
                            textBoxData.currentChar = 0;
                            if(texts[0].instant){
                                textBoxText.text = texts[0].text.ToString();
                                textBoxData.isFinishedPage = true;
                            }
                            else{
                                textBoxData.currentSentence = texts[0].text.ToString();
                                textBoxData.isFinishedPage = false;
                            }
                            // updating the portrait
                            CharacterPortraitData characterPortraitData = GetPortriatList(texts[0].dialoguePortraitName.ToString());
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
                    }
                    else if(input.goselected && !texts[0].unSkipable){
                        characterMouthAnimation.active = false;
                        textBoxData.currentChar = textBoxData.currentSentence.Length - 1;
                            textBoxData.isFinishedPage = true;
                            textBoxText.text = textBoxData.currentSentence.ToString();
                            characterMouthAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterMouthAnimation.sprites[0]);
                    }
                    else if(!textBoxData.isFinishedPage){
                        textBoxData.timeFromLastChar += DeltaTime * texts[0].textSpeed;
                        while (textBoxData.timeFromLastChar >= textBoxData.textSpeed && !textBoxData.isFinishedPage){
                            string textString = textBoxData.currentSentence.ToString();
                            textBoxData.currentChar++;
                            if(textBoxData.currentSentence[textBoxData.currentChar -1] != ' '){
                                AudioManager.playDialogue(texts[0].dialogueSoundName.ToString());
                            }
                            
                            
                            if(textBoxData.currentChar >= textString.Length){
                                textBoxData.isFinishedPage = true;
                                characterMouthAnimation.active = false;
                                characterMouthAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterMouthAnimation.sprites[0]);
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
        EntityManager.SetComponentData(characterBaseEntity, characterBaseAnimation);
        EntityManager.SetComponentData(characterEyesEntity, characterEyeAnimation);
        EntityManager.SetComponentData(characterMouthEntity, characterMouthAnimation);
    }
    public void DisplayChoices(Button[] choices){
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
        if(!isDisplaying){
            textBoxUI.visible = true;
            InputGatheringSystem.currentInput = CurrentInput.ui;
        }
        VisualElement playerChoiceUI = textBoxUI.Q<VisualElement>("player_choices");
        playerChoiceUI.Clear();
        foreach(Button choice in choices){
            choice.focusable = true;
            choice.Focus();
            playerChoiceUI.Add(choice);
        }

        EntityManager.SetComponentData(characterBaseEntity, characterBaseAnimation);
        EntityManager.SetComponentData(characterEyesEntity, characterEyeAnimation);
        EntityManager.SetComponentData(characterMouthEntity, characterMouthAnimation);
    }
    public void ContinueText(){
        VisualElement textBoxUI = uISystem.textBoxUI;
        VisualElement overworldOverlay = uISystem.overworldOverlay;

        Entity characterBaseEntity = characterBaseAnimationQuery.GetSingletonEntity();
        Entity characterEyesEntity = characterEyeAnimationQuery.GetSingletonEntity();
        Entity characterMouthEntity = characterMouthAnimationQuery.GetSingletonEntity();

        UIAnimationData characterBaseAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterBaseEntity);
        UIAnimationData characterEyeAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterEyesEntity);
        UIAnimationData characterMouthAnimation = EntityManager.GetComponentObject<UIAnimationData>(characterMouthEntity);

        Entities
        .WithoutBurst()
        .ForEach((DynamicBuffer<Text> texts, ref TextBoxData textBoxData) =>{
            Label textBoxText = textBoxUI.Q<Label>("TextBoxText");
            VisualElement characterImage = textBoxUI.Q<VisualElement>("CharacterImage");
            if(textBoxData.isFinishedPage && !texts.IsEmpty){
                if(texts.Length == 1){
                    texts.RemoveAt(0);
                    isDisplaying = false;
                    textBoxUI.visible = false;
                    overworldOverlay.visible = true;
                    textBoxText.text = "";
                    InputGatheringSystem.currentInput = CurrentInput.overworld;
                    OnDisplayFinished?.Invoke(this, EventArgs.Empty);
                }
                else{
                    texts.RemoveAt(0);
                    textBoxData.timeFromLastChar = 0f;
                    textBoxData.currentChar = 0;
                    if(texts[0].instant){
                        textBoxText.text = texts[0].text.ToString();
                        textBoxData.isFinishedPage = true;
                    }
                    else{
                        textBoxData.currentSentence = texts[0].text.ToString();
                        textBoxData.isFinishedPage = false;
                    }
                    CharacterPortraitData characterPortraitData = GetPortriatList(texts[0].dialoguePortraitName.ToString());
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
            }
            else if(!texts.IsEmpty && !texts[0].unSkipable){
                textBoxData.currentChar = textBoxData.currentSentence.Length - 1;
                textBoxData.isFinishedPage = true;
                characterMouthAnimation.active = false;
                characterMouthAnimation.visualElement.style.backgroundImage = Background.FromSprite(characterMouthAnimation.sprites[0]);
                textBoxText.text = textBoxData.currentSentence.ToString();
            }
        }).Run();
        EntityManager.SetComponentData(characterBaseEntity, characterBaseAnimation);
        EntityManager.SetComponentData(characterEyesEntity, characterEyeAnimation);
        EntityManager.SetComponentData(characterMouthEntity, characterMouthAnimation);
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

}