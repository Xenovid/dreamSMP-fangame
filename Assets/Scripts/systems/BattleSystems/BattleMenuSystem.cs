using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;
using Unity.Collections;
using System;

public class BattleMenuSystem : SystemBase
{
    selectables currentSelection = selectables.attack;
    VisualElement battleUI;
    VisualElement enemySelector;
    VisualElement skillSelector;
    UIDocument UIDoc;
    bool hasBattleStarted;

    InkDisplaySystem inkDisplaySystem;
    bool isPrintingVictoryData;
    public int currentSkill;

    //private bool isBattleMenuOn = false;
    private menuType currentMenu;
    private int currentCharacterSelected;
    private int currentEnemySelected;
    public BattleSystem battleSystem;
    public TransitionSystem transitionSystem;
    private bool isInEnemySelection;

    public int playerNumber;
    public bool hasMoved;
    VisualTreeAsset enemySelectionUITemplate;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleStart += WaitForTransition_OnBattleStart;
        battleSystem.OnBattleEnd += StartVictoryData_OnBattleEnd;
        battleSystem.OnBattleEnd += DisableMenu_OnBattleEnd;

        transitionSystem = World.GetExistingSystem<TransitionSystem>();
        enemySelectionUITemplate = Resources.Load<VisualTreeAsset>("UIDocuments/EnemyDetails");

