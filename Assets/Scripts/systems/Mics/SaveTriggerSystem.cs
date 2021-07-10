using Unity.Entities;
using Unity.Physics;
using System;
using Unity.Physics.Systems;
using UnityEngine.UIElements;
using UnityEngine;
public class SaveTriggerSystem : SystemBase
{
    StepPhysicsWorld physicsWorld;
    public event SavePointEventHandler SavePointAlert;
    SaveAndLoadSystem saveAndLoadSystem;
    UISystem uISystem;
    TextBoxSystem textBoxSystem;
    InkDisplaySystem inkDisplaySystem;

    protected override void OnStartRunning()
    {
        saveAndLoadSystem = World.GetOrCreateSystem<SaveAndLoadSystem>();
        textBoxSystem = World.GetOrCreateSystem<TextBoxSystem>();
        uISystem = World.GetOrCreateSystem<UISystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        inkDisplaySystem = World.GetOrCreateSystem<InkDisplaySystem>();
    }

    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;
        foreach(TriggerEvent triggerEvent in triggerEvents)
        {
                
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (HasComponent<SavePointTag>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
            {
                if(input.select){
                    string savePointName = GetComponent<SavePointTag>(entityA).saveName.ToString();
                    //SavePointAlert?.Invoke(this, new SavePointEventArg{savePointName = savePointName});
                    Entities
                    .WithStructuralChanges()
                    .ForEach((ref CharacterStats character) => {
                        character.health = character.maxHealth;
                    }).Run();
                    
                    Button ringButton = new Button();
                    ringButton.AddToClassList("player_choice");
                    ringButton.text = "ring the bell";
                    ringButton.text = "ring the bell";
                    textBoxSystem.DisplayChoices(new Button[]{ringButton});
                   
                }
                else{
                    uISystem.EnableInteractive();
                }
            }
            else if (HasComponent<SavePointTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
            {
                
                if(input.select){
                    Entities
                    .WithStructuralChanges()
                    .ForEach((ref CharacterStats character) => {
                        character.health = character.maxHealth;
                    }).Run();
                    uISystem.overworldOverlay.visible = false;
                    string savePointName = GetComponent<SavePointTag>(entityB).saveName.ToString();
                    Button ringButton = new Button();
                    ringButton.AddToClassList("player_choice");
                    ringButton.text = "ring the bell";
                    ringButton.focusable = true;
                    // when the ring button is pressed, display a cutscene to ring the bell
                    ringButton.clicked += () => {inkDisplaySystem.StartCutScene("thebell.thefirstbell");};

                    Button saveButton = new Button();
                    saveButton.AddToClassList("player_choice");
                    saveButton.text = "save progress";
                    saveButton.clicked += () => saveAndLoadSystem.LoadSaveUI(this, new SavePointEventArg{savePointName = savePointName});

                    textBoxSystem.DisplayChoices(new Button[]{ringButton, saveButton});

                    
                }
                else{
                    uISystem.EnableInteractive();
                }
            }
        }
    }
}

public delegate void SavePointEventHandler(System.Object sender, SavePointEventArg e);

public class SavePointEventArg : EventArgs{
    public string savePointName;
}
