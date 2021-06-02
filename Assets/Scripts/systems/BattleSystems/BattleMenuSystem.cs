using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Transforms;

public class BattleMenuSystem : SystemBase
{
    VisualElement battleUI;
    VisualElement enemySelector;
    VisualElement skillSelector;
    VisualElement itemSelector;
    UIDocument UIDoc;
    bool hasBattleStarted;

    InkDisplaySystem inkDisplaySystem;
    public int currentSkill;
    public int currentItem;
    private int currentPlayer;

    private menuType currentMenu;
    private int currentCharacterSelected;
    private int currentEnemySelected;
    public BattleSystem battleSystem;
    public TransitionSystem transitionSystem;
    private bool isInEnemySelection;
    private bool isInPlayerSelection;

    public int playerNumber;
    public bool hasMoved;
    VisualTreeAsset enemySelectionUITemplate;
    VisualTreeAsset overHeadUITemplate;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleStart += WaitForTransition_OnBattleStart;
        battleSystem.OnBattleEnd += StartVictoryData_OnBattleEnd;
        battleSystem.OnBattleEnd += DisableMenu_OnBattleEnd;

        transitionSystem = World.GetExistingSystem<TransitionSystem>();
        enemySelectionUITemplate = Resources.Load<VisualTreeAsset>("UIDocuments/EnemyDetails");
        overHeadUITemplate = Resources.Load<VisualTreeAsset>("UIDocuments/OverHeadBattleStats");