        inkDisplaySystem = World.GetOrCreateSystem<InkDisplaySystem>();
    }

    protected override void OnStartRunning(){
        currentMenu = menuType.battleMenu;

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //getting the battle system then subscribing to the battle start event so it can activate the menu when the transition is done
    }

    protected override void OnUpdate()
    {
            EntityManager.CompleteAllJobs();
            float deltaTime = Time.DeltaTime;

            EntityQuery enemyUiSelectionGroup = GetEntityQuery(typeof(EnemySelectorUI), typeof(EnemySelectorData));
            NativeArray<Entity> enemyUiSelection = enemyUiSelectionGroup.ToEntityArray(Allocator.TempJob);


            int enemyLength = battleSystem.enemyEntities.Count;

            EntityQuery battleCharacters = GetEntityQuery(typeof(CharacterStats), typeof(BattleData));

            EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
            UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            Color black = Color.black;
            Color grey = Color.grey;

            /*EntityQuery caravanQuery = GetEntityQuery(typeof(CaravanTag));
            Entity caravan = caravanQuery.GetSingletonEntity();
            DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
            DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
            DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);
            */
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((DynamicBuffer<ItemData> itemInventory, AnimationData animation, Animator animator, PlayerSelectorUI selectorUI,int entityInQueryIndex, ref BattleData battleData, ref CharacterStats characterStats, in Entity entity) =>{
                DynamicBuffer<EquipedSkillData> equipedSkills = GetBuffer<EquipedSkillData>(entity);
                
                Label healthText = selectorUI.UI.Q<Label>("health_text");
                VisualElement healthBarBase = selectorUI.UI.Q<VisualElement>("health_bar_base");
                VisualElement healthBar = selectorUI.UI.Q<VisualElement>("health_bar");

                healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
                healthText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();

                if(selectorUI.isSelectable){
                    VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");
                    useBar.style.width = 0;
                    switch (currentMenu){
                        case menuType.battleMenu:
                            battleUI.visible = true;
                            enemySelector.visible = false;
                            skillSelector.visible = false;
                            if(selectorUI.isHovered && currentCharacterSelected == entityInQueryIndex && !hasMoved){
                                selectorUI.UI.AddToClassList("hovering");
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    selectorUI.isHovered = false;
                                    selectorUI.isSelected = true;

                                    selectorUI.UI.RemoveFromClassList("hovering");
                                    selectorUI.UI.AddToClassList("selected");
                                }
                                else if(input.moveleft){
                                    hasMoved = true;
                                    AudioManager.playSound("menuchange");
                                    currentCharacterSelected--;

                                    selectorUI.UI.RemoveFromClassList("hovering");
                                }
                                else if(input.moveright){
                                    hasMoved = true;
                                    AudioManager.playSound("menuchange");
                                    currentCharacterSelected++;

                                    selectorUI.UI.RemoveFromClassList("hovering");
                                }
                                if(currentCharacterSelected < 0){
                                    currentCharacterSelected = playerNumber - 1;
                                }
                                else if(currentCharacterSelected >= playerNumber){
                                    currentCharacterSelected = 0;
                                }
                            }
                            else if(selectorUI.isSelected){
                                if(input.goback){
                                    AudioManager.playSound("menuchange");
                                    selectorUI.isSelected = false;
                                    selectorUI.isHovered = true;

                                    selectorUI.UI.RemoveFromClassList("selected");
                                    selectorUI.UI.AddToClassList("hovering");
                                }
                                if(input.goselected){
                                    switch(selectorUI.currentSelection){
                                        case battleSelectables.fight:
                                            currentMenu = menuType.attackMenu;
                                            currentEnemySelected = 0;
                                            SelectEnemy(enemyUiSelection[0]);
                                        break;
                                        case battleSelectables.skills:
                                            battleUI.visible = false;
                                            skillSelector.visible = true;
                                            currentMenu = menuType.skillMenu;
                                            currentSkill = 1;
                                            SelectSkill(skillSelector.Q<Label>("skill1"));
                                            for(int i = 0; i < equipedSkills.Length; i++){
                                                skillSelector.Q<Label>("skill" + (i + 1).ToString()).text = equipedSkills[i].skill.name.ToString();
                                            }
                                        break;
                                        case battleSelectables.items:
                                        break;
                                    }
                                }
                                
                                if(input.moveright && !(selectorUI.currentSelection == battleSelectables.run)){
                                    Debug.Log(selectorUI.currentSelection.ToString());
                                    AudioManager.playSound("menuchange");
                                    VisualElement currentItemUI = selectorUI.UI.Q(selectorUI.currentSelection.ToString());
                                    currentItemUI.AddToClassList("item");
                                    currentItemUI.RemoveFromClassList("item_selected");
                                    selectorUI.currentSelection++;

                                    VisualElement nextItemUI = selectorUI.UI.Q((selectorUI.currentSelection).ToString());
                                    nextItemUI.RemoveFromClassList("item");
                                    nextItemUI.AddToClassList("item_selected");
                                    
                                }
                                else if(input.moveleft && !(selectorUI.currentSelection == battleSelectables.fight)){
                                    AudioManager.playSound("menuchange");
                                    VisualElement currentItemUI = selectorUI.UI.Q(selectorUI.currentSelection.ToString());
                                    currentItemUI.AddToClassList("item");
                                    currentItemUI.RemoveFromClassList("item_selected");
                                    selectorUI.currentSelection--;

                                    VisualElement nextItemUI = selectorUI.UI.Q((selectorUI.currentSelection).ToString());
                                    nextItemUI.RemoveFromClassList("item");
                                    nextItemUI.AddToClassList("item_selected");
                                }
                            }

                            break;
                        case menuType.attackMenu:
                            battleUI.visible = false;
                            skillSelector.visible = false;
                            enemySelector.visible = true;
                            if(input.goselected){
                                DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(enemyUiSelection[currentEnemySelected]);
                                animator.Play(animation.basicSwordAnimationName);
                                AudioManager.playSound("swordswing");
                                //makes sure that nothing is selected
                                foreach(Entity ent in enemyUiSelection){
                                    EnemySelectorData temp = GetComponent<EnemySelectorData>(ent);
                                    temp.isSelected = false;
                                    EntityManager.SetComponentData(ent, temp);
                                }
                                //deal damage to the enemy
                                enemyDamages.Add(new DamageData{damage = characterStats.equipedWeapon.power});

                                // wait until you are recharged
                                battleData.useTime = characterStats.equipedWeapon.useTime;
                                battleData.maxUseTime = battleData.useTime;


                                //inventory.inventory[selectorUI.currentItem].weapon.rechargeTime = inventory.inventory[selectorUI.currentItem].weapon.attackTime;                               

                                currentMenu = menuType.battleMenu;

                                selectorUI.isSelected = false;
                                selectorUI.isHovered = true;

                                battleUI.visible = true;
                                enemySelector.visible = false;

                                selectorUI.UI.RemoveFromClassList("selected");
                                selectorUI.UI.AddToClassList("hovering");

                                selectorUI.isSelectable = false;
                                currentEnemySelected = 0;
                                break;
                            }
                            else if(input.moveleft && currentEnemySelected > 0){
                                AudioManager.playSound("menuchange");
                                UnSelectEnemy(enemyUiSelection[currentEnemySelected]);
                                currentEnemySelected--;
                                SelectEnemy(enemyUiSelection[currentEnemySelected]);
                            }
                            else if(input.moveright && (currentEnemySelected < (enemyUiSelection.Length - 1))){
                                
                                AudioManager.playSound("menuchange");
                                UnSelectEnemy(enemyUiSelection[currentEnemySelected]);
                                currentEnemySelected++;
                                SelectEnemy(enemyUiSelection[currentEnemySelected]);
                            }
                            if(input.goback){
                                AudioManager.playSound("menuchange");
                                currentMenu = menuType.battleMenu;
                                foreach(Entity ent in enemyUiSelection){
                                    EnemySelectorData temp = GetComponent<EnemySelectorData>(ent);
                                    temp.isSelected = false;
                                    EntityManager.SetComponentData(ent, temp);
                                }
                                
                            }                           
                            break;
                        case menuType.skillMenu:
                            if(isInEnemySelection){
                                if(input.goselected){
                                    Skill skill = equipedSkills[currentSkill - 1].skill;
                                    DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(enemyUiSelection[currentEnemySelected]);
                                    animator.Play(animation.basicSwordAnimationName);
                                    AudioManager.playSound("swordswing");
                                    UnSelectEnemy(enemyUiSelection[currentEnemySelected]);
                                    //deal damage to the enemy
                                    enemyDamages.Add(new DamageData{damage = skill.damageBoost * characterStats.equipedWeapon.power});

                                    // wait until you are recharged
                                    battleData.useTime = skill.useTime;
                                    battleData.maxUseTime = battleData.useTime;                            

                                    currentMenu = menuType.battleMenu;

                                    selectorUI.isSelected = false;
                                    selectorUI.isHovered = true;

                                    battleUI.visible = true;
                                    enemySelector.visible = false;
                                    isInEnemySelection = false;

                                    selectorUI.UI.RemoveFromClassList("selected");
                                    selectorUI.UI.AddToClassList("hovering");

                                    selectorUI.isSelectable = false;
                                    currentEnemySelected = 0;
                                    break;
                                }
                                else if(input.moveleft && currentEnemySelected > 0){
                                    AudioManager.playSound("menuchange");
                                    UnSelectEnemy(enemyUiSelection[currentEnemySelected]);
                                    currentEnemySelected--;
                                    SelectEnemy(enemyUiSelection[currentEnemySelected]);
                                }
                                else if(input.moveright && (currentEnemySelected < (enemyUiSelection.Length - 1))){
                                    
                                    AudioManager.playSound("menuchange");
                                    UnSelectEnemy(enemyUiSelection[currentEnemySelected]);
                                    currentEnemySelected++;
                                    SelectEnemy(enemyUiSelection[currentEnemySelected]);
                                }
                            }
                            else{
                                if(input.goselected && currentSkill - 1< equipedSkills.Length){
                                    Debug.Log("hello you should go to enemy selection");
                                    isInEnemySelection = true;
                                    skillSelector.visible = false;
                                    enemySelector.visible = true;

                                    currentEnemySelected = 0;
                                    SelectEnemy(enemyUiSelection[0]);
                                }   
                                else if(input.goback){
                                    currentMenu = menuType.battleMenu;
                                }
                                else if(input.moveup){
                                    if(currentSkill > 1){
                                        UnSelectSkill(skillSelector.Q<Label>("skill" + currentSkill.ToString()));
                                        currentSkill--;
                                        SelectSkill(skillSelector.Q<Label>("skill" + currentSkill.ToString()));
                                    }
                                }
                                else if(input.movedown){
                                    if(currentSkill < 5){
                                        UnSelectSkill(skillSelector.Q<Label>("skill" + currentSkill.ToString()));
                                        currentSkill++;
                                        SelectSkill(skillSelector.Q<Label>("skill" + currentSkill.ToString()));
                                    }
                                }
                            }
                        break;
                    }
                    
                }
                else{
                    if(battleData.useTime > 0)
                    {
                        VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");

                        useBar.style.width = selectorUI.UI.contentRect.width * ((battleData.maxUseTime - battleData.useTime) / battleData.maxUseTime);
                        battleData.useTime -= deltaTime;
                    }
                    else
                    {
                        VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");

                        
                  
                        selectorUI.isSelectable = true;
                        AudioManager.playSound("menuavailable");
                        //play audio
                    }
                }
            }).Run();
            enemyUiSelection.Dispose();
        
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        hasMoved = false;
    }
    private void SelectEnemy(Entity entity){
        EnemySelectorData temp = GetComponent<EnemySelectorData>(entity);
        temp.isSelected = true;
        EntityManager.SetComponentData(entity, temp);
    }
    private void UnSelectEnemy(Entity entity){
        EnemySelectorData temp = GetComponent<EnemySelectorData>(entity);
        temp.isSelected = false;
        EntityManager.SetComponentData(entity, temp);
    }
    private void ResumeGameWorld_OnTransitionEnd(System.Object sender, System.EventArgs e){
        InputGatheringSystem.currentInput = CurrentInput.overworld;
        transitionSystem.OnTransitionEnd -= ResumeGameWorld_OnTransitionEnd;
    }
    private void DisableMenu_OnBattleEnd(System.Object sender, System.EventArgs e){
        battleUI.visible = false;
        enemySelector.visible = false;
        hasBattleStarted = false;

        transitionSystem.OnTransitionEnd += ResumeGameWorld_OnTransitionEnd;
    }
    private void EnableMenu_OnTransitionEnd(System.Object sender, System.EventArgs e){
        if(!hasBattleStarted){
            Debug.Log("enabled menu");
            EntityQuery UIDocumentGroup = GetEntityQuery(typeof(UIDocument), typeof(BattleUITag));
            UIDoc = UIDocumentGroup.ToComponentArray<UIDocument>()[0];
            VisualElement root = UIDoc.rootVisualElement;
            battleUI = root.Q<VisualElement>("BattleUI");
            enemySelector = root.Q<VisualElement>("EnemySelector");
            skillSelector = root.Q<VisualElement>("skill_selector");
            battleUI.visible = true;
            // adding the selectorUI stuff to the players
            int i = 0;
            foreach(Entity entity in battleSystem.playerEntities){
                string tempstr = "character" + (i + 1).ToString();
                VisualElement currentCharacter = battleUI.Q<VisualElement>(tempstr);
                EntityManager.AddComponentObject(entity, new PlayerSelectorUI { UI = currentCharacter, currentSelection = battleSelectables.fight, isSelected = false, isHovered = i == 0 });
            }

            foreach(Entity entity in battleSystem.enemyEntities){
                VisualElement newEnemySelectUI = enemySelectionUITemplate.CloneTree();
                enemySelector.Add(newEnemySelectUI);
                CharacterStats characterStats = GetComponent<CharacterStats>(entity);
                EntityManager.AddComponentData(entity, new EnemySelectorData { enemyId = characterStats.id, isSelected = false });
                EntityManager.AddComponentObject(entity, new EnemySelectorUI{ enemySelectorUI = newEnemySelectUI});
            }
            transitionSystem.OnTransitionEnd -= EnableMenu_OnTransitionEnd;
            hasBattleStarted = true;
        }
    }
    private void WaitForTransition_OnBattleStart(System.Object sender, System.EventArgs e){
        playerNumber = battleSystem.playerEntities.Count;
        transitionSystem.OnTransitionEnd += EnableMenu_OnTransitionEnd;
    }

    private void FinishVictoryData_OnWritingFinished(object sender, System.EventArgs e){
        inkDisplaySystem.OnWritingFinished -= FinishVictoryData_OnWritingFinished;
        isPrintingVictoryData = false;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        // transition back once the writer is done
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((ref BeforeBattleData beforeBattleData, in Entity entity) =>
        {
            ecb.AddComponent(entity, new TransitionData{newPosition = beforeBattleData.previousLocation});
            ecb.RemoveComponent<BeforeBattleData>(entity);
        }).Run();
    }
    private void StartVictoryData_OnBattleEnd(object sender, OnBattleEndEventArgs e){
        AudioManager.stopSong("tempBattleMusic");
        if(e.isPlayerVictor){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            EntityQuery battleCharacterGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<Entity> battleCharacters = battleCharacterGroup.ToEntityArray(Allocator.TempJob);

            EntityQuery beforeBattleGroup = GetEntityQuery(typeof(BeforeBattleData));
            NativeArray<Entity> beforeBattleDatas = beforeBattleGroup.ToEntityArray(Allocator.TempJob);

            // if the player wins, give them some awards and give them some kind 
            // if the player loses, set them to their last respawn point

            //if a character loses or wins, say the results
            //since they aren't in a battle, remove the battle data
            foreach (Entity entity in battleCharacters){
                ecb.RemoveComponent<BattleData>(entity);
            }

            beforeBattleDatas.Dispose();
            battleCharacters.Dispose();

            isPrintingVictoryData = true;
            inkDisplaySystem.DisplayVictoryData();
            inkDisplaySystem.OnWritingFinished += FinishVictoryData_OnWritingFinished;
        }
        
    }
    private void SelectSkill(Label skill){
        skill.RemoveFromClassList("skill_unselected");
        skill.AddToClassList("skill_selected");
    }
    private void UnSelectSkill(Label skill){
        skill.RemoveFromClassList("skill_selected");
        skill.AddToClassList("skill_unselected");
    }
}


public enum menuType{
    battleMenu,
    attackMenu,
    skillMenu,
    itemsMenu
}
