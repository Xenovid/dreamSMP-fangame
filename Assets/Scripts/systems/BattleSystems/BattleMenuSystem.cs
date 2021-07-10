using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using Unity.Scenes;
using System;
using System.IO;

public class BattleMenuSystem : SystemBase
{
    public event EventHandler OnContinue;
    SceneSystem sceneSystem;
    SaveAndLoadSystem saveAndLoadSystem;
    VisualElement battleUI;
    VisualElement losingBackground;
    VisualElement enemySelector;
    VisualElement skillSelector;
    VisualElement itemSelector;
    VisualElement previousUI;
    UIDocument UIDoc;
    bool hasBattleStarted;

    InkDisplaySystem inkDisplaySystem;
    TextBoxSystem textBoxSystem;
    private int currentPlayer;

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
    UISystem uISystem;
    EntityQuery playerCharactersQuery;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        saveAndLoadSystem = World.GetOrCreateSystem<SaveAndLoadSystem>();
        uISystem = World.GetOrCreateSystem<UISystem>();
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleStart += WaitForTransition_OnBattleStart;
        battleSystem.OnBattleEnd += StartVictoryData_OnBattleEnd;
        battleSystem.OnBattleEnd += DisableMenu_OnBattleEnd;

        transitionSystem = World.GetExistingSystem<TransitionSystem>();
        enemySelectionUITemplate = Resources.Load<VisualTreeAsset>("UIDocuments/EnemyDetails");
        overHeadUITemplate = Resources.Load<VisualTreeAsset>("UIDocuments/OverHeadBattleStats");

        inkDisplaySystem = World.GetOrCreateSystem<InkDisplaySystem>();
        textBoxSystem = World.GetOrCreateSystem<TextBoxSystem>();