        inkDisplaySystem = World.GetOrCreateSystem<InkDisplaySystem>();
    }

    protected override void OnStartRunning(){
        currentMenu = menuType.battleMenu;

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //getting the battle system then subscribing to the battle start event so it can activate the menu when the transition is done
    }

    protected override void OnUpdate()
    {
        PlayerSkillFunctions playerSkillFunctions = new PlayerSkillFunctions();
            EntityManager.CompleteAllJobs();
            float deltaTime = Time.DeltaTime;

            EntityQuery enemyUiSelectionGroup = GetEntityQuery(typeof(EnemySelectorUI), typeof(EnemySelectorData));
            NativeArray<Entity> temp = enemyUiSelectionGroup.ToEntityArray(Allocator.TempJob);
            List<Entity> enemyUiSelection = new List<Entity>();

            // sometimes the order isn't right, this makes it match the order in the battlesystem
            foreach(Entity entity in battleSystem.enemyEntities){
                for(int i = 0; i < temp.Length; i++){
                    if(temp[i] == entity){
                        enemyUiSelection.Add(entity);
                    }
                }
            }

            int enemyLength = battleSystem.enemyEntities.Count;

            EntityQuery battleCharacters = GetEntityQuery(typeof(CharacterStats), typeof(BattleData));

            EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
            UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            Color black = Color.black;
            Color grey = Color.grey;

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((DynamicBuffer<ItemData> itemInventory, AnimationData animation, Animator animator, PlayerSelectorUI selectorUI,int entityInQueryIndex, ref BattleData battleData, ref CharacterStats characterStats, in Entity entity) =>{
                DynamicBuffer<EquipedSkillData> equipedSkills = GetBuffer<EquipedSkillData>(entity);

                Label healthText = selectorUI.UI.Q<Label>("health_text");
                VisualElement healthBarBase = selectorUI.UI.Q<VisualElement>("health_bar_base");
                VisualElement healthBar = selectorUI.UI.Q<VisualElement>("health_bar");

                Label bloodText = selectorUI.UI.Q<Label>("blood_text");
                VisualElement bloodBarBase = selectorUI.UI.Q<VisualElement>("blood_bar_base");
                VisualElement bloodBar = selectorUI.UI.Q<VisualElement>("blood_bar");

                healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
                healthText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();

                bloodBar.style.width = bloodBarBase.contentRect.width * (characterStats.points/ characterStats.maxPoints);
                bloodText.text = "Blood: " + characterStats.points.ToString() + "/" + characterStats.maxPoints.ToString();

                if(selectorUI.isSelectable){
                    battleData.DamageTaken = 0;
                    VisualElement useBar = selectorUI.UI.Q<VisualElement>("use_bar");
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
                                    selectorUI.UI.experimental.animation.Position(new Vector3(0,-76,0), 100);
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
                                    selectorUI.UI.experimental.animation.Position(new Vector3(0,0,0), 0);
                                    AudioManager.playSound("menuchange");
                                    selectorUI.isSelected = false;
                                    selectorUI.isHovered = true;

                                    selectorUI.UI.RemoveFromClassList("selected");
                                    selectorUI.UI.AddToClassList("hovering");
                                }
                                if(input.goselected){
                                    int i = 0;
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
                                            while(i < equipedSkills.Length){
                                                skillSelector.Q<Label>("skill" + (i + 1).ToString()).text = equipedSkills[i].skill.name.ToString();
                                                i++;
                                            }
                                        break;
                                        case battleSelectables.items:
                                            battleUI .visible = false;
                                            itemSelector.visible = true;
                                            currentMenu = menuType.itemsMenu;
                                            currentItem = 1;
                                            SelectItem(itemSelector.Q<Label>("item1"));
                                            // changing the names of the items
                                            while(i < itemInventory.Length){
                                                itemSelector.Q<Label>("item" + (i + 1).ToString()).text = itemInventory[i].item.name.ToString();
                                                i++;
                                            }
                                            while(i < 10){
                                                itemSelector.Q<Label>("item" + (i + 1).ToString()).text = "None";
                                                i++;
                                            }
                                        break;
                                    }
                                }
                                
                                if(input.moveright && !(selectorUI.currentSelection == battleSelectables.run)){
                                    AudioManager.playSound("menuchange");
                                    VisualElement currentChoiceUI = selectorUI.UI.Q(selectorUI.currentSelection.ToString());
                                    UnSelectMenuChoice(currentChoiceUI);
                                    selectorUI.currentSelection++;

                                    VisualElement nextChoiceUI = selectorUI.UI.Q((selectorUI.currentSelection).ToString());
                                    SelectMenuChoice(nextChoiceUI);
                                    
                                }
                                else if(input.moveleft && !(selectorUI.currentSelection == battleSelectables.fight)){
                                    AudioManager.playSound("menuchange");
                                    VisualElement currentChoiceUI = selectorUI.UI.Q(selectorUI.currentSelection.ToString());
                                    UnSelectMenuChoice(currentChoiceUI);
                                    selectorUI.currentSelection--;

                                    VisualElement nextChoiceUI = selectorUI.UI.Q((selectorUI.currentSelection).ToString());
                                    SelectMenuChoice(nextChoiceUI);
                                }
                            }
                            break;
                        case menuType.attackMenu:
                            battleUI.visible = false;
                            skillSelector.visible = false;
                            enemySelector.visible = true;
                            if(input.goselected){
                                selectorUI.UI.experimental.animation.Position(new Vector3(0,0,0), 0);
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
                            else if(input.moveright && (currentEnemySelected < (enemyUiSelection.Count - 1))){
                                
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
                                    selectorUI.UI.experimental.animation.Position(new Vector3(0,0,0), 0);
                                            Skill skill = equipedSkills[currentSkill - 1].skill;
                                            if(characterStats.points - skill.cost >= 0){
                                                characterStats.points -= skill.cost;
                                            }
                                            else{
                                                characterStats.points = 0;
                                                int cost = skill.cost - characterStats.points;
                                            }
                                            
                                            animator.Play(animation.basicSwordAnimationName);
                                            AudioManager.playSound("swordswing");
                                            UnSelectEnemy(enemyUiSelection[currentEnemySelected]);
                                            //deal damage to the enemy
                                            /*
                                            Type type = typeof(PlayerSkillFunctions);
                                            MethodInfo method = type.GetMethod("basicSkill");
                                            method.Invoke(playerSkillFunctions, );*/

                                            ecb.AddComponent(entity, new UsingSkillData{
                                                target = enemyUiSelection[currentEnemySelected],
                                                skill = skill
                                            });
                                            ecb.AddComponent<BasicSkillTag>(entity);

                                            // wait until you are recharged
                                            battleData.useTime = 0;
                                            battleData.maxUseTime = skill.waitTime;                            

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
                                    } 
                                else if(input.goback){
                                    selectorUI.UI.experimental.animation.Position(new Vector3(0,-38,0), 0);
                                    isInEnemySelection = false;
                                    skillSelector.visible = true;
                                    enemySelector.visible = false;
                                }
                                else if(input.moveleft && currentEnemySelected > 0){
                                    AudioManager.playSound("menuchange");
                                    UnSelectEnemy(enemyUiSelection[currentEnemySelected]);
                                    currentEnemySelected--;
                                    SelectEnemy(enemyUiSelection[currentEnemySelected]);
                                }
                                else if(input.moveright && (currentEnemySelected < (enemyUiSelection.Count - 1))){
                                    
                                    AudioManager.playSound("menuchange");
                                    UnSelectEnemy(enemyUiSelection[currentEnemySelected]);
                                    currentEnemySelected++;
                                    SelectEnemy(enemyUiSelection[currentEnemySelected]);
                                }
                            }
                            else{
                                if(input.goselected && currentSkill - 1< equipedSkills.Length){
                                    isInEnemySelection = true;
                                    skillSelector.visible = false;
                                    enemySelector.visible = true;

                                    currentEnemySelected = 0;
                                    SelectEnemy(enemyUiSelection[0]);
                                }   
                                else if(input.goback){
                                    currentMenu = menuType.battleMenu;
                                    skillSelector.visible = false;
                                    battleUI.visible = true;
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
                        case menuType.itemsMenu:
                            Item item =  currentItem - 1 < itemInventory.Length ? itemInventory[currentItem - 1].item : new Item{itemType = ItemType.none};
                            if(isInEnemySelection){

                            }
                            else if(isInPlayerSelection){
                                if(input.goselected){
                                    // when adding the moving box, make sure to turn it off here
                                    selectorUI.UI.experimental.animation.Position(new Vector3(0,0,0), 0);
                                    switch(item.itemType){
                                        case ItemType.healing:
                                            DynamicBuffer<HealingData> healings = GetBuffer<HealingData>(battleSystem.playerEntities[currentPlayer]);
                                            HealingData healing = new HealingData{healing = item.strength};
                                            healings.Add(healing);

                                            
                                        break;
                                    }
                                    currentMenu = menuType.battleMenu;
                                    selectorUI.isSelectable = false;
                                    battleData.useTime = item.useTime;
                                    battleData.maxUseTime = battleData.useTime;
                                    itemInventory.RemoveAt(currentItem - 1);
                                    currentPlayer = 0;
                                    currentItem = 0;
                                    isInPlayerSelection = false;
                                }
                                else if(input.goback){
                                    isInPlayerSelection = false;
                                    itemSelector.visible = true;
                                    battleUI.visible = false;
                                }
                                //have a box that highlights a playerbox that animates over
                                else if(input.moveright && currentPlayer < playerNumber){
                                    currentPlayer++;
                                }
                                else if(input.moveleft && currentPlayer > 0){
                                    currentPlayer--;
                                }
                            }
                            else{
                                if(input.goselected){
                                    
                                    switch(item.itemType){
                                        case ItemType.healing:
                                            isInPlayerSelection = true;
                                            itemSelector.visible = false;
                                            battleUI.visible = true;
                                        break;
                                    }
                                }
                                else if(input.goback){
                                    currentMenu = menuType.battleMenu;
                                    itemSelector.visible = false;
                                    battleUI.visible = true;
                                }
                                else if(input.movedown){
                                    if(currentItem < 10){
                                        UnSelectItem(itemSelector.Q<Label>("item" + currentItem.ToString()));
                                        currentItem++;
                                        SelectItem(itemSelector.Q<Label>("item" + currentItem.ToString()));
                                    }
                                }
                                else if(input.moveup){
                                    if(currentItem > 1){
                                        UnSelectItem(itemSelector.Q<Label>("item" + currentItem.ToString()));
                                        currentItem--;
                                        SelectItem(itemSelector.Q<Label>("item" + currentItem.ToString()));
                                    }
                                }
                                else if(input.moveleft){
                                    if(currentItem > 5){
                                        UnSelectItem(itemSelector.Q<Label>("item" + currentItem.ToString()));
                                        currentItem-= 5;
                                        SelectItem(itemSelector.Q<Label>("item" + currentItem.ToString()));
                                    }
                                }
                                else if(input.moveright){
                                    if(currentItem < 6){
                                        UnSelectItem(itemSelector.Q<Label>("item" + currentItem.ToString()));
                                        currentItem+= 5;
                                        SelectItem(itemSelector.Q<Label>("item" + currentItem.ToString()));
                                    }
                                }
                            }
                        break;
                    } 
                }
                else{
                    if(battleData.useTime < battleData.maxUseTime)
                    {
                        VisualElement useBar = selectorUI.UI.Q<VisualElement>("use_bar");

                        useBar.style.width = selectorUI.UI.contentRect.width * ((battleData.useTime) / battleData.maxUseTime);
                        battleData.useTime += deltaTime;
                    }
                    else
                    {
                        VisualElement useBar = selectorUI.UI.Q<VisualElement>("use_bar");
                    
                        selectorUI.isSelectable = true;
                        selectorUI.isHovered = true;
                        AudioManager.playSound("menuavailable");
                        //play audio
                    }
                }
            }).Run();
            temp.Dispose();
        
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        hasMoved = false;
    }
    private void SelectMenuChoice(VisualElement choice){
        choice.RemoveFromClassList("choice_unselected");
        choice.AddToClassList("choice_selected");
    }
    private void UnSelectMenuChoice(VisualElement choice){
        choice.RemoveFromClassList("choice_selected");
        choice.AddToClassList("choice_unselected");
    }
    private void SelectItem(Label item){
        item.RemoveFromClassList("skill_unselected");
        item.AddToClassList("skill_selected");
    }
    private void UnSelectItem(Label item){
        item.RemoveFromClassList("skill_selected");
        item.AddToClassList("skill_unselected");
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
            // enables all the features of the menu
            Camera cam = GetEntityQuery(typeof(Camera)).ToComponentArray<Camera>()[0];
            float positionRatio = 1280.0f / cam.pixelWidth;
            EntityQuery UIDocumentGroup = GetEntityQuery(typeof(UIDocument), typeof(BattleUITag));
            UIDoc = UIDocumentGroup.ToComponentArray<UIDocument>()[0];
            VisualElement root = UIDoc.rootVisualElement;
            battleUI = root.Q<VisualElement>("BattleUI");
            enemySelector = root.Q<VisualElement>("EnemySelector");
            skillSelector = root.Q<VisualElement>("skill_selector");
            itemSelector = root.Q<VisualElement>("item_selector");
            battleUI.visible = true;
            // adding the selectorUI stuff to the players
            int i = 0;
            foreach(Entity entity in battleSystem.playerEntities){
                Translation translation = GetComponent<Translation>(entity);
                string tempstr = "character" + (i + 1).ToString();
                VisualElement currentCharacter = battleUI.Q<VisualElement>(tempstr);
                EntityManager.AddComponentObject(entity, new PlayerSelectorUI { UI = currentCharacter, currentSelection = battleSelectables.fight, isSelected = false, isHovered = i == 0 });
            } 

            foreach(Entity entity in battleSystem.enemyEntities){
                Translation translation = GetComponent<Translation>(entity);

                VisualElement newEnemySelectUI = enemySelectionUITemplate.CloneTree();
                VisualElement newHeadsUpDisplay = overHeadUITemplate.CloneTree();
                Debug.Log(root);
                root.Add(newHeadsUpDisplay);
                Vector3 camPo =  cam.WorldToScreenPoint(translation.Value);
                Vector2 uiPosition =  new Vector2(camPo.x * positionRatio, camPo.y * positionRatio);//root.WorldToLocal(new Vector2(translation.Value.x, translation.Value.y));
                //uiPosition = new Vector2(uiPosition.x )
                Debug.Log(uiPosition);
                newHeadsUpDisplay.Q<VisualElement>("base").style.bottom = uiPosition.y;
                newHeadsUpDisplay.Q<VisualElement>("base").style.left = uiPosition.x;
                //newHeadsUpDisplay.transform.position = translation.Value;
                //newHeadsUpDisplay.contentRect.position.Set(-10, 0);//uiPosition.x, uiPosition.y);
                //newHeadsUpDisplay.experimental.animation.Position(new Vector3(0, -100, 0), 100);
                
                enemySelector.Add(newEnemySelectUI);
                CharacterStats characterStats = GetComponent<CharacterStats>(entity);
                EntityManager.AddComponentData(entity, new EnemySelectorData { enemyId = characterStats.id, isSelected = false });
                EntityManager.AddComponentObject(entity, new EnemySelectorUI{ enemySelectorUI = newEnemySelectUI});
                EntityManager.AddComponentObject(entity, new HeadsUpUIData{UI = newHeadsUpDisplay, messages = new List<Message>()});
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
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        // transition back once the writer is done
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((ref BeforeBattleData beforeBattleData,in Translation translation, in Entity entity) =>
        {
            ecb.AddComponent(entity, new TransitionData{newPosition = beforeBattleData.previousLocation, oldPosition = translation.Value});
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
