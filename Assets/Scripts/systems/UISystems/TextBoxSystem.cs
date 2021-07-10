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

    protected override void OnStartRunning()
    {
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

        UIInputData input = GetSingleton<UIInputData>();
        Entities
        .WithoutBurst()
        .ForEach((DynamicBuffer<Text> texts, CharacterPortraitData characterPortrait, ref TextBoxData textBoxData) =>{ 
                if(!texts.IsEmpty && !isDisplaying){
                    overworldOverlay.visible = false;
                    textBoxUI.visible = true;
                    textBoxUI.Focus();

                    //ensures that there isn't any buttons blocking the view
                    VisualElement playerChoiceUI = textBoxUI.Q<VisualElement>("player_choices");
                    playerChoiceUI.Clear();
                    

                    InputGatheringSystem.currentInput = CurrentInput.ui;
                    Label textBoxText = textBoxUI.Q<Label>("TextBoxText");
                    VisualElement characterImage = textBoxUI.Q<VisualElement>("CharacterImage");

                    characterImage.visible = false;
                    
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

                    if(characterPortrait.portraits.Count > 0 && characterPortrait.portraits[0] != null){
                        characterImage.visible = true;
                        characterImage.style.backgroundImage = Background.FromSprite(characterPortrait.portraits[0]);
                        characterPortrait.portraits.RemoveAt(0);

                    }
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
                            characterImage.visible = false;
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
                            if(characterPortrait.portraits.Count > 0 && characterPortrait.portraits[0] != null){
                                characterImage.visible = true;
                                characterImage.style.backgroundImage = Background.FromSprite(characterPortrait.portraits[0]);
                                characterPortrait.portraits.RemoveAt(0);
                            }
                            
                        }
                    }
                    else if(input.goselected){
                        textBoxData.currentChar = textBoxData.currentSentence.Length - 1;
                            textBoxData.isFinishedPage = true;
                            textBoxText.text = textBoxData.currentSentence.ToString();
                    }
                    else if(!textBoxData.isFinishedPage){
                        textBoxData.timeFromLastChar += DeltaTime;
                        while (textBoxData.timeFromLastChar >= textBoxData.textSpeed && !textBoxData.isFinishedPage){
                            string textString = textBoxData.currentSentence.ToString();
                            AudioManager.playDialogue(texts[0].dialogueSoundName.ToString());
                            textBoxData.currentChar++;
                            if(textBoxData.currentChar >= textString.Length){
                                textBoxData.isFinishedPage = true;
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
    }
    public void DisplayChoices(Button[] choices){
        
        Button textBoxUI = uISystem.textBoxUI;
        if(!isDisplaying){
            textBoxUI.visible = true;
            InputGatheringSystem.currentInput = CurrentInput.ui;
        }
        VisualElement playerChoiceUI = textBoxUI.Q<VisualElement>("player_choices");
        playerChoiceUI.Clear();
        foreach(Button choice in choices){
            choice.Focus();
            playerChoiceUI.Add(choice);
        }

    }
    public void ContinueText(){
        VisualElement textBoxUI = uISystem.textBoxUI;
        VisualElement overworldOverlay = uISystem.overworldOverlay;
        Entities
        .WithoutBurst()
        .ForEach((DynamicBuffer<Text> texts, CharacterPortraitData characterPortrait, ref TextBoxData textBoxData) =>{
            Label textBoxText = textBoxUI.Q<Label>("TextBoxText");
            VisualElement characterImage = textBoxUI.Q<VisualElement>("CharacterImage");
            if(textBoxData.isFinishedPage && !texts.IsEmpty){
                if(texts.Length == 1){
                    texts.RemoveAt(0);
                    isDisplaying = false;
                    textBoxUI.visible = false;
                    overworldOverlay.visible = true;
                    characterImage.visible = false;
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
                    if(characterPortrait.portraits.Count > 0 && characterPortrait.portraits[0] != null){
                        characterImage.visible = true;
                        characterImage.style.backgroundImage = Background.FromSprite(characterPortrait.portraits[0]);
                        characterPortrait.portraits.RemoveAt(0);
                    } 
                }
            }
            else if(!texts.IsEmpty){
                textBoxData.currentChar = textBoxData.currentSentence.Length - 1;
                textBoxData.isFinishedPage = true;
                textBoxText.text = textBoxData.currentSentence.ToString();
            }
        
        }).Run();
    }
}