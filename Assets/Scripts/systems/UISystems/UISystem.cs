using Unity.Entities;
using Unity.Collections;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.IO;
using Unity.Scenes;
//[UpdateBefore(typeof(SaveTriggerSystem))]
public class UISystem : SystemBase
{
    bool setup = false;
    bool isInteractiveEnabled = false;
    Equipment switchChoice;
    int currentCharacter = 0;
    int currentItem = 1;
    public event StartGameEventHandler StartGame;
    public event EventHandler StartNewGame;
    SceneSystem sceneSystem;
    PauseSystem pauseSystem;
    public VisualElement root;
    public Button textBoxUI;
    VisualElement nullFocus;
    SaveAndLoadSystem saveAndLoadSystem;

    public VisualElement titleBackground;
    VisualElement creditsBackground;
    VisualElement fileSelectBackground;
    VisualElement settingsBackground;
    VisualElement pauseBackground;
    Button settingsBackButton;

    VisualElement itemsQuickMenu;
    VisualElement equipmentQuickMenu;
    VisualElement skillsQuickMenu;
    VisualElement equipmentInfo;
    VisualElement itemInfo;
    public VisualElement overworldOverlay;
    VisualElement skillInfo;
    EntityQuery caravanQuery;
    EntityQuery characterStatsQuery;
    OverWorldHealingSystem healingSystem;
    TextBoxSystem textBoxSystem;
    BattleMenuSystem battleMenuSystem;
    EntityQuery interactiveButtonQuery;
    VisualElement overworldSaveFileSelect;
    InkDisplaySystem inkDisplaySystem;
    protected override void OnCreate()
    {
        battleMenuSystem = World.GetOrCreateSystem<BattleMenuSystem>();
        interactiveButtonQuery = GetEntityQuery(typeof(InteractiveButtonData));
        healingSystem = World.GetOrCreateSystem<OverWorldHealingSystem>();
        textBoxSystem = World.GetOrCreateSystem<TextBoxSystem>();
        healingSystem.OnHealthChange += UpdateCharacterInfo_OnStatsUpdate;

        pauseSystem = World.GetOrCreateSystem<PauseSystem>();

        characterStatsQuery = GetEntityQuery(typeof(CharacterStats), typeof(PlayerTag));
        caravanQuery = GetEntityQuery(typeof(CaravanTag));
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        saveAndLoadSystem = World.GetOrCreateSystem<SaveAndLoadSystem>();
        inkDisplaySystem = World.GetOrCreateSystem<InkDisplaySystem>();
        
    }
    protected override void OnUpdate()
    {
        // sets up the ui
        Entities
        .WithoutBurst()
        .WithAll<UILoadTag>()
        .WithStructuralChanges()
        .ForEach((Entity entity, UIDocument UIDoc) =>{
            if(UIDoc.rootVisualElement != null){
                EntityManager.RemoveComponent<UILoadTag>(entity);
                setup = true;
                AudioManager.playSong("menuMusic");
                root = UIDoc.rootVisualElement;
                nullFocus = root.Q<VisualElement>("null_focus");
                // for the title screen
                {
                titleBackground = root.Q<VisualElement>("title_background");
                creditsBackground = root.Q<VisualElement>("credits_background");
                fileSelectBackground = root.Q<VisualElement>("file_select");

                titleBackground.Q<Button>("Start").clicked += StartButton;
                titleBackground.Q<Button>("Continue").clicked += continueButton;
                titleBackground.Q<Button>("Options").clicked += OptionsButton;
                titleBackground.Q<Button>("Credits").clicked += CreditsButton;
                titleBackground.Q<Button>("Exit").clicked += ExitButton;

                creditsBackground.Q<Button>("credits_back_button").clicked += CreditsBackButton;

                fileSelectBackground.Q<TemplateContainer>("save_file1").Q<Button>("background").clicked += () => ContinueGameButton(1);
                fileSelectBackground.Q<TemplateContainer>("save_file2").Q<Button>("background").clicked += () => ContinueGameButton(2);

                fileSelectBackground.Q<Button>("load_back_button").clicked += LoadSaveFileBackButton;
                }
                //for the credits menu, links all the items to the links
                {
                Button technoYoutubeButton = root.Q<Button>("techno_youtube");
                Button technoTwitterButton = root.Q<Button>("techno_twitter");

                Button tommyYoutubeButton = root.Q<Button>("tommy_youtube");
                Button tommyTwitterButton = root.Q<Button>("tommy_twitter");
                Button tommyTwitchButton = root.Q<Button>("tommy_twitch");

                Button wilburYoutubeButton = root.Q<Button>("wilbur_youtube");
                Button wilburTwitterButton = root.Q<Button>("wilbur_twitter");
                Button wilburTwitchButton = root.Q<Button>("wilbur_twitch");
                technoYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTechno(websites.youtube));
                technoTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTechno(websites.twitter));

                tommyYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.youtube));
                tommyTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.twitter));
                tommyTwitchButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.twitch));

                wilburYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.youtube));
                wilburTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.twitter));
                wilburTwitchButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.twitch));
                }
                // for the settings menu
                {
                settingsBackground = root.Q<VisualElement>("settings_background");

                settingsBackButton = settingsBackground.Q<Button>("settings_back_button");

                Button volumeButton = settingsBackground.Q<Button>("volume_button");
                volumeButton.RegisterCallback<FocusEvent>(ev => ActivateSettingsTab(settingsBackground.Q<VisualElement>("volume_controls")));
                Button bindingsButton = settingsBackground.Q<Button>("bindings_button");
                bindingsButton.SetEnabled(false);
                bindingsButton.RegisterCallback<FocusEvent>(ev => ActivateSettingsTab(settingsBackground.Q<VisualElement>("bindings_controls")));
                Button othersButton = settingsBackground.Q<Button>("others_button");
                othersButton.RegisterCallback<FocusEvent>(ev => ActivateSettingsTab(settingsBackground.Q<VisualElement>("other_controls")));
                settingsBackground.Q<Slider>("volume_slider").RegisterValueChangedCallback(ev => ChangeVolume(ev.newValue));
                settingsBackground.Q<Button>("title_return_button").clicked += SettingsReturnToTitleButton;
                }
                // for pause menu
                {
                pauseBackground = root.Q<VisualElement>("pause_background");

                pauseBackground.Q<Button>("pause_back_button").clicked += PauseBackButton;
                
                pauseBackground.Q<Button>("Equipment").clicked += EquipmentButton;
                pauseBackground.Q<Button>("Item").clicked += PauseItemsButton;
                pauseBackground.Q<Button>("Skills").clicked += PauseSkillsButton;
                pauseBackground.Q<Button>("Settings").clicked += PauseSettingsButton;

                itemsQuickMenu = pauseBackground.Q<VisualElement>("items_quickmenu");
                itemsQuickMenu.Q<Button>("use").clicked += ItemUse;
                itemsQuickMenu.Q<Button>("drop").clicked += ItemDrop;
                itemsQuickMenu.Q<Button>("cancel").clicked += ItemCancel;

                equipmentInfo = pauseBackground.Q<VisualElement>("equipment_info");
                itemInfo = pauseBackground.Q<VisualElement>("item_selection");
                skillInfo = pauseBackground.Q<VisualElement>("skill_selection");

                equipmentQuickMenu = pauseBackground.Q<VisualElement>("equipment_quickmenu");
                equipmentQuickMenu.Q<Button>("cancel").clicked += EquipmentCancel;

                skillsQuickMenu = pauseBackground.Q<VisualElement>("skills_quickmenu");
                //skillsQuickMenu.Q<Button>("switch").clicked += SkillSwitchButton;
                skillsQuickMenu.Q<Button>("cancel").clicked += SkillCancel;

                Button currentWeapon = equipmentInfo.Q<Button>("current_weapon");
                currentWeapon.clicked += () => CurrentEquipmentButton(currentWeapon, Equipment.Weapon);
                equipmentInfo.Q<Button>("current_armor").clicked += () => CurrentEquipmentButton(currentWeapon, Equipment.Armor);
                equipmentInfo.Q<Button>("current_charm").clicked += () => CurrentEquipmentButton(currentWeapon, Equipment.Charm);
                }
                // for overworld overlay
                {
                overworldOverlay = root.Q<VisualElement>("overworld_overlay");
                overworldOverlay.Q<Button>("pause_button").clicked += ActivatePauseMenu;

                overworldOverlay.Q<Button>("interactive_item_check").SetEnabled(false);
                overworldOverlay.Q<Button>("interactive_item_check").clicked += InteractButton;}
                // for battle menu
                {
                VisualElement battleBackground = root.Q<VisualElement>("battle_background");
                VisualElement battleUI = battleBackground.Q<VisualElement>("BattleUI");
                VisualElement losingBackground = root.Q<VisualElement>("losing_screen");
                losingBackground.Q<Button>("continue").clicked += battleMenuSystem.ContinueButton;
                losingBackground.Q<Button>("title").clicked += battleMenuSystem.LossReturnToTitleButton;
                
                // should be technoblade
                Button technobladeBattleUI = battleUI.Q<Button>("character1");
                technobladeBattleUI.Q<Button>("fight").clicked += () => battleMenuSystem.AttackButton(0);
                technobladeBattleUI.Q<Button>("skills").clicked += () => battleMenuSystem.SkillsButton(0);
                technobladeBattleUI.Q<Button>("items").clicked += () => battleMenuSystem.ItemsButton(0);

                battleBackground.Q<Button>("selector_back_button").clicked += battleMenuSystem.EnemySelectBack;
                battleBackground.Q<Button>("skills_back_button").clicked += battleMenuSystem.SkillsBackButton;
                battleBackground.Q<Button>("items_back_button").clicked += battleMenuSystem.ItemsBackButton;
                }
                overworldSaveFileSelect = root.Q<VisualElement>("overworld_file_select");
                TemplateContainer fileContainer1 = overworldSaveFileSelect.Q<TemplateContainer>("save_file1");
                fileContainer1.Q<Button>("background").clicked += () =>  saveAndLoadSystem.SaveGame(1);
                fileContainer1.Q<Button>("background").clicked += () =>  saveAndLoadSystem.UpdateSaveFile(fileContainer1.Q<Button>("background"), 1);

                TemplateContainer fileContainer2 = overworldSaveFileSelect.Q<TemplateContainer>("save_file2");
                fileContainer2.Q<Button>("background").clicked += () =>  saveAndLoadSystem.SaveGame(2);
                fileContainer2.Q<Button>("background").clicked += () =>  saveAndLoadSystem.UpdateSaveFile(fileContainer2.Q<Button>("background"), 2);
                overworldSaveFileSelect.Q<Button>("save_back_button").clicked += SaveBackButton;

                textBoxUI = root.Q<Button>("textbox");
                textBoxUI.clicked += inkDisplaySystem.ContinueText;
                
                EntityManager.RemoveComponent<UILoadTag>(entity);
            }
        }).Run();
        if(isInteractiveEnabled && setup){
            Button interactiveButton = overworldOverlay.Q<Button>("interactive_item_check");
            if(!interactiveButton.enabledSelf && interactiveButton.visible == true){
                AudioManager.playSound("menuavailable");
                interactiveButton.SetEnabled(true);
            }
        }
        else if(setup){
            Button interactiveButton = overworldOverlay.Q<Button>("interactive_item_check");
            interactiveButton.SetEnabled(false);
        }
        InteractiveButtonData interactiveButtonData = GetSingleton<InteractiveButtonData>();
        if(interactiveButtonData.isPressed){
            OverworldInputData tempinput = GetSingleton<OverworldInputData>();
            tempinput.select = true;
            SetSingleton<OverworldInputData>(tempinput);
        }
        interactiveButtonData.isPressed = false;
        SetSingleton(interactiveButtonData);
        isInteractiveEnabled = false;
    }
    public void ResetFocus(){
        nullFocus.Focus();
    }
    private void LoadSaveFileBackButton(){
        AudioManager.playSound("menuback");
        titleBackground.visible = true;
        fileSelectBackground.visible = false;
    }
    private void SaveBackButton(){
        pauseSystem.UnPause();
        textBoxUI.visible = false;
        InputGatheringSystem.currentInput = CurrentInput.overworld;
        overworldOverlay.visible = true;
        overworldSaveFileSelect.visible = false;
    }
    private void ChangeVolume(float newVolume){
        AudioListener.volume = newVolume;
    }
    private void InteractButton(){
        InteractiveButtonData interactiveButton = GetSingleton<InteractiveButtonData>();
        interactiveButton.isPressed = true;
        SetSingleton(interactiveButton);
    }
    public void EnableInteractive(){
        isInteractiveEnabled = true;
    }
    private void ActivatePauseMenu(){
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.Temp);
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);

        AudioManager.playSound("menuselect");
        pauseSystem.Pause();
        InputGatheringSystem.currentInput = CurrentInput.ui;
        overworldOverlay.visible = false;
        pauseBackground.visible = true;

        VisualElement currentCharacterUI = pauseBackground.Q<VisualElement>("character" + (currentCharacter + 1).ToString());

        Label healthBarText = currentCharacterUI.Q<Label>("health_text");
        VisualElement healthBarBase = currentCharacterUI.Q<VisualElement>("health_bar_base");
        VisualElement healthBar = currentCharacterUI.Q<VisualElement>("health_bar");

        VisualElement bloodBar = currentCharacterUI.Q<VisualElement>("blood_bar");
        VisualElement bloodBarBase = currentCharacterUI.Q<VisualElement>("blood_bar_base");
        Label bloodBarText = currentCharacterUI.Q<Label>("blood_text");

        Label currentLevel = currentCharacterUI.Q<Label>("current_level");
        Label currentAttack = currentCharacterUI.Q<Label>("current_attack");
        Label currentDefence = currentCharacterUI.Q<Label>("current_defence");
        Label neededEXP = currentCharacterUI.Q<Label>("EXP");

        


        CharacterStats characterStats = characterStatsList[currentCharacter];
        LevelData levelData = GetComponent<LevelData>(characterEntities[currentCharacter]);

        currentLevel.text = "LVL " + levelData.currentLVL.ToString();
        currentAttack.text = "ATK: " + (characterStats.baseStats.attack + characterStats.equipedWeapon.power).ToString();
        currentDefence.text = "DEF: " + (characterStats.baseStats.defense + characterStats.equipedArmor.defense).ToString();
        neededEXP.text = "EXP needed: " + (levelData.requiredEXP - levelData.currentEXP).ToString();

        healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
        healthBarText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();

        bloodBar.style.width = bloodBarBase.contentRect.width * (characterStats.points / characterStats.maxPoints);
        bloodBarText.text = "Blood: " + characterStats.points.ToString() + "/" + characterStats.maxPoints.ToString();



        characterEntities.Dispose();
    }
    public void StartButton(){
        titleBackground.visible = false;
        overworldOverlay.visible = true;
        SetSingleton<OverworldAtmosphereData>(new OverworldAtmosphereData{songName = "CalmMountain"});
        AudioManager.playSound("menuselect");
        StartNewGame?.Invoke(this, EventArgs.Empty);
        AudioManager.playSong("CalmMountain");
        InputGatheringSystem.currentInput = CurrentInput.overworld;
        sceneSystem.UnloadScene(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
        AudioManager.stopSong("menuMusic");
    }
    public void continueButton(){
        AudioManager.playSound("menuselect");
        saveAndLoadSystem.UpdateSaveFileUI(fileSelectBackground);                
                            
        fileSelectBackground.visible = true;
        titleBackground.visible = false;   
    }
    public void OptionsButton(){
        AudioManager.playSound("menuselect");

        settingsBackButton.clicked += ToTitleSettingsBackButton;

        titleBackground.visible = false;
        settingsBackground.visible = true;
        //settingsMenuSystem.ActivateMenu();
    }
    public void ToTitleSettingsBackButton(){
        AudioManager.playSound("menuback");
        DeActivateSettingsTabs();
        settingsBackground.visible = false;
        titleBackground.visible = true;
        titleBackground.Focus();
        settingsBackButton.clicked -= ToTitleSettingsBackButton;
    }
    public void CreditsButton(){
        AudioManager.playSound("menuchange");
        creditsBackground.visible = true;
        titleBackground.visible = false;
        creditsBackground.Q<Button>("credits_back_button").Focus();
    }
    public void ExitButton(){
        Application.Quit();
    }
    public void CreditsBackButton(){
        AudioManager.playSound("menuback");
        creditsBackground.visible = false;
        titleBackground.visible = true;
    }
    public void ContinueGameButton(int saveFileNubmer){
        overworldOverlay.visible = true;
        titleBackground.visible = false;
        fileSelectBackground.visible = false;

        StartGame?.Invoke(this, new StartGameEventArgs{saveFileNumber = saveFileNubmer});
        AudioManager.playSound("menuselect");
        InputGatheringSystem.currentInput = CurrentInput.overworld;
        sceneSystem.UnloadScene(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
        AudioManager.stopSong("menuMusic");
    }
    public void ActivateSettingsTab(VisualElement tab){
        AudioManager.playSound("menuchange");
        DeActivateSettingsTabs();
        tab.visible = true;
    }
    public void DeActivateSettingsTabs(){
        settingsBackground.Q<VisualElement>("volume_controls").visible = false;
        settingsBackground.Q<VisualElement>("bindings_controls").visible = false;
        settingsBackground.Q<VisualElement>("other_controls").visible = false;
    }
    public void EquipmentButton(){
        RestartPauseMenu();
        AudioManager.playSound("menuselect");
            if(true){
                NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.Temp);

                VisualElement otherEquipmentBase = equipmentInfo.Q<VisualElement>("other_equipment");
                VisualElement currentEquipment= equipmentInfo.Q<VisualElement>("current_equipment");
                Button currentWeaponLabel = equipmentInfo.Q<Button>("current_weapon");
                Button currentArmorLabel = equipmentInfo.Q<Button>("current_armor");
                Button currentCharmLabel = equipmentInfo.Q<Button>("current_charm");
                Label equipmentDesc = equipmentInfo.Q<Label>("equipment_text");

                Entity caravan = caravanQuery.GetSingletonEntity();
                DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
                DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
                DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);

                currentWeaponLabel.text = "Weapon: " + characterStatsList[currentCharacter].equipedWeapon.name;
                currentArmorLabel.text = "Armor: " + characterStatsList[currentCharacter].equipedArmor.name;
                currentCharmLabel.text = "Charm: " + characterStatsList[currentCharacter].equipedCharm.name;
                currentEquipment.visible = true;
                equipmentDesc.text = characterStatsList[currentCharacter].equipedWeapon.description.ToString();
                equipmentInfo.visible = true;
                characterStatsList.Dispose();

                equipmentInfo.Q<Button>("current_weapon").Focus();
            }
            else{
            }
    }
    private void CurrentEquipmentButton(Button equipmentButton, Equipment equipmentType){
        equipmentQuickMenu.visible = true;
        placeQuickMenu(equipmentButton, equipmentQuickMenu);
        switch(equipmentType){
            case Equipment.Weapon:
                equipmentQuickMenu.Q<Button>("switch").clicked += WeaponSwitchButton;
            break;
            case Equipment.Armor:
                equipmentQuickMenu.Q<Button>("switch").clicked += ArmorSwitchButton;
            break;
            case Equipment.Charm:
                equipmentQuickMenu.Q<Button>("switch").clicked += CharmSwitchButton;
            break;
        }
    }
    private void EquipmentCancel(){
        AudioManager.playSound("menuback");
        equipmentQuickMenu.visible = false;
    }
    private void ArmorSwitchButton(){
        equipmentQuickMenu.Q<Button>("switch").clicked -= WeaponSwitchButton;
        DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravanQuery.GetSingletonEntity());

        var otherEquipmentBase = equipmentInfo.Q<VisualElement>("other_equipment");
        var currentEquipment = equipmentInfo.Q<VisualElement>("current_equipment");
        var equipmentDesc = equipmentInfo.Q<Label>("equipment_text");
        var otherEquipmentList = otherEquipmentBase.Q<ScrollView>("equipment_list");
        otherEquipmentList.Clear();
        AudioManager.playSound("menuselect");
        // makes the new equipment visable and the current equipment invisable
        otherEquipmentBase.visible = true;
        currentEquipment.visible = false;
        // reseting the switch item
        equipmentQuickMenu.visible = false;

        // show all available equipment switching what to display depending on what is selected
        for(int i = 0; i < armorInventory.Length; i++){
            int z = i;
            Armor armor = armorInventory[i].armor;
            Button newButton = new Button();
            newButton.clicked += () => SwitchArmor(z);
            newButton.text = armor.name.ToString();
            newButton.AddToClassList("item_button");
            otherEquipmentList.Add(newButton); 
            if(i == 0){
                equipmentDesc.text = armor.description.ToString();
            }
        }
    }
    private void SwitchArmor(int newArmorNumber){
        DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravanQuery.GetSingletonEntity());
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.Temp);
        var otherEquipmentBase = equipmentInfo.Q<VisualElement>("other_equipment");
        var currentEquipment = equipmentInfo.Q<VisualElement>("current_equipment");

        CharacterStats characterStats = GetComponent<CharacterStats>(characterEntities[currentCharacter]);
        ArmorData unEquipedArmor = new ArmorData{armor = characterStatsList[currentCharacter].equipedArmor};
        characterStats.equipedArmor = armorInventory[newArmorNumber].armor;
        // move the unequiped item to the weaponinventory
        armorInventory.RemoveAt(newArmorNumber);
        armorInventory.Insert(0, unEquipedArmor);
        
        EntityManager.SetComponentData(characterEntities[currentCharacter], characterStats);
        // may need to set the data of the characterStats
        // update equipment info
        //equipmentDesc.text = characterStats.equipedWeapon.description.ToString();
        equipmentInfo.Q<Button>("current_armor").text = "Armor: " + characterStats.equipedArmor.name.ToString();

        AudioManager.playSound("menuselect");
        currentEquipment.visible = true;
        otherEquipmentBase.visible = false;
        characterEntities.Dispose();
    }
    private void CharmSwitchButton(){
        equipmentQuickMenu.Q<Button>("switch").clicked -= WeaponSwitchButton;
        DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravanQuery.GetSingletonEntity());

        var otherEquipmentBase = equipmentInfo.Q<VisualElement>("other_equipment");
        var currentEquipment = equipmentInfo.Q<VisualElement>("current_equipment");
        var equipmentDesc = equipmentInfo.Q<Label>("equipment_text");
        var otherEquipmentList = otherEquipmentBase.Q<ScrollView>("equipment_list");
        otherEquipmentList.Clear();
        AudioManager.playSound("menuselect");
        // makes the new equipment visable and the current equipment invisable
        otherEquipmentBase.visible = true;
        currentEquipment.visible = false;
        // reseting the switch item
        equipmentQuickMenu.visible = false;

        // show all available equipment switching what to display depending on what is selected
        for(int i = 0; i < charmInventory.Length; i++){
            int z = i;
            Charm charm = charmInventory[i].charm;
            Button newButton = new Button();
            newButton.clicked += () => SwitchCharm(z);
            newButton.text = charm.name.ToString();
            newButton.AddToClassList("item_button");
            otherEquipmentList.Add(newButton); 
            if(i == 0){
                equipmentDesc.text = charm.description.ToString();
            }
        }
    }
    private void SwitchCharm(int newCharmNumber){
        DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravanQuery.GetSingletonEntity());
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.Temp);
        var otherEquipmentBase = equipmentInfo.Q<VisualElement>("other_equipment");
        var currentEquipment = equipmentInfo.Q<VisualElement>("current_equipment");

        CharacterStats characterStats = GetComponent<CharacterStats>(characterEntities[currentCharacter]);
        CharmData unEquipedArmor = new CharmData{charm = characterStatsList[currentCharacter].equipedCharm};
        characterStats.equipedCharm = charmInventory[newCharmNumber].charm;
        // move the unequiped item to the weaponinventory
        charmInventory.RemoveAt(newCharmNumber);
        charmInventory.Insert(0, unEquipedArmor);
        
        EntityManager.SetComponentData(characterEntities[currentCharacter], characterStats);
        // may need to set the data of the characterStats
        // update equipment info
        //equipmentDesc.text = characterStats.equipedWeapon.description.ToString();
        equipmentInfo.Q<Button>("current_charm").text = "Charm: " + characterStats.equipedCharm.name.ToString();

        AudioManager.playSound("menuselect");
        currentEquipment.visible = true;
        otherEquipmentBase.visible = false;
        characterEntities.Dispose();
    }
    private void WeaponSwitchButton(){
        equipmentQuickMenu.Q<Button>("switch").clicked -= WeaponSwitchButton;
        DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravanQuery.GetSingletonEntity());

        var otherEquipmentBase = equipmentInfo.Q<VisualElement>("other_equipment");
        var currentEquipment = equipmentInfo.Q<VisualElement>("current_equipment");
        var equipmentDesc = equipmentInfo.Q<Label>("equipment_text");
        var otherEquipmentList = otherEquipmentBase.Q<ScrollView>("equipment_list");
        otherEquipmentList.Clear();
        AudioManager.playSound("menuselect");
        // makes the new equipment visable and the current equipment invisable
        otherEquipmentBase.visible = true;
        currentEquipment.visible = false;
        // reseting the switch item
        equipmentQuickMenu.visible = false;

        // show all available equipment switching what to display depending on what is selected
        for(int i = 0; i < weaponInventory.Length; i++){
            int z = i;
            Weapon weapon = weaponInventory[i].weapon;
            Button newButton = new Button();
            newButton.clicked += () => SwitchWeapon(z);
            newButton.text = weapon.name.ToString();
            newButton.AddToClassList("item_button");
            otherEquipmentList.Add(newButton); 
            if(i == 0){
                equipmentDesc.text = weapon.description.ToString();
            }
        }
            
    }
    private void SwitchWeapon(int newWeaponNumber){
        DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravanQuery.GetSingletonEntity());
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.Temp);
        var otherEquipmentBase = equipmentInfo.Q<VisualElement>("other_equipment");
        var currentEquipment = equipmentInfo.Q<VisualElement>("current_equipment");

        CharacterStats characterStats = GetComponent<CharacterStats>(characterEntities[currentCharacter]);
        WeaponData unEquipedWeapon = new WeaponData{weapon = characterStatsList[currentCharacter].equipedWeapon};
        characterStats.equipedWeapon = weaponInventory[newWeaponNumber].weapon;
        // move the unequiped item to the weaponinventory
        weaponInventory.RemoveAt(newWeaponNumber);
        weaponInventory.Insert(0, unEquipedWeapon);
        
        EntityManager.SetComponentData(characterEntities[currentCharacter], characterStats);
        // may need to set the data of the characterStats
        // update equipment info
        //equipmentDesc.text = characterStats.equipedWeapon.description.ToString();
        equipmentInfo.Q<Button>("current_weapon").text = "Weapon: " + characterStats.equipedWeapon.name.ToString();

        AudioManager.playSound("menuselect");
        currentEquipment.visible = true;
        otherEquipmentBase.visible = false;
        characterEntities.Dispose();
    }
    public void PauseItemsButton(){
        RestartPauseMenu();
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);

        itemInfo.visible = true;
        ScrollView itemList = itemInfo.Q<ScrollView>("item_list");
        itemList.Clear();
        Label itemDesc = itemInfo.Q<Label>("item_desc");
        AudioManager.playSound("menuselect");
        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(GetSingletonEntity<CaravanTag>());
        itemList.Focus();
        int i = 0;
        foreach(ItemData itemData in itemInventory){

            Item item = itemData.item;
            if(item.name == ""){
                int z = i;
                Button itemButton = new Button();
                itemButton.focusable = true;
                itemButton.name = "item" + (z + 1).ToString();
                itemButton.text = "No Name";
                itemButton.AddToClassList("item_button");
                itemList.Add(itemButton);
            }
            else{
                int z = i;
                Button itemButton = new Button();
                itemButton.focusable = true;
                itemButton.name = "item" + (z + 1).ToString();
                itemButton.text = item.name.ConvertToString();
                itemButton.AddToClassList("item_button");
                itemButton.clicked += () => PauseItemButton(z + 1);
                // once an item is in focus, change the description accordingly
                string descriptionToShow = itemInventory[z].item.description.ToString() == "" ? "no description" : itemInventory[z].item.description.ToString();
                itemButton.RegisterCallback<FocusEvent>(ev => UpdateDescription(itemDesc, descriptionToShow));
                itemList.Add(itemButton);
            }
            i++;
        }
        string itemDescriptionToShow = itemInventory.Length > 0 ? itemInventory[0].item.description.ToString() : "";
        if(itemDescriptionToShow == ""){
            itemDesc.text = "no description";
        }
        else{
            itemDesc.text = itemDescriptionToShow;
        }
        characterEntities.Dispose();
    }
    private void UpdateDescription(Label label, string desc){
        Debug.Log("updating");
        label.text = desc;
    }
    public void PauseSkillsButton(){
        RestartPauseMenu();
        skillInfo.visible = true;
        VisualElement currentSkills = skillInfo.Q<VisualElement>("current_skills");
        ScrollView skillList = skillInfo.Q<ScrollView>("skill_list");
        Label skillDesc = skillInfo.Q<Label>("skill_desc");

        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        DynamicBuffer<PolySkillData> equipedSkills = GetBuffer<PolySkillData>(characterEntities[0]);

        AudioManager.playSound("menuselect");
        currentSkills.visible = true;
        int i = 1;
        foreach(PolySkillData skillData in equipedSkills){
            PolySkillData skill = skillData;
            Button skillButton = new Button();
            skillButton.AddToClassList("item_button");
            skillButton.name = "skill" + i.ToString();
            skillButton.focusable = true;
            if(skill.SharedSkillData.name == ""){
                skillButton.text = "None";
            }
            else{
                skillButton.text = skill.SharedSkillData.name.ToString();
            }
            // for some reason it tracks the value until its not used, so I used a value that wont change
            int z = i;
            skillButton.clicked += () => CurrentSkillButton(z);
            skillButton.RegisterCallback<FocusEvent>(ev => UpdateDescription(skillDesc, skill.SharedSkillData.description.ToString()));
            skillList.Add(skillButton);
            i++;
        }

        // sets the description to the first skill
        string descToDisplay = equipedSkills[0].SharedSkillData.description.ToString();
        if(descToDisplay == ""){
            skillDesc.text = "No description";
        }
        else{
            skillDesc.text = descToDisplay;
        }
        characterEntities.Dispose();
    }
    private void CurrentSkillButton(int currentSkillNumber){
        currentItem = currentSkillNumber;
        var currentSkills = skillInfo.Q<VisualElement>("current_skills");
        AudioManager.playSound("menuselect");
                                                
        //skillsQuickMenu.visible = true;
        //placeQuickMenu(currentSkills.Q<Button>("skill" + currentItem), skillsQuickMenu);
    }
    private void SkillCancel(){
        AudioManager.playSound("menuback");
        skillsQuickMenu.visible = false;
    }
    public void PauseSettingsButton(){
        AudioManager.playSound("menuselect");
        //select the ui
        settingsBackground.visible = true;
        settingsBackButton.clicked += ReturnToPauseMenu;
        pauseBackground.visible = false;

        RestartPauseMenu();
    }
    public void ReturnToPauseMenu(){
        DeActivateSettingsTabs();
        settingsBackground.visible = false;
        pauseBackground.visible = true;
        settingsBackButton.clicked -= ReturnToPauseMenu;
    }
    private void PauseBackButton(){
        overworldOverlay.visible = true;
        RestartPauseMenu();

        AudioManager.playSound("menuback");
        pauseSystem.UnPause();
        pauseBackground.visible = false;
        InputGatheringSystem.currentInput = CurrentInput.overworld;
    }
    public void RestartPauseMenu(){
        skillInfo.visible = false;
        equipmentInfo.visible = false;
        itemInfo.visible = false;
        equipmentInfo.Q<VisualElement>("current_equipment").visible = false;
        equipmentInfo.Q<VisualElement>("other_equipment").visible = false;
        skillsQuickMenu.visible = false;
        equipmentQuickMenu.visible = false;
        itemsQuickMenu.visible = false;

        skillInfo.Q<VisualElement>("current_skills").visible = false;
    }
    public void PauseItemButton(int itemNumber){
        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(GetSingletonEntity<CaravanTag>());
        ScrollView itemList = itemInfo.Q<ScrollView>("item_list");
        currentItem = itemNumber;
        AudioManager.playSound("menuchange");
        itemsQuickMenu.visible = true;

        placeQuickMenu(itemList.Q<Button>("item" + itemNumber.ToString()), itemsQuickMenu);
        //all items will have these features
        itemsQuickMenu.Q<Button>("give").SetEnabled(false);
        //add the useable selectables based on the item type
        switch(itemInventory[itemNumber - 1].item.itemType){
            case ItemType.healing:
                itemsQuickMenu.Q<Button>("use").SetEnabled(true);
                itemsQuickMenu.Q<Button>("use").Focus();
            break;
            case ItemType.none:
                itemsQuickMenu.Q<Button>("use").SetEnabled(false);
                itemsQuickMenu.Q<Button>("cancel").Focus();
            break;
        }

    }
    private void placeQuickMenu(Button item, VisualElement quickMenu){
        quickMenu.style.right = item.contentRect.width + item.worldBound.x;
        quickMenu.style.top = item.worldBound.y;
    }
    private void SettingsReturnToTitleButton(){
        AudioManager.playSound("menuback");
        DeActivateSettingsTabs();
        settingsBackButton.clicked -= ToTitleSettingsBackButton;
        settingsBackButton.clicked -= ReturnToPauseMenu;

        
        settingsBackground.visible = false;
        titleBackground.visible = true;
        saveAndLoadSystem.UnLoadSubScenes();
        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
        
        sceneSystem.UnloadScene(SubSceneReferences.Instance.EssentialsSubScene.SceneGUID);
        AudioManager.playSong("menuMusic");
    }
    private void ItemUse(){
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.Temp);
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(GetSingletonEntity<CaravanTag>());
        DynamicBuffer<HealingData> healings = GetBuffer<HealingData>(characterEntities[currentCharacter]);

        HealingData healing = new HealingData{healing = itemInventory[currentItem - 1].item.healingAmount};
        healings.Add(healing);

        itemInventory.RemoveAt(currentItem - 1);
        //exit items menu
        itemsQuickMenu.visible = false;

        itemInfo.visible = false;
        // updating the character health
        VisualElement currentCharacterUI = pauseBackground.Q<VisualElement>("character" + (currentCharacter + 1).ToString());
        Label healthBarText = currentCharacterUI.Q<Label>("health_text");
        VisualElement healthBarBase = currentCharacterUI.Q<VisualElement>("health_bar_base");
        VisualElement healthBar = currentCharacterUI.Q<VisualElement>("health_bar");
        VisualElement bloodBar = currentCharacterUI.Q<VisualElement>("blood");
        CharacterStats characterStats = characterStatsList[currentCharacter];
        healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
        healthBarText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();

        characterStatsList.Dispose();
        characterEntities.Dispose();
    }
    private void ItemCancel(){
        AudioManager.playSound("menuback");
        itemsQuickMenu.visible = false;
    } 
    private void ItemDrop(){
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(GetSingletonEntity<CaravanTag>());
        itemInventory.RemoveAt(currentItem - 1);
        //exit items menu
        itemsQuickMenu.visible = false;
        itemInfo.visible = false;

        characterEntities.Dispose();
    }
    private void UpdateCharacterInfo_OnStatsUpdate(System.Object sender, System.EventArgs e){
        // will only work while there is one character, will need to add a loop for all player characters
        EntityQuery characterStatsQuery = GetEntityQuery(typeof(CharacterStats), typeof(PlayerTag));
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
        VisualElement currentCharacterUI = pauseBackground.Q<VisualElement>("character" + (currentCharacter + 1).ToString());
        Label healthBarText = currentCharacterUI.Q<Label>("health_text");
        VisualElement healthBarBase = currentCharacterUI.Q<VisualElement>("health_bar_base");
        VisualElement healthBar = currentCharacterUI.Q<VisualElement>("health_bar");
        //VisualElement bloodBar = currentCharacterUI.Q<VisualElement>("blood");
        CharacterStats characterStats = characterStatsList[currentCharacter];
        healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
        healthBarText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();
        characterStatsList.Dispose();
    }
    public delegate void StartGameEventHandler(object sender, StartGameEventArgs e);
}
public class StartGameEventArgs: EventArgs{
    public int saveFileNumber{get; set;}
}
public enum Equipment{
    Weapon,
    Armor,
    Charm
}