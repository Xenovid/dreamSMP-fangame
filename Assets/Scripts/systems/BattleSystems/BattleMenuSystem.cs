using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;
using Unity.Collections;

public class BattleMenuSystem : SystemBase
{
    public selectables currentSelection = selectables.attack;
    public VisualElement battleUI;
    public Label attackLabel;
    public Label itemLabel;
    public Label runLabel;
    UIDocument UIDoc;
    public int test = 0;

    //private bool isBattleMenuOn = false;
    private menuType currentMenu;
    private int currentCharacterSelected;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnStartRunning(){
        currentMenu = menuType.battleMenu;
        base.OnStartRunning();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDoc = UIDocs[0];

        Label attackLabel = UIDoc.rootVisualElement.Q<Label>("AttackLabel");
        attackLabel.RemoveFromClassList("battle-label");
        attackLabel.AddToClassList("battle-label-selected");
    }

    protected override void OnUpdate()
    {
        EntityManager.CompleteAllJobs();

        EntityQuery BattleManagerGroup = GetEntityQuery(typeof(BattleManagerTag));
        NativeArray<Entity> battleManagers = BattleManagerGroup.ToEntityArray(Allocator.Temp);
        
        EntityQuery enemyUiSelectionGroup = GetEntityQuery(typeof(EnemySelectorUI), typeof(EnemySelectorData));
        NativeArray<Entity> enemyUiSelection = enemyUiSelectionGroup.ToEntityArray(Allocator.TempJob);

        DynamicBuffer<EnemyBattleData> EnemyIds = new DynamicBuffer<EnemyBattleData>();

        EntityQuery battleCharacters = GetEntityQuery(typeof(CharacterStats), typeof(BattleData));

        bool isBattling = false;
        
        foreach(Entity entity in battleManagers){
            EnemyIds = GetBuffer<EnemyBattleData>(entity);
            isBattling = true;
        }
        if(!isBattling){
            
        }

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Color black = Color.black;
        Color grey = Color.grey;
        var rootVisualElement = UIDoc.rootVisualElement;
        if(rootVisualElement == null){
            Debug.Log("didn't find root visual element");
        }
        else{
            attackLabel = rootVisualElement.Q<Label>("AttackLabel");
            itemLabel= rootVisualElement.Q<Label>("ItemsLabel");
            runLabel = rootVisualElement.Q<Label>("RunLabel");
            battleUI = rootVisualElement.Q<VisualElement>("BattleUI");

            VisualElement enemySelector = rootVisualElement.Q<VisualElement>("EnemySelector");
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((ref BattleData battleData, ref CharacterStats characterStat, in Entity entity, in UIInputData input) =>{
                if(!isBattling){
                    //To Do: should display victory screen
                    
                }
                else{
                    //isBattleMenuOn = true;
                    switch(currentMenu){
                        case menuType.battleMenu:
                            battleUI.visible = true;
                            enemySelector.visible = false;

                            switch(currentSelection){
                            case selectables.attack:
                                if(input.goselected){
                                    currentMenu = menuType.attackMenu;
                                }
                                else{
                                    battleData.selected = selectables.none;
                                    if(input.moveup){
                                        AudioManager.playSound("menuchange");
                                        currentSelection = selectables.run;
                                        attackLabel.RemoveFromClassList("battle-label-selected");
                                        attackLabel.AddToClassList("battle-label");
                                        runLabel.RemoveFromClassList("battle-label");
                                        runLabel.AddToClassList("battle-label-selected");
                                    }
                                    else if(input.movedown){
                                        AudioManager.playSound("menuchange");
                                        currentSelection = selectables.items;
                                        attackLabel.RemoveFromClassList("battle-label-selected");
                                        attackLabel.AddToClassList("battle-label");
                                        itemLabel.RemoveFromClassList("battle-label");
                                        itemLabel.AddToClassList("battle-label-selected");
                                    }
                                }
                                break;
                            case selectables.items:
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    battleData.selected = selectables.items;
                                }
                                else{
                                    battleData.selected = selectables.none;
                                    if(input.moveup){
                                        AudioManager.playSound("menuchange");
                                        currentSelection = selectables.attack;
                                        attackLabel.RemoveFromClassList("battle-label");
                                        attackLabel.AddToClassList("battle-label-selected");
                                        itemLabel.RemoveFromClassList("battle-label-selected");
                                        itemLabel.AddToClassList("battle-label");
                                    }
                                    else if(input.movedown){
                                        AudioManager.playSound("menuchange");
                                        currentSelection = selectables.run;
                                        itemLabel.RemoveFromClassList("battle-label-selected");
                                        itemLabel.AddToClassList("battle-label");
                                        runLabel.RemoveFromClassList("battle-label");
                                        runLabel.AddToClassList("battle-label-selected");
                                    }
                                }
                                break;
                            case selectables.run:
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    battleData.selected = selectables.run;
                                }
                                else{
                                    battleData.selected = selectables.none;
                                    if(input.movedown){
                                        AudioManager.playSound("menuchange");
                                        currentSelection = selectables.attack;
                                        attackLabel.RemoveFromClassList("battle-label");
                                        attackLabel.AddToClassList("battle-label-selected");
                                        runLabel.RemoveFromClassList("battle-label-selected");
                                        runLabel.AddToClassList("battle-label");
                                    }
                                    else if(input.moveup){
                                        AudioManager.playSound("menuchange");
                                        currentSelection = selectables.items;
                                        runLabel.RemoveFromClassList("battle-label-selected");
                                        runLabel.AddToClassList("battle-label");
                                        itemLabel.RemoveFromClassList("battle-label");
                                        itemLabel.AddToClassList("battle-label-selected");
                                    }
                                }
                                
                                break;
                        }
                            break;
                        case menuType.attackMenu:
                            battleUI.visible = false;
                            enemySelector.visible = true;

                            if(input.moveleft){
                                AudioManager.playSound("menuchange");
                                currentCharacterSelected--;
                            }
                            else if(input.moveright){
                                AudioManager.playSound("menuchange");
                                currentCharacterSelected++;
                            }
                            if(currentCharacterSelected == EnemyIds.Length){
                                currentCharacterSelected = 0;
                            }
                            else if(currentCharacterSelected < 0){
                                currentCharacterSelected = EnemyIds.Length - 1;
                            }

                            foreach(Entity ent in enemyUiSelection){
                                    EnemySelectorData temp = GetComponent<EnemySelectorData>(ent);

                                    if(temp.isDead && temp.enemyId == EnemyIds[currentCharacterSelected].id){
                                        currentCharacterSelected++;
                                        if(currentCharacterSelected == EnemyIds.Length){
                                            currentCharacterSelected = 0;
                                        }
                                    }
                                    else if(temp.enemyId == EnemyIds[currentCharacterSelected].id){
                                        Debug.Log(temp.enemyId.ToString() + ": should be selected");
                                        temp.isSelected = true;
                                        EntityManager.SetComponentData(ent, temp);
                                    }
                                    else{
                                        Debug.Log(temp.enemyId.ToString() + ": should not be selected");
                                        temp.isSelected = false;
                                        EntityManager.SetComponentData(ent, temp);
                                    }

                                    
                            }

                            if(input.goback){
                                AudioManager.playSound("menuchange");
                                currentMenu = menuType.battleMenu;
                            }                           
                            if(input.goselected){
                                AudioManager.playSound("menuchange");
                                battleData.targetingId = EnemyIds[currentCharacterSelected].id;
                                battleData.selected = selectables.attack;
                            }
                            else{
                                battleData.selected = selectables.none;
                            }
                            break;
                    }
                    
                }
            }).Run();
        }
        enemyUiSelection.Dispose();
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}

public enum menuType{
    battleMenu,
    attackMenu,
    skillMenu,
    itemsMenu
}
