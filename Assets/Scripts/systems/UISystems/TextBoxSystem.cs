using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;

public class TextBoxSystem : SystemBase
{
    public event EventHandler OnTextFinished;
    public event EventHandler OnDisplayFinished;
    public bool isDisplaying;
    UISystem uISystem;
    InkDisplaySystem inkDisplaySystem;
    EntityQuery characterBaseAnimationQuery;
    EntityQuery characterEyeAnimationQuery;
    EntityQuery characterMouthAnimationQuery;

    protected override void OnStartRunning()
    {
        characterBaseAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterBaseTag));
        characterEyeAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterEyeTag));
        characterMouthAnimationQuery = GetEntityQuery(typeof(UIAnimationData), typeof(CharacterMouthTag));

        TextBoxData text = GetSingleton<TextBoxData>();
        text.textSpeed = text.textSpeed == 0 ? .02f : text.textSpeed;
        SetSingleton<TextBoxData>(text);
        uISystem = World.GetOrCreateSystem<UISystem>();
        inkDisplaySystem = World.GetOrCreateSystem<InkDisplaySystem>();
    }
    protected override void OnUpdate()
    {
        float DeltaTime = Time.DeltaTime;
        
        

        
    }



}