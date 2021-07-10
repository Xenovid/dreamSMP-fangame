using Unity.Entities;
using Unity.Collections;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.IO;
using Unity.Scenes;
[UpdateBefore(typeof(SaveTriggerSystem))]
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
                skillsQuickMenu.Q<Button>("switch").clicked += SkillSwitchButton;
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
                textBoxUI.clicked += textBoxSystem.ContinueText;
                
                EntityManager.RemoveComponent<UILoadTag>(entity);
            }
        }).Run();
        if(isInteractiveEnabled && setup &&!textBoxSystem.isDisplaying){
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
        if(!textBoxSystem.isDisplaying){
            InteractiveButtonData interactiveButton = GetSingleton<InteractiveButtonData>();
            interactiveButton.isPressed = true;
            SetSingleton(interactiveButton);
        }
    }
    public void EnableInteractive(){
        isInteractiveEnabled = true;
    }
    private void ActivatePauseMenu(){
        AudioManager.playSound("menuselect");
        pauseSystem.Pause();
        InputGatheringSystem.currentInput = CurrentInput.ui;
        overworldOverlay.visible = false;
        pauseBackground.visible = true;
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
        VisualElement itemList = itemInfo.Q<VisualElement>("item_list");
        Label itemDesc = itemInfo.Q<Label>("item_desc");
        AudioManager.playSound("menuselect");
        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(characterEntities[0]);
        itemList.Q<Button>("item1").Focus();
        int i = 0;
        foreach(ItemData itemData in itemInventory){
            Item item = itemData.item;
            if(item.name == ""){
                itemList.Q<Button>("item" + (i + 1).ToString()).text = "None";
            }
            else{
                int z = i;
                Button currentItemButton = itemList.Q<Button>("item" + (i + 1).ToString());
                currentItemButton.text = item.name.ConvertToString();
                currentItemButton.clicked += () => PauseItemButton(z + 1);
                // once an item is in focus, change the description accordingly
                string descriptionToShow = itemInventory[z].item.description.ToString() == "" ? "no description" : itemInventory[0].item.description.ToString();
                currentItemButton.RegisterCallback<FocusEvent>(ev => UpdateDescription(itemDesc, descriptionToShow));
            }
            i++;
        }
        while(i < 10){
            itemList.Q<Button>("item" + (i + 1).ToString()).text = "None";
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
        Label skillDesc = skillInfo.Q<Label>("skill_desc");

        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        DynamicBuffer<EquipedSkillData> equipedSkills = GetBuffer<EquipedSkillData>(characterEntities[0]);

        AudioManager.playSound("menuselect");
        currentSkills.visible = true;
        currentSkills.Q<Button>("skill1").Focus();
        int i = 1;
        foreach(EquipedSkillData skillData in equipedSkills){
            Skill skill = skillData.skill;
            if(skill.name == ""){
                currentSkills.Q<Button>("skill" + i.ToString()).text = "None";
            }
            else{
                currentSkills.Q<Button>("skill" + i.ToString()).text = skill.name.ToString();
            }
            // for some reason it tracks the value until its not used, so I used a value that wont change
            int z = i;
            currentSkills.Q<Button>("skill" + i.ToString()).clicked += () => CurrentSkillButton(z);
            i++;
        }
        while(i <= 5){
            int z = i;
            currentSkills.Q<Button>("skill" + i.ToString()).text = "None";
            currentSkills.Q<Button>("skill" + i.ToString()).clicked += () => CurrentSkillButton(z);
            i++;
        }

        string descToDisplay = equipedSkills[0].skill.description.ToString();
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
                                                
        skillsQuickMenu.visible = true;
        placeQuickMenu(currentSkills.Q<Button>("skill" + currentItem), skillsQuickMenu);
    }
    private void SkillSwitchButton(){
        // make sure all ui have the right visability
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);

        DynamicBuffer<SkillData> skills = GetBuffer<SkillData>(characterEntities[currentCharacter]);
        VisualElement currentSkills = skillInfo.Q<VisualElement>("current_skills");
        VisualElement otherSkills = skillInfo.Q<VisualElement>("other_skills");
        Label skillDesc = skillInfo.Q<Label>("skill_desc");
        skillsQuickMenu.visible = false;
        currentSkills.visible = false;
        otherSkills.visible = true;
        // can't switch unless there is at least one skill, who's description should be displayed
        skillDesc.text = skills[0].skill.description.ToString();

        ScrollView skillList = otherSkills.Q<ScrollView>("other_skills_list");
        skillList.Clear();
        // set up all of the other skills
        for(int i = 1 ; i < skills.Length; i++){
            int z = i;
            Button newButton = new Button();
            newButton.AddToClassList("item_button");
            newButton.clicked += () => SwitchSkill(z);
            if(i <= skills.Length){
               newButton.text = skills[i].skill.name.ToString();
            }
            else{
                newButton.text = "None";
            }
            skillList.Add(newButton);
            
        }
        characterEntities.Dispose();
    }
    private void SkillCancel(){
        AudioManager.playSound("menuback");
        skillsQuickMenu.visible = false;
    }
    private void SwitchSkill(int newSkillNumber){
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        DynamicBuffer<SkillData> skills = GetBuffer<SkillData>(characterEntities[currentCharacter]);
        DynamicBuffer<EquipedSkillData> equipedSkills = GetBuffer<EquipedSkillData>(characterEntities[currentCharacter]);
        VisualElement otherSkills = skillInfo.Q<VisualElement>("other_skills");
        VisualElement currentSkills = skillInfo.Q<VisualElement>("current_skills");
        AudioManager.playSound("menuselect");
        //switchskill
        EquipedSkillData newSkill = new EquipedSkillData{skill = skills[newSkillNumber - 1].skill};
        if(currentItem > equipedSkills.Length){
            // nothing equiped, so nothing to add to the unequiped skills
            skills.RemoveAt(newSkillNumber - 1);

        }
        else{
            SkillData currentSkill = new SkillData{skill = equipedSkills[currentItem - 1].skill};

            equipedSkills.RemoveAt(currentItem - 1);
            skills.RemoveAt(newSkillNumber - 1);
            skills.Add(currentSkill);
        }
        equipedSkills.Insert(currentItem -1, newSkill);

        currentSkills.Q<Button>("skill" + currentItem.ToString()).text = newSkill.skill.name.ToString();

        currentSkills.visible = true;
        otherSkills.visible = false;
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
        skillInfo.Q<VisualElement>("other_skills").visible = false;
    }
    public void PauseItemButton(int itemNumber){
        NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(characterEntities[currentCharacter]);
        VisualElement itemList = itemInfo.Q<VisualElement>("item_list");
        currentItem = itemNumber;

        if(itemNumber <= itemInventory.Length){
            AudioManager.playSound("menuchange");
            itemsQuickMenu.visible = true;
            placeQuickMenu(itemList.Q<Button>("item" + itemNumber), itemsQuickMenu);
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

        characterEntities.Dispose();
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
        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(characterEntities[currentCharacter]);
        DynamicBuffer<HealingData> healings = GetBuffer<HealingData>(characterEntities[currentCharacter]);

        HealingData healing = new HealingData{healing = itemInventory[currentItem - 1].item.strength};
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
        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(characterEntities[currentCharacter]);
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