using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Unity.Scenes;
using System.IO;
public class PlayerLoseMenuSystem : SystemBase
{
    UIDocument UIDoc;
    bool isActive = false;
    LoseMenuSelectables currentSelectable;

    public event EventHandler OnContinue;
    SceneSystem sceneSystem;
    BattleSystem battleSystem;
    TransitionSystem transitionSystem;

    SaveAndLoadSystem saveAndLoadSystem;
    protected override void OnCreate()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        saveAndLoadSystem = World.GetOrCreateSystem<SaveAndLoadSystem>();
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        transitionSystem = World.GetOrCreateSystem<TransitionSystem>();

        battleSystem.OnBattleEnd += DisplayLoss_OnPlayerLoss;
    }
    protected override void OnUpdate()
    {
        if(isActive){
            UIInputData input = GetSingleton<UIInputData>();
            VisualElement root = UIDoc.rootVisualElement;
            VisualElement losingBackground = root.Q<VisualElement>("losing_screen");

            switch(currentSelectable){
                case LoseMenuSelectables.continueButton:
                    if(input.goselected && File.Exists(Application.persistentDataPath + "/tempsave" + "/SavePointData")){
                        /*OverworldUITag overworld = GetSingleton<OverworldUITag>();
                        overworld.isVisable = false;
                        SetSingleton<OverworldUITag>(overworld);*/


                        AudioManager.playSound("menuselect");
                        isActive = false;
                        OnContinue?.Invoke(this, EventArgs.Empty);
                        losingBackground.visible = false;
                        saveAndLoadSystem.LoadLastSavePoint();
                        InputGatheringSystem.currentInput = CurrentInput.overworld;
                        Entities
                        .WithoutBurst()
                        .WithStructuralChanges()
                        .WithAll<PlayerTag>()
                        .ForEach((Animator animator, in AnimationData animationData) => {
                            animator.Play(animationData.idleRightAnimationName);
                        }).Run();
                    }
                    if(input.movedown){
                        AudioManager.playSound("menuchange");
                        UnSelectButton(losingBackground.Q<Label>("continue"));
                        SelectButton(losingBackground.Q<Label>("title"));
                        currentSelectable = LoseMenuSelectables.titleScreen;
                    }
                break;
                case LoseMenuSelectables.titleScreen:
                    if(input.goselected){
                        AudioManager.playSound("menuselect");
                        isActive = false;
                        currentSelectable = LoseMenuSelectables.continueButton;
                        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
                        sceneSystem.UnloadScene(SubSceneReferences.Instance.EssentialsSubScene.SceneGUID);
                        AudioManager.playSong("menuMusic");
                    }
                    if(input.moveup){
                        AudioManager.playSound("menuchange");
                        SelectButton(losingBackground.Q<Label>("continue"));
                        UnSelectButton(losingBackground.Q<Label>("title"));
                        currentSelectable = LoseMenuSelectables.continueButton;
                    }
                break;
            }
        }
    }

    private void DisplayLoss_OnPlayerLoss(object sender, OnBattleEndEventArgs e){
        if(!e.isPlayerVictor){
            /*
            AudioManager.playSound("defeatsong");
            EntityQuery UIDocQuery = GetEntityQuery(typeof(UIDocument), typeof(OverworldUITag));
            UIDoc = EntityManager.GetComponentObject<UIDocument>(UIDocQuery.GetSingletonEntity());

            VisualElement root = UIDoc.rootVisualElement;
            VisualElement losingBackground = root.Q<VisualElement>("losing_screen");
            

            losingBackground.visible = true;

            isActive = true;
            transitionSystem.OnTransitionEnd += UnLoadScenes_OnTransitionnEnd;*/
        }
    }
    private void UnLoadScenes_OnTransitionnEnd(object sender, System.EventArgs e){
        transitionSystem.OnTransitionEnd -= UnLoadScenes_OnTransitionnEnd;
        saveAndLoadSystem.UnLoadSubScenes();
    }
    private void SelectButton(Label button){
        button.AddToClassList("selected");
        button.RemoveFromClassList("unselected");
    }
    private void UnSelectButton(Label button){
        button.AddToClassList("unselected");
        button.RemoveFromClassList("selected");
    }

    public enum LoseMenuSelectables{
        continueButton,
        titleScreen
}
}