        playerCharactersQuery = GetEntityQuery(typeof(PlayerSelectorUI));
        battleSystem.OnBattleEnd += DisplayLoss_OnPlayerLoss;
    }

    protected override void OnStartRunning(){
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //getting the battle system then subscribing to the battle start event so it can activate the menu when the transition is done
    }

    protected override void OnUpdate()
    {
        PlayerSkillFunctions playerSkillFunctions = new PlayerSkillFunctions();
            EntityManager.CompleteAllJobs();
            float deltaTime = Time.DeltaTime;

            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((DynamicBuffer<ItemData> itemInventory, AnimationData animation, Animator animator, PlayerSelectorUI selectorUI,int entityInQueryIndex, ref BattleData battleData, ref CharacterStats characterStats, in Entity entity) =>{

                Label healthText = selectorUI.UI.Q<Label>("health_text");
                VisualElement healthBarBase = selectorUI.UI.Q<VisualElement>("health_bar_base");
                VisualElement healthBar = selectorUI.UI.Q<VisualElement>("health_bar");

                Label bloodText = selectorUI.UI.Q<Label>("blood_text");
                VisualElement bloodBarBase = selectorUI.UI.Q<VisualElement>("blood_bar_base");
                VisualElement bloodBar = selectorUI.UI.Q<VisualElement>("blood_bar");

                healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
                healthText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();

                bloodBar.style.width = bloodBarBase.contentRect.width * ((float)characterStats.points/ characterStats.maxPoints);
                bloodText.text = "Blood: " + characterStats.points.ToString() + "/" + characterStats.maxPoints.ToString();

                if(itemInventory.Length <= 0){
                    selectorUI.UI.Q<Button>("items").SetEnabled(false);
                }
                else{
                    selectorUI.UI.Q<Button>("items").SetEnabled(true);
                }
                if(battleData.useTime < battleData.maxUseTime)
                {

                    VisualElement useBar = selectorUI.UI.Q<VisualElement>("use_bar");

                    useBar.style.width = selectorUI.UI.contentRect.width * ((battleData.useTime) / battleData.maxUseTime);
                    battleData.useTime += deltaTime;
                    battleData.isRecharging = true;
                }
                else if(battleData.isRecharging)
                {
                    VisualElement useBar = selectorUI.UI.Q<VisualElement>("use_bar");
                    selectorUI.UI.SetEnabled(true);
                    AudioManager.playSound("menuavailable");
                    //play audio
                    battleData.isRecharging = false;
                }
            }).Run();
        
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        hasMoved = false;
    }
    public void ItemsButton(int characterNumber){
        AudioManager.playSound("menuchange");
        currentCharacterSelected = characterNumber;
        battleUI .visible = false;
        itemSelector.visible = true;

        DynamicBuffer<ItemData> itemInventory = EntityManager.GetBuffer<ItemData>(playerCharactersQuery.ToEntityArray(Allocator.Temp)[characterNumber]);
        int i = 0;
        int firstItemCheck = 0;
        ScrollView list1 = itemSelector.Q<ScrollView>("list1");
        ScrollView list2 = itemSelector.Q<ScrollView>("list2");
        list1.Clear();
        list1.Clear();
        list2.Clear();
        // changing the names of the items
        while(i < itemInventory.Length){
            int z = i;
            Item item = itemInventory[i].item;
            Button button = new Button();
            button.focusable = true;
            button.text = item.name.ToString();
            button.AddToClassList("item_button");
            button.clicked += () => ItemButton(z);
            button.RegisterCallback<FocusEvent>(ev => UpdateItemDescription(item.description.ToString()));
            button.RegisterCallback<PointerEnterEvent>(ev => UpdateItemDescription(item.description.ToString()));
            switch(itemInventory[i].item.itemType){
                case ItemType.none:
                    button.SetEnabled(false);
                break;
            }

            if(i < 6){
                list1.Add(button);
            }
            else{
                list2.Add(button);
            }
            if(i == firstItemCheck && item.itemType != ItemType.none){
                button.Focus();
                UpdateItemDescription(item.description.ToString());
            }
            else{
                firstItemCheck++;
                UpdateItemDescription("");
            }
            i++;
        }
        while(i < 10){
            Button button = new Button();
            button.text = "None";
            button.SetEnabled(false);
            i++;
        }
    }
    private void ItemButton(int itemNumber){
        AudioManager.playSound("menuchange");
        Entity playerEntity = playerCharactersQuery.ToEntityArray(Allocator.Temp)[currentCharacterSelected];
        DynamicBuffer<ItemData> itemInventory = EntityManager.GetBuffer<ItemData>(playerEntity);
        PlayerSelectorUI selectorUI = EntityManager.GetComponentObject<PlayerSelectorUI>(playerEntity);
        BattleData battleData = GetComponent<BattleData>(playerEntity);
        Item item = itemInventory[itemNumber].item;
        switch(item.itemType){
            case ItemType.healing:
                DynamicBuffer<HealingData> healings = GetBuffer<HealingData>(battleSystem.playerEntities[currentPlayer]);
                HealingData healing = new HealingData{healing = item.strength};
                healings.Add(healing);
            break;
        }
        battleData.useTime = item.useTime;
        battleData.maxUseTime = battleData.useTime;
        itemInventory.RemoveAt(itemNumber);

        battleUI.visible = true;
        itemSelector.visible = false;
    }
    private void SkillButton(int skillNumber){
        Entity playerEntity = playerCharactersQuery.ToEntityArray(Allocator.Temp)[currentCharacterSelected];
        DynamicBuffer<EquipedSkillData> equipedSkills = GetBuffer<EquipedSkillData>(playerEntity);
        CharacterStats characterStats = GetComponent<CharacterStats>(playerEntity);
        //checking if there have enough points to use the move
        if(characterStats.points >= equipedSkills[skillNumber].skill.cost){
            AudioManager.playSound("menuchange");
            int i = 0;
            previousUI = skillSelector;
            skillSelector.visible = false;
            enemySelector.visible = true;

            EntityQuery enemySelectorQuery = GetEntityQuery(typeof(EnemySelectorUI));
            NativeArray<Entity> enemySelectorEntities = enemySelectorQuery.ToEntityArray(Allocator.Temp);
            while(i < enemySelectorEntities.Length){
                int z = i;
                EnemySelectorUI enemySelectorUI = EntityManager.GetComponentObject<EnemySelectorUI>(enemySelectorEntities[i]);
                Button button = enemySelectorUI.enemySelectorUI.Q<Button>("Base");
                button.clicked += () => SkillEnemySelectButton(z, skillNumber, button);
                i++;
            }   
            enemySelectorEntities.Dispose();
        }
        
    }
    private void SkillEnemySelectButton(int enemyNumber, int currentSkill, Button selectButton){
        AudioManager.playSound("menuselect");
        battleUI.Focus();
        EntityQuery enemyUiSelectionGroup = GetEntityQuery(typeof(EnemySelectorUI), typeof(EnemySelectorData));
        NativeArray<Entity> enemyUiSelection = enemyUiSelectionGroup.ToEntityArray(Allocator.TempJob);
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((DynamicBuffer<ItemData> itemInventory, AnimationData animation, Animator animator, PlayerSelectorUI selectorUI,int entityInQueryIndex, ref BattleData battleData, ref CharacterStats characterStats, in Entity entity) =>{
            
            DynamicBuffer<EquipedSkillData> equipedSkills = GetBuffer<EquipedSkillData>(entity);
            Skill skill = equipedSkills[currentSkill].skill;
            if(characterStats.points >= skill.cost){
                characterStats.points -= skill.cost;
            }
            else{
                characterStats.points = 0;
            }
            
            animator.Play(animation.basicSwordAnimationName);
            AudioManager.playSound("swordswing");
            
            //deal damage to the enemy

            ecb.AddComponent(entity, new UsingSkillData{
                target = enemyUiSelection[currentEnemySelected],
                skill = skill
            });
            ecb.AddComponent<BasicSkillTag>(entity);

            // wait until you are recharged
            battleData.useTime = 0;
            battleData.maxUseTime = skill.waitTime;                            


            battleUI.visible = true;
            enemySelector.visible = false;
            isInEnemySelection = false;
        }).Run();
        selectButton.clicked -= () => SkillEnemySelectButton(enemyNumber, currentSkill, selectButton);

        enemyUiSelection.Dispose();
    }
    public void EnemySelectBack(){
        AudioManager.playSound("menuback");
        enemySelector.visible = false;
        previousUI.visible = true;
    }
    public void SkillsButton(int characterNumber){
        AudioManager.playSound("menuchange");
        currentCharacterSelected = characterNumber;
        skillSelector.visible = true;
        battleUI.visible = false;
        uISystem.ResetFocus();
        List<Button> skillButtons = new List<Button>();
        DynamicBuffer<EquipedSkillData> equipedSkills = GetBuffer<EquipedSkillData>(playerCharactersQuery.ToEntityArray(Allocator.Temp)[characterNumber]);
        int i = 0;
        ScrollView skillList = skillSelector.Q<ScrollView>("skill_list");
        skillList.Clear();
        while(i < equipedSkills.Length){
            int z = i;
            Button skillButton = new Button();
            skillButton.focusable = true;

            Skill skill = equipedSkills[i].skill;
            skillButton.text = skill.description.ToString();
            skillButton.clicked += () => SkillButton(z);
            skillButton.RegisterCallback<FocusEvent>(ev => UpdateSkillDescription(skill.description.ToString()));
            skillButton.RegisterCallback<PointerEnterEvent>(ev => UpdateSkillDescription(skill.description.ToString()));
            

            skillButton.AddToClassList("item_button");
            skillSelector.Q<Label>("skill_desc").text = equipedSkills[i].skill.description.ToString();
            skillList.Add(skillButton);
            skillButtons.Add(skillButton);
            if(i == 0){
                UpdateSkillDescription(skill.description.ToString());
                skillButton.Focus();
            }
            i++;
        }
    }
    private void UpdateSkillDescription(string description){
        skillSelector.Q<Label>("skill_desc").text = description;
    }
    private void UpdateItemDescription(string description){
        itemSelector.Q<Label>("item_desc").text = description;
    }
    public void ItemsBackButton(){
        AudioManager.playSound("menuback");
        itemSelector.visible = false;
        battleUI.visible = true;
    }
    public void SkillsBackButton(){
        AudioManager.playSound("menuback");
        skillSelector.visible = false;
        battleUI.visible = true;
    }
    public void AttackButton(int characterNumber){
        AudioManager.playSound("menuchange");
        previousUI = battleUI;
        battleUI.visible = false;
        enemySelector.visible = true;
        currentCharacterSelected = characterNumber;

        int i = 0;

        EntityQuery enemySelectorQuery = GetEntityQuery(typeof(EnemySelectorUI));
        NativeArray<Entity> enemySelectorEntities = enemySelectorQuery.ToEntityArray(Allocator.Temp);
        while(i < enemySelectorEntities.Length){
            int z = i;
            EnemySelectorUI enemySelectorUI = EntityManager.GetComponentObject<EnemySelectorUI>(enemySelectorEntities[i]);
            Button button = enemySelectorUI.enemySelectorUI.Q<Button>("Base");
            button.clicked += () => AttackEnemySelectButton(z, button);
            i++;
        }   
        enemySelectorEntities.Dispose();
    }
    private void AttackEnemySelectButton(int EnemyNumber, Button selectButton){
        uISystem.ResetFocus();
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();


        EntityQuery enemyUiSelectionGroup = GetEntityQuery(typeof(EnemySelectorUI), typeof(EnemySelectorData));
        NativeArray<Entity> enemyUiSelection = enemyUiSelectionGroup.ToEntityArray(Allocator.TempJob);
        int i = 0;
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((DynamicBuffer<ItemData> itemInventory, AnimationData animation, Animator animator, PlayerSelectorUI selectorUI,int entityInQueryIndex, ref BattleData battleData, ref CharacterStats characterStats, in Entity entity) =>{
            if(i == currentCharacterSelected){
                battleUI.visible = true;
                selectorUI.UI.SetEnabled(false);
                animator.Play(animation.basicSwordAnimationName);
                AudioManager.playSound("swordswing");
                //makes sure that nothing is selected
                foreach(Entity ent in enemyUiSelection){
                    EnemySelectorData selector = GetComponent<EnemySelectorData>(ent);
                    selector.isSelected = false;
                    EntityManager.SetComponentData(ent, selector);
                }
                switch(characterStats.equipedWeapon.weaponSkill.skillType){
                    case SkillType.Regular:
                        ecb.AddComponent(entity, new UsingSkillData{
                            target = enemyUiSelection[EnemyNumber],
                            skill = characterStats.equipedWeapon.weaponSkill
                        });
                        ecb.AddComponent<RegularAttackData>(entity);
                    break;
                }
                battleData.useTime = 0;
                battleData.maxUseTime = characterStats.equipedWeapon.useTime;                           

                battleUI.visible = true;
                enemySelector.visible = false;
            }
            i++;
        }).Run();

        selectButton.clicked -= () => AttackEnemySelectButton(EnemyNumber, selectButton);
        enemyUiSelection.Dispose();
        
        //deal damage to the enemy
        

        // wait until you are recharged
        
    }
    private void ResumeGameWorld_OnTransitionEnd(System.Object sender, System.EventArgs e){
        InputGatheringSystem.currentInput = CurrentInput.overworld;
        transitionSystem.OnTransitionEnd -= ResumeGameWorld_OnTransitionEnd;
        
        uISystem.overworldOverlay.visible = true;
    }
    private void DisableMenu_OnBattleEnd(System.Object sender, OnBattleEndEventArgs e){
        battleUI.visible = false;
        enemySelector.visible = false;
        skillSelector.visible = false;
        itemSelector.visible = false;
        hasBattleStarted = false;
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, PlayerSelectorUI selectorUI) =>{
            selectorUI.UI.SetEnabled(true);
            EntityManager.RemoveComponent<PlayerSelectorUI>(entity);
        }).Run();
        if(e.isPlayerVictor){
            transitionSystem.OnTransitionEnd += ResumeGameWorld_OnTransitionEnd;
        }
    }
    private void EnableMenu_OnTransitionEnd(System.Object sender, System.EventArgs e){
        if(!hasBattleStarted){
            // enables all the features of the menu
            Camera cam = GetEntityQuery(typeof(Camera)).ToComponentArray<Camera>()[0];
            float positionRatio = 1280.0f / cam.pixelWidth;

            VisualElement root = uISystem.root;
            battleUI = root.Q<VisualElement>("BattleUI");
            enemySelector = root.Q<VisualElement>("EnemySelector");
            skillSelector = root.Q<VisualElement>("skill_selector");
            itemSelector = root.Q<VisualElement>("item_selector");
            losingBackground = root.Q<VisualElement>("losing_screen");
            battleUI.visible = true;
            battleUI.Focus();
            // adding the selectorUI stuff to the players
            int i = 0;
            foreach(Entity entity in battleSystem.playerEntities){
                Translation translation = GetComponent<Translation>(entity);
                string tempstr = "character" + (i + 1).ToString();
                VisualElement currentCharacter = battleUI.Q<Button>(tempstr);
                EntityManager.AddComponentObject(entity, new PlayerSelectorUI { UI = currentCharacter, currentSelection = battleSelectables.fight, isSelected = false, isHovered = i == 0 });
            
                VisualElement newHeadsUpDisplay = overHeadUITemplate.CloneTree();
                root.Add(newHeadsUpDisplay);
                Vector3 camPo =  cam.WorldToScreenPoint(translation.Value);
                Vector2 uiPosition =  new Vector2(camPo.x * positionRatio, camPo.y * positionRatio);
                newHeadsUpDisplay.Q<VisualElement>("base").style.bottom = uiPosition.y;
                newHeadsUpDisplay.Q<VisualElement>("base").style.left = uiPosition.x;
                EntityManager.AddComponentObject(entity, new HeadsUpUIData{UI = newHeadsUpDisplay, messages = new List<Message>()});
                i++;
            } 
            i = 0;
            foreach(Entity entity in battleSystem.enemyEntities){
                int z = i;
                Translation translation = GetComponent<Translation>(entity);

                VisualElement newEnemySelectUI = enemySelectionUITemplate.CloneTree();
                //newEnemySelectUI.Q<Button>("Base").clicked += () => EnemySelectorButton(z);
                VisualElement newHeadsUpDisplay = overHeadUITemplate.CloneTree();
                root.Add(newHeadsUpDisplay);
                Vector3 camPo =  cam.WorldToScreenPoint(translation.Value);
                Vector2 uiPosition =  new Vector2(camPo.x * positionRatio, camPo.y * positionRatio);
                
                newHeadsUpDisplay.Q<VisualElement>("base").style.bottom = uiPosition.y;
                newHeadsUpDisplay.Q<VisualElement>("base").style.left = uiPosition.x;
                
                enemySelector.Add(newEnemySelectUI);
                CharacterStats characterStats = GetComponent<CharacterStats>(entity);
                EntityManager.AddComponentData(entity, new EnemySelectorData { enemyId = characterStats.id, isSelected = false });
                EntityManager.AddComponentObject(entity, new EnemySelectorUI{ enemySelectorUI = newEnemySelectUI});
                EntityManager.AddComponentObject(entity, new HeadsUpUIData{UI = newHeadsUpDisplay, messages = new List<Message>()});
                i++;
            }
            transitionSystem.OnTransitionEnd -= EnableMenu_OnTransitionEnd;
            hasBattleStarted = true;
        }
    }
    private void WaitForTransition_OnBattleStart(System.Object sender, System.EventArgs e){
        uISystem.overworldOverlay.visible = false;
        uISystem.ResetFocus();

        playerNumber = battleSystem.playerEntities.Count;
        transitionSystem.OnTransitionEnd += EnableMenu_OnTransitionEnd;
    }

    private void FinishVictoryData_OnDisplayFinished(object sender, System.EventArgs e){
        textBoxSystem.OnDisplayFinished -= FinishVictoryData_OnDisplayFinished;
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
        AudioManager.stopCurrentSong();
        if(e.isPlayerVictor){
            AudioManager.playSong(GetSingleton<OverworldAtmosphereData>().songName.ToString());
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
            textBoxSystem.OnDisplayFinished += FinishVictoryData_OnDisplayFinished;
        }
        else{
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithAll<BattleData>()
            .ForEach((ref BeforeBattleData beforeBattleData,in Translation translation, in Entity entity) =>
            {
                ecb.AddComponent(entity, new TransitionData{newPosition = beforeBattleData.previousLocation, oldPosition = translation.Value});
                ecb.RemoveComponent<BeforeBattleData>(entity);
                ecb.RemoveComponent<BattleData>(entity);
            }).Run();
        }
        
    }
    public void ContinueButton(){
        
        AudioManager.playSound("menuselect");
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
    public void LossReturnToTitleButton(){
        losingBackground.visible = false;
        uISystem.titleBackground.visible = true;
        AudioManager.playSound("menuselect");
        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
        sceneSystem.UnloadScene(SubSceneReferences.Instance.EssentialsSubScene.SceneGUID);
        AudioManager.playSong("menuMusic");
    }

    private void UnLoadScenes_OnTransitionnEnd(object sender, System.EventArgs e){
        transitionSystem.OnTransitionEnd -= UnLoadScenes_OnTransitionnEnd;
        saveAndLoadSystem.UnLoadSubScenes();
    }
    private void DisplayLoss_OnPlayerLoss(object sender, OnBattleEndEventArgs e){
        if(!e.isPlayerVictor){
            AudioManager.playSound("defeatsong");
            VisualElement losingBackground = uISystem.root.Q<VisualElement>("losing_screen");
            if(!File.Exists(Application.persistentDataPath + "/tempsave"+ "/SavePointData")){
                losingBackground.Q<Button>("continue").SetEnabled(false);
            }
            else{
                losingBackground.Q<Button>("continue").SetEnabled(true);
            }

            losingBackground.visible = true;
            transitionSystem.OnTransitionEnd += UnLoadScenes_OnTransitionnEnd;
        }
    }
}

