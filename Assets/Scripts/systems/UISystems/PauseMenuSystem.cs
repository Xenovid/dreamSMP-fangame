using Unity.Entities;
using UnityEngine.UIElements;
using Unity.Scenes;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental;
using Unity.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuSystem : SystemBase
{
      private PauseMenuSelectables currentSelection;
      private SceneSystem sceneSystem;
      private SettingsMenuSystem settingsMenuSystem;
      private OverWorldHealingSystem healingSystem;
      
      private bool isPaused;

      private int currentCharacter;
      private int currentSwitchItem;
      private bool isSelected;
      private bool onCharacterSelect;
      private bool onQuickMenu;
      private QuickMenuSelectables currentQuickMenuSelection;
      private int currentItem;
      private bool onItemSwitch;
      private List<QuickMenuSelectables> useableQuickSelectables = new List<QuickMenuSelectables>();
      private Equipment selectedEquipment;

      private bool canQuickSwitch;
      private bool canQuickGive;
      private bool canQuickDrop;
      private bool canQuickUse;
      PauseSystem pauseSystem;

      protected override void OnStartRunning()
      {
          pauseSystem = World.GetOrCreateSystem<PauseSystem>();
            base.OnStartRunning();
            sceneSystem = World.GetOrCreateSystem<SceneSystem>();
            settingsMenuSystem = World.GetOrCreateSystem<SettingsMenuSystem>();
            healingSystem = World.GetOrCreateSystem<OverWorldHealingSystem>();
            settingsMenuSystem.OnTitleReturn += DisableMenu_OnTitleReturn;
            currentCharacter = 1;
            currentItem = 1;

            currentSelection = PauseMenuSelectables.Party;
      }

      protected override void OnUpdate()
      {
        
        

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((in UIDocument UIDoc,in PauseMenuTag pausMenuTag) =>{
                VisualElement root = UIDoc.rootVisualElement;
                EntityQuery overworldInputQuery = GetEntityQuery(typeof(OverworldInputData));
                OverworldInputData overworldInput = overworldInputQuery.GetSingleton<OverworldInputData>();

                EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
                UIInputData uiInput = uiInputQuery.GetSingleton<UIInputData>();

                DynamicBuffer<PlayerPartyData> playerParty = GetBuffer<PlayerPartyData>(GetSingletonEntity<PlayerTag>());

                EntityQuery characterStatsQuery = GetEntityQuery(typeof(CharacterStats), typeof(PlayerTag));
                NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
                NativeArray<Entity> characterEntities = characterStatsQuery.ToEntityArray(Allocator.TempJob);


                EntityQuery caravanQuery = GetEntityQuery(typeof(CaravanTag));
                Entity caravan = caravanQuery.GetSingletonEntity();
                DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
                DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
                DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);
                  
                  
                  
                  if(root == null){

                  }
                  else{
                    VisualElement pauseBackground = root.Q<VisualElement>("pause_background");

                    if(!isPaused && overworldInput.escape){
                        healingSystem.OnHealthChange += UpdateCharacterInfo_OnStatsUpdate;
                        isPaused = true;
                        InputGatheringSystem.currentInput = CurrentInput.ui;
                        pauseBackground.visible = true;
                            //**** need to update character stats
                        foreach(CharacterStats characterStats in characterStatsList)
                        {
                            int i = 0;
                            foreach(PlayerPartyData player in playerParty)
                            {
                                if(player.playerId == characterStats.id)
                                {
                                    VisualElement currentCharacter = pauseBackground.Q<VisualElement>("character" + (i + 1).ToString());
                                    Label healthBarText = currentCharacter.Q<Label>("health_text");
                                    VisualElement healthBarBase = currentCharacter.Q<VisualElement>("health_bar_base");
                                    VisualElement healthBar = currentCharacter.Q<VisualElement>("health_bar");

                                    VisualElement bloodBarBase = currentCharacter.Q<VisualElement>("blood");
                                    Label bloodBarText = bloodBarBase.Q<Label>("blood_text");
                                    VisualElement bloodBar = bloodBarBase.Q<VisualElement>("blood_bar");

                                    bloodBar.style.width = bloodBarBase.contentRect.width * ((float)characterStats.points/ characterStats.maxPoints);
                                    bloodBarText.text = "Blood: " + characterStats.points.ToString() + "/" + characterStats.maxPoints.ToString();

                                    healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
                                    healthBarText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();
                                }
                                i++;
                            }
                            
                        }
                        pauseSystem.Pause();
                    }
                    else if(isPaused && !isSelected && uiInput.goback)
                        {
                            AudioManager.playSound("menuback");
                            pauseSystem.UnPause();
                            isPaused = false;
                            pauseBackground.visible = false;
                            InputGatheringSystem.currentInput = CurrentInput.overworld;
                            healingSystem.OnHealthChange -= UpdateCharacterInfo_OnStatsUpdate;
                        }
                    else if(isPaused){
                            Button partyButton = root.Q<Button>("Party");
                            Button equipButton = root.Q<Button>("Equipment");
                            Button itemsButton = root.Q<Button>("Item");
                            Button skillsButton = root.Q<Button>("Skills");
                            Button optionsButton = root.Q<Button>("Settings");

                            VisualElement equipmentInfo = root.Q<VisualElement>("equipment_info");
                            VisualElement itemInfo = root.Q<VisualElement>("item_selection");
                            VisualElement skillInfo = root.Q<VisualElement>("skill_selection");
                            TemplateContainer optionsUI = root.Q<TemplateContainer>("SettingsUI");
                            VisualElement characterSelection = root.Q<VisualElement>("character_selection");
                            switch(currentSelection){
                                case PauseMenuSelectables.Party:
                                    if (isSelected)
                                    {
                                            //show it is selected on the current character
                                    }
                                    else if (uiInput.goselected) { 
                                            
                                    }
                                    else if (uiInput.moveright)
                                    {
                                        AudioManager.playSound("menuchange");
                                        UnselectButton(partyButton);
                                        SelectButton(equipButton);
                                        currentSelection = PauseMenuSelectables.Equip;
                                    }
                                break;
                                //for when the pause menu is on the equipment menu, can switch equipment
                                case PauseMenuSelectables.Equip:
                                    // all the needed labels in the ui for equipment
                                    VisualElement otherEquipmentBase = equipmentInfo.Q<VisualElement>("other_equipment");
                                    VisualElement currentEquipment= equipmentInfo.Q<VisualElement>("current_equipment");
                                    Label currentWeaponLabel = equipmentInfo.Q<Label>("current_weapon");
                                    Label currentArmorLabel = equipmentInfo.Q<Label>("current_armor");
                                    Label currentCharmLabel = equipmentInfo.Q<Label>("current_charm");
                                    Label equipmentDesc = equipmentInfo.Q<Label>("equipment_text");
                                    VisualElement equipQuickMenu = root.Q<VisualElement>("equipment_quickmenu");
                                    Label quickSwitch = equipQuickMenu.Q<Label>("switch");
                                    Label quickCancel = equipQuickMenu.Q<Label>("cancel");
                                    //for when you are in the actual equipment menu
                                    if (isSelected)
                                    {
                                        // only used when there is more than one character
                                        if(onCharacterSelect){
                                            if(uiInput.goselected){
                                                AudioManager.playSound("menuselect");
                                                onCharacterSelect = false;
                                                SelectInfoTab(equipmentInfo.Q<VisualElement>("current_equipment"));
                                                SelectItem(equipmentInfo.Q<Label>("current_weapon"));
                                                selectedEquipment = Equipment.Weapon;
                                            }
                                            else if (uiInput.goback)
                                            {
                                                AudioManager.playSound("menuback");
                                                isSelected = false;
                                                onCharacterSelect = false;
                                                equipmentInfo.visible = false;
                                                UnselectCharacter(root.Q<VisualElement>("character" + currentCharacter.ToString()));
                                            }
                                            else{
                                                if(playerParty.Length == 1){
                                                    //do nothing since there is only one option
                                                }
                                                else{
                                                    SelectCharacter(uiInput, playerParty.Length, characterSelection);
                                                }
                                            }
                                        }
                                        //choose a new piece of equipment to use
                                        else if(onItemSwitch){
                                            // switch the item
                                            if(uiInput.goselected){
                                                // must do switch, since if the inventory is empty, it will break
                                                switch(selectedEquipment){
                                                    case Equipment.Weapon:
                                                        //makes sure that the inventory isn't empty
                                                        if(!(weaponInventory.Length == 0)){
                                                            CharacterStats characterStats = GetComponent<CharacterStats>(characterEntities[currentCharacter-1]);
                                                            WeaponData unEquipedWeapon = new WeaponData{weapon = characterStatsList[currentCharacter - 1].equipedWeapon};
                                                            characterStats.equipedWeapon = weaponInventory[currentSwitchItem].weapon;
                                                            // move the unequiped item to the weaponinventory
                                                            weaponInventory.RemoveAt(currentSwitchItem);
                                                            weaponInventory.Insert(0, unEquipedWeapon);
                                                            
                                                            EntityManager.SetComponentData(characterEntities[currentCharacter-1], characterStats);
                                                            // may need to set the data of the characterStats
                                                            // update equipment info
                                                            equipmentDesc.text = characterStats.equipedWeapon.description.ToString();
                                                            equipmentInfo.Q<Label>("current_weapon").text = "Weapon: " + characterStats.equipedWeapon.name.ToString();
                                                        }
                                                    break;
                                                    case Equipment.Armor:
                                                        //makes sure that the inventory isn't empty
                                                        if(!(armorInventory.Length == 0)){
                                                            CharacterStats characterStats = GetComponent<CharacterStats>(characterEntities[currentCharacter-1]);
                                                            ArmorData unEquipedArmor = new ArmorData{armor = characterStatsList[currentCharacter - 1].equipedArmor};
                                                            characterStats.equipedArmor = armorInventory[currentSwitchItem].armor;
                                                            // move the unequiped item to the inventory
                                                            armorInventory.RemoveAt(currentSwitchItem);
                                                            armorInventory.Insert(0, unEquipedArmor);
                                                            
                                                            EntityManager.SetComponentData(characterEntities[currentCharacter-1], characterStats);
                                                            // may need to set the data of the characterStats
                                                            // update equipment info
                                                            equipmentDesc.text = characterStats.equipedArmor.description.ToString();
                                                            equipmentInfo.Q<Label>("current_armor").text = "Armor: " + characterStats.equipedArmor.name.ToString();
                                                        }
                                                    break;
                                                    case Equipment.Charm:
                                                        //makes sure that the inventory isn't empty
                                                        if(!(charmInventory.Length == 0)){
                                                            CharacterStats characterStats = GetComponent<CharacterStats>(characterEntities[currentCharacter-1]);
                                                            CharmData unEquipedCharm = new CharmData{charm = characterStatsList[currentCharacter - 1].equipedCharm};
                                                            characterStats.equipedCharm = charmInventory[currentSwitchItem].charm;
                                                            // move the unequiped item to the inventory
                                                            charmInventory.RemoveAt(currentSwitchItem);
                                                            charmInventory.Insert(0, unEquipedCharm);
                                                            
                                                            EntityManager.SetComponentData(characterEntities[currentCharacter-1], characterStats);
                                                            // may need to set the data of the characterStats
                                                            // update equipment info
                                                            equipmentDesc.text = characterStats.equipedCharm.description.ToString();
                                                            equipmentInfo.Q<Label>("current_charm").text = "Charm: " + characterStats.equipedCharm.name.ToString();
                                                        }
                                                    break;
                                                }
                                                // do the equivalent of goback
                                                AudioManager.playSound("menuselect");
                                                onItemSwitch = false;
                                                currentEquipment.visible = true;
                                                otherEquipmentBase.visible = false;
                                                UnSelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                            }
                                            else if(uiInput.goback){
                                                AudioManager.playSound("menuback");
                                                onItemSwitch = false;
                                                currentEquipment.visible = true;
                                                otherEquipmentBase.visible = false;
                                                UnSelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                            }
                                            else if(uiInput.moveup && currentSwitchItem > 0){
                                                // need to update the shown equipment when greater than 5
                                                if(currentSwitchItem > 5){

                                                }
                                                else{
                                                    AudioManager.playSound("menuchange");
                                                    UnSelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                                    currentSwitchItem--;
                                                    SelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                                    // updating the equipment information based on what is currently selected
                                                    switch(selectedEquipment){
                                                        case Equipment.Weapon:
                                                            equipmentDesc.text = weaponInventory[currentSwitchItem].weapon.description.ToString();
                                                        break;
                                                        case Equipment.Armor:
                                                            equipmentDesc.text = armorInventory[currentSwitchItem].armor.description.ToString();
                                                        break;
                                                        case Equipment.Charm:
                                                            equipmentDesc.text = charmInventory[currentSwitchItem].charm.description.ToString();
                                                        break;
                                                    }
                                                }

                                            }
                                            else if(uiInput.movedown){
                                                switch(selectedEquipment){
                                                    case Equipment.Weapon:
                                                        // need to make sure that when moving it doesn't go beyond the number of available items
                                                        if(currentSwitchItem < weaponInventory.Length - 1){
                                                            
                                                            // move the items down when moving down when greater than or equal to 5
                                                            if(currentSwitchItem >= 5){
                                                                
                                                            }
                                                            else{
                                                                AudioManager.playSound("menuchange");
                                                                UnSelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                                                currentSwitchItem++;
                                                                SelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                                                //updating the equipment description
                                                                equipmentDesc.text = weaponInventory[currentSwitchItem].weapon.description.ToString();
                                                            }
                                                        }
                                                    break;
                                                    case Equipment.Armor:
                                                        if(currentSwitchItem < armorInventory.Length - 1){
                                                            // move the items down when moving down when greater than or equal to 5
                                                            if(currentSwitchItem >= 5){
                                                                
                                                            }
                                                            else{
                                                                AudioManager.playSound("menuchange");
                                                                UnSelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                                                currentSwitchItem++;
                                                                SelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                                                //updating the equipment description
                                                                equipmentDesc.text = armorInventory[currentSwitchItem].armor.description.ToString();
                                                            }
                                                        }
                                                    break;
                                                    case Equipment.Charm:
                                                        if(currentSwitchItem < charmInventory.Length - 1){
                                                            // move the items down when moving down when greater than or equal to 5
                                                            if(currentSwitchItem >= 5){
                                                                
                                                            }
                                                            else{
                                                                AudioManager.playSound("menuchange");
                                                                UnSelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                                                currentSwitchItem++;
                                                                SelectItem(otherEquipmentBase.Q<Label>("equip" + (currentSwitchItem + 1).ToString()));
                                                                //updating the equipment description
                                                                equipmentDesc.text = charmInventory[currentSwitchItem].charm.description.ToString();
                                                            }
                                                        }
                                                    break;
                                                }
                                            }
                                        }
                                        // gives you the option to switch a piece of equipment or cancel
                                        else if(onQuickMenu){
                                            if(uiInput.goback){
                                                AudioManager.playSound("menuback");
                                                UnSelectQuickMenuButton(quickCancel);
                                                UnSelectQuickMenuButton(quickSwitch);
                                                equipQuickMenu.visible = false;
                                                onQuickMenu = false;
                                            }
                                            switch(currentQuickMenuSelection){
                                                    case QuickMenuSelectables.Switch:
                                                        // opens up the needed menus and sets the values nessasary to switch items
                                                        if(uiInput.goselected){
                                                            AudioManager.playSound("menuselect");
                                                            // makes the new equipment visable and the current equipment invisable
                                                            otherEquipmentBase.visible = true;
                                                            currentEquipment.visible = false;
                                                            // reseting the switch item
                                                            currentSwitchItem = 0;
                                                            UnSelectQuickMenuButton(quickSwitch);
                                                            equipQuickMenu.visible = false;
                                                            //moving the state machine
                                                            onQuickMenu = false;
                                                            onItemSwitch = true;
                                                            // show all available equipment switching what to display depending on what is selected
                                                            switch(selectedEquipment){
                                                                case Equipment.Weapon:
                                                                    for(int i = 0; i < 5; i++){
                                                                        if(i < weaponInventory.Length){
                                                                            Weapon weapon = weaponInventory[i].weapon;
                                                                            if(i == 0){
                                                                                equipmentDesc.text = weapon.description.ToString();
                                                                                SelectItem(otherEquipmentBase.Q<Label>("equip" + (i + 1)));
                                                                            }
                                                                            otherEquipmentBase.Q<Label>("equip" + (i + 1).ToString()).text = weapon.name.ToString();
                                                                        }
                                                                        else{
                                                                            otherEquipmentBase.Q<Label>("equip" + (i + 1).ToString()).text = "None";
                                                                        }
                                                                    }
                                                                break;
                                                                case Equipment.Armor:  
                                                                    for(int i = 0; i < 5; i++){
                                                                        if(i < armorInventory.Length){
                                                                            Armor armor = armorInventory[i].armor;
                                                                            if(i == 0){
                                                                                equipmentDesc.text = armor.description.ToString();
                                                                                SelectItem(otherEquipmentBase.Q<Label>("equip" + (i + 1)));
                                                                            }
                                                                            otherEquipmentBase.Q<Label>("equip" + (i + 1).ToString()).text = armor.name.ToString();
                                                                        }
                                                                        else{
                                                                            otherEquipmentBase.Q<Label>("equip" + (i + 1).ToString()).text = "None";
                                                                        }
                                                                    }
                                                                break;
                                                                case Equipment.Charm:
                                                                    for(int i = 0; i < 5; i++){
                                                                        if(i < charmInventory.Length){
                                                                            Charm charm = charmInventory[i].charm;
                                                                            if(i == 0){
                                                                                equipmentDesc.text = charm.description.ToString();
                                                                                SelectItem(otherEquipmentBase.Q<Label>("equip" + (i + 1)));
                                                                            }
                                                                            otherEquipmentBase.Q<Label>("equip" + (i + 1).ToString()).text = charm.name.ToString();
                                                                        }
                                                                        else{
                                                                            otherEquipmentBase.Q<Label>("equip" + (i + 1).ToString()).text = "None";
                                                                        }
                                                                    }
                                                                break;
                                                            }
                                                            
                                                        }
                                                        else if(uiInput.movedown){
                                                            AudioManager.playSound("menuchange");
                                                            currentQuickMenuSelection = QuickMenuSelectables.Cancel;
                                                            UnSelectQuickMenuButton(quickSwitch);
                                                            SelectQuickMenuButton(quickCancel);
                                                        }
                                                    break;
                                                    case QuickMenuSelectables.Cancel:
                                                        if(uiInput.goselected){
                                                            AudioManager.playSound("menuselect");
                                                            UnSelectQuickMenuButton(quickCancel);
                                                            UnSelectQuickMenuButton(quickSwitch);
                                                            equipQuickMenu.visible = false;
                                                            onQuickMenu = false;
                                                        }
                                                        else if(uiInput.moveup){
                                                            AudioManager.playSound("menuchange");
                                                            currentQuickMenuSelection = QuickMenuSelectables.Switch;
                                                            UnSelectQuickMenuButton(quickCancel);
                                                            SelectQuickMenuButton(quickSwitch);
                                                        }
                                                    break;
                                                }
                                        }
                                        else{
                                            if(uiInput.goselected){
                                                AudioManager.playSound("menuselect");
                                                //activate quickmenu
                                                currentQuickMenuSelection = QuickMenuSelectables.Switch;
                                                SelectQuickMenuButton(quickSwitch);
                                                equipQuickMenu.visible = true;
                                                onQuickMenu = true;
                                                switch(selectedEquipment){
                                                    case Equipment.Weapon:
                                                        placeQuickMenu(currentWeaponLabel, equipQuickMenu);
                                                    break;
                                                    case Equipment.Armor:
                                                        placeQuickMenu(currentArmorLabel, equipQuickMenu);
                                                    break;
                                                    case Equipment.Charm:
                                                        placeQuickMenu(currentCharmLabel, equipQuickMenu);
                                                    break;
                                                }
                                            }
                                            else if (uiInput.goback)
                                            {
                                                AudioManager.playSound("menuback");
                                                if(playerParty.Length == 1){
                                                    isSelected = false;
                                                    equipmentInfo.visible = false;
                                                    currentEquipment.visible = false;
                                                    UnselectCharacter(root.Q<VisualElement>("character1"));
                                                }
                                                else{
                                                    onCharacterSelect = true;
                                                }
                                                UnselectInfoTab(equipmentInfo.Q<VisualElement>("current_equipment"));
                                                switch(selectedEquipment){
                                                    case Equipment.Weapon:
                                                        UnSelectItem(currentWeaponLabel);
                                                    break;
                                                    case Equipment.Armor:
                                                        UnSelectItem(currentArmorLabel);
                                                    break;
                                                    case Equipment.Charm:
                                                        UnSelectItem(currentCharmLabel);
                                                    break;
                                                }
                                            }
                                            else if(uiInput.movedown){
                                                AudioManager.playSound("menuchange");
                                                switch(selectedEquipment){
                                                    case Equipment.Weapon:
                                                        selectedEquipment = Equipment.Armor;
                                                        UnSelectItem(currentWeaponLabel);
                                                        SelectItem(currentArmorLabel);
                                                        equipmentDesc.text = characterStatsList[currentCharacter - 1].equipedArmor.description.ToString();
                                                    break;
                                                    case Equipment.Armor:
                                                        selectedEquipment = Equipment.Charm;
                                                        UnSelectItem(currentArmorLabel);
                                                        SelectItem(currentCharmLabel);
                                                        equipmentDesc.text = characterStatsList[currentCharacter - 1].equipedCharm.description.ToString();
                                                    break;
                                                    case Equipment.Charm:
                                                        //selectedEquipment = Equipment.Armor;
                                                        //UnSelectItem(currentCharm);
                                                        //SelectItem(currentArmor);
                                                    break;
                                                }
                                            }
                                            else if(uiInput.moveup){
                                                AudioManager.playSound("menuchange");
                                                switch(selectedEquipment){
                                                    case Equipment.Weapon:
                                                        //UnSelectItem(currentWeapon);
                                                        //SelectItem(currentArmor);
                                                    break;
                                                    case Equipment.Armor:
                                                        selectedEquipment = Equipment.Weapon;
                                                        UnSelectItem(currentArmorLabel);
                                                        SelectItem(currentWeaponLabel);
                                                        equipmentDesc.text = characterStatsList[currentCharacter - 1].equipedWeapon.description.ToString();
                                                    break;
                                                    case Equipment.Charm:
                                                        selectedEquipment = Equipment.Armor;
                                                        UnSelectItem(currentCharmLabel);
                                                        SelectItem(currentArmorLabel);
                                                        equipmentDesc.text = characterStatsList[currentCharacter - 1].equipedArmor.description.ToString();
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    // go into the menu
                                    else if (uiInput.goselected)
                                    {
                                        AudioManager.playSound("menuselect");
                                        isSelected = true;
                                        if(playerParty.Length == 1){
                                            
                                            onCharacterSelect = false;
                                            SelectInfoTab(equipmentInfo.Q<VisualElement>("current_equipment"));
                                            SelectItem(equipmentInfo.Q<Label>("current_weapon"));
                                            selectedEquipment = Equipment.Weapon;
                                        }
                                        else{
                                            onCharacterSelect = true;
                                        }
                                        currentWeaponLabel.text = "Weapon: " + characterStatsList[currentCharacter -1].equipedWeapon.name;
                                        currentArmorLabel.text = "Armor: " + characterStatsList[currentCharacter -1].equipedArmor.name;
                                        currentCharmLabel.text = "Charm: " + characterStatsList[currentCharacter -1].equipedCharm.name;
                                        currentEquipment.visible = true;
                                        equipmentDesc.text = characterStatsList[currentCharacter - 1].equipedWeapon.description.ToString();
                                        SelectCharacter(root.Q<VisualElement>("character" + currentCharacter.ToString()));
                                        equipmentInfo.visible = true;
                                    }
                                    // move to the other menus
                                    else if (uiInput.moveleft)
                                    {
                                        AudioManager.playSound("menuchange");
                                        UnselectButton(equipButton);
                                        SelectButton(partyButton);
                                        currentSelection = PauseMenuSelectables.Party;
                                    }
                                    else if (uiInput.moveright)
                                    {
                                        AudioManager.playSound("menuchange");
                                        UnselectButton(equipButton);
                                        SelectButton(itemsButton);
                                        currentSelection = PauseMenuSelectables.Items;
                                    }
                                break;
                                case PauseMenuSelectables.Items:
                                    VisualElement itemList = itemInfo.Q<VisualElement>("item_list");
                                    Label itemDesc = itemInfo.Q<Label>("item_desc");
                                    VisualElement itemsQuickMenu = root.Q<VisualElement>("items_quickmenu");
                                    Label quickUseItems = itemsQuickMenu.Q<Label>("use");
                                    Label quickDropItems = itemsQuickMenu.Q<Label>("drop");
                                    Label quickGiveItems = itemsQuickMenu.Q<Label>("give");
                                    Label quickCancelItems = itemsQuickMenu.Q<Label>("cancel");

                                    if (isSelected)
                                    {
                                        if(onCharacterSelect){
                                            //select a character
                                            if (uiInput.goback) { 
                                                AudioManager.playSound("menuback");
                                                isSelected = false;
                                                itemInfo.visible = false;
                                            }
                                        }
                                        else if(onQuickMenu){
                                            if(uiInput.goback){

                                                AudioManager.playSound("menuback");
                                                UnSelectQuickMenuButton(quickUseItems);
                                                UnSelectQuickMenuButton(quickDropItems);
                                                UnSelectQuickMenuButton(quickGiveItems);
                                                UnSelectQuickMenuButton(quickCancelItems);
                                                itemsQuickMenu.visible = false;
                                                onQuickMenu = false;
                                            }
                                            if(uiInput.goselected){
                                                AudioManager.playSound("menuselect");
                                                DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(characterEntities[currentCharacter -1]);
                                                switch(currentQuickMenuSelection){
                                                    case QuickMenuSelectables.Use:
                                                        {DynamicBuffer<HealingData> healings = GetBuffer<HealingData>(characterEntities[currentCharacter -1]);
                                                        HealingData healing = new HealingData{healing = itemInventory[currentItem - 1].item.strength};
                                                        healings.Add(healing);
                                                        itemInventory.RemoveAt(currentItem - 1);
                                                        UnSelectQuickMenuButton(quickUseItems);
                                                        //exit items menu
                                                        itemsQuickMenu.visible = false;
                                                        onQuickMenu = false;
                                                        isSelected = false;
                                                        itemInfo.visible = false;
                                                        UnselectCharacter(root.Q<VisualElement>("character" + currentCharacter));
                                                        currentItem = 1;
                                                        // updating the character health
                                                        VisualElement currentCharacterUI = pauseBackground.Q<VisualElement>("character" + (currentCharacter).ToString());
                                                        Label healthBarText = currentCharacterUI.Q<Label>("health_text");
                                                        VisualElement healthBarBase = currentCharacterUI.Q<VisualElement>("health_bar_base");
                                                        VisualElement healthBar = currentCharacterUI.Q<VisualElement>("health_bar");
                                                        VisualElement bloodBar = currentCharacterUI.Q<VisualElement>("blood");
                                                        CharacterStats characterStats = characterStatsList[currentCharacter - 1];
                                                        healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
                                                        healthBarText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();}
                                                    break;
                                                    case QuickMenuSelectables.Drop:
                                                        itemInventory.RemoveAt(currentItem - 1);
                                                        UnSelectQuickMenuButton(quickDropItems);
                                                        //exit items menu
                                                        itemsQuickMenu.visible = false;
                                                        onQuickMenu = false;
                                                        isSelected = false;
                                                        itemInfo.visible = false;
                                                        UnselectCharacter(root.Q<VisualElement>("character" + currentCharacter));
                                                        currentItem = 1;
                                                    break;
                                                    case QuickMenuSelectables.Cancel:
                                                        UnSelectQuickMenuButton(quickCancelItems);
                                                        itemsQuickMenu.visible = false;
                                                        onQuickMenu = false;
                                                        
                                                    break;
                                                }
                                            }
                                            if(uiInput.moveup){
                                                AudioManager.playSound("menuchange");
                                                QuickMenuUp(itemsQuickMenu);
                                            }
                                            else if(uiInput.movedown){
                                                AudioManager.playSound("menuchange");
                                                QuickMenuDown(itemsQuickMenu);
                                            }
                                        }
                                        else{
                                            DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(characterEntities[currentCharacter -1]);
                                            if(uiInput.goselected && !(currentItem > itemInventory.Length )){
                                                AudioManager.playSound("menuchange");
                                                itemsQuickMenu.visible = true;
                                                onQuickMenu = true;
                                                placeQuickMenu(itemList.Q<Label>("item" + currentItem), itemsQuickMenu);
                                                //ensures that it is reset
                                                useableQuickSelectables.Clear();
                                                //all items will have these features
                                                useableQuickSelectables.Add(QuickMenuSelectables.Cancel);
                                                if(playerParty.Length != 1){
                                                    useableQuickSelectables.Add(QuickMenuSelectables.Give);
                                                    SelectableQuickMenuButton(quickGiveItems);
                                                }
                                                else{
                                                    UnSelectableQuickMenuButton(quickGiveItems);
                                                }
                                                useableQuickSelectables.Add(QuickMenuSelectables.Drop);
                                                //add the useable selectables based on the item type
                                                switch(itemInventory[currentItem - 1].item.itemType){
                                                    case ItemType.healing:
                                                        useableQuickSelectables.Add(QuickMenuSelectables.Use);
                                                        currentQuickMenuSelection = QuickMenuSelectables.Use;
                                                        SelectQuickMenuButton(quickUseItems);
                                                    break;
                                                    case ItemType.none:
                                                        currentQuickMenuSelection = QuickMenuSelectables.Cancel;
                                                        SelectQuickMenuButton(quickCancelItems);
                                                    break;
                                                }
                                                
                                            }
                                            else if(uiInput.goback){
                                                AudioManager.playSound("menuback");
                                                UnSelectItem(itemList.Q<Label>("item" + currentItem));
                                                if(playerParty.Length == 1){
                                                    isSelected = false;
                                                    itemInfo.visible = false;
                                                    UnselectCharacter(root.Q<VisualElement>("character1"));
                                                    currentItem = 1;
                                                }
                                                else{
                                                    onCharacterSelect = true;
                                                }
                                                UnselectInfoTab(itemInfo.Q<VisualElement>("item_list"));
                                            }
                                            else if(uiInput.movedown){
                                                AudioManager.playSound("menuchange");
                                                if(currentItem < 10){
                                                    UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    currentItem++;
                                                    SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    if(itemInventory.Length < currentItem){
                                                        itemDesc.text = "no description";
                                                    }
                                                    else{
                                                        string itemDescriptionToShow = itemInventory[currentItem - 1].item.description.ToString();
                                                        if(itemDescriptionToShow == ""){
                                                            itemDesc.text = "no description";
                                                        }
                                                        else{
                                                            itemDesc.text = itemInventory[currentItem - 1].item.description.ToString();
                                                        }
                                                    }
                                                    
                                                }
                                            }
                                            else if(uiInput.moveup){
                                                AudioManager.playSound("menuchange");
                                                if(currentItem > 1){
                                                    UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    currentItem--;
                                                    SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    if(itemInventory.Length < currentItem){
                                                        itemDesc.text = "no description";
                                                    }
                                                    else{
                                                        string itemDescriptionToShow = itemInventory[currentItem - 1].item.description.ToString();
                                                        if(itemDescriptionToShow == ""){
                                                            itemDesc.text = "no description";
                                                        }
                                                        else{
                                                            itemDesc.text = itemDescriptionToShow;
                                                        }
                                                    }
                                                    
                                                }
                                            }
                                            else if(uiInput.moveleft){
                                                AudioManager.playSound("menuchange");
                                                if(currentItem > 5){
                                                    UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    currentItem-= 5;
                                                    SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    if(itemInventory.Length < currentItem){
                                                        itemDesc.text = "no description";
                                                    }
                                                    else{
                                                        string itemDescriptionToShow = itemInventory[currentItem - 1].item.description.ToString();
                                                        if(itemDescriptionToShow == ""){
                                                            itemDesc.text = "no description";
                                                        }
                                                        else{
                                                            itemDesc.text = itemDescriptionToShow;
                                                        }
                                                    }
                                                }
                                            }
                                            else if(uiInput.moveright){
                                                AudioManager.playSound("menuchange");
                                                if(currentItem < 6){
                                                    UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    currentItem+= 5;
                                                    SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    if(itemInventory.Length < currentItem){
                                                        itemDesc.text = "no description";
                                                    }
                                                    else{
                                                        string itemDescriptionToShow = itemInventory[currentItem - 1].item.description.ToString();
                                                        if(itemDescriptionToShow == ""){
                                                            itemDesc.text = "no description";
                                                        }
                                                        else{
                                                            itemDesc.text = itemDescriptionToShow;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        
                                    }
                                    else if (uiInput.goselected)
                                    {
                                        AudioManager.playSound("menuselect");
                                        DynamicBuffer<ItemData> itemInventory = GetBuffer<ItemData>(characterEntities[currentCharacter - 1]);
                                        isSelected = true;
                                        if(playerParty.Length == 1){
                                            onCharacterSelect = false;
                                            SelectInfoTab(itemList);
                                            SelectItem(itemList.Q<Label>("item1"));
                                            
                                        }
                                        else{
                                            onCharacterSelect = true;
                                        }
                                        int i = 0;
                                        foreach(ItemData itemData in itemInventory){
                                            Item item = itemData.item;
                                            if(item.name == ""){
                                                itemList.Q<Label>("item" + (i + 1).ToString()).text = "None";
                                            }
                                            else{
                                                itemList.Q<Label>("item" + (i + 1).ToString()).text = item.name.ConvertToString();
                                            }
                                            i++;
                                        }
                                        while(i < 10){
                                            itemList.Q<Label>("item" + (i + 1).ToString()).text = "None";
                                            i++;
                                        }
                                        string itemDescriptionToShow = itemInventory[currentItem - 1].item.description.ToString();
                                        if(itemDescriptionToShow == ""){
                                            itemDesc.text = "no description";
                                        }
                                        else{
                                            itemDesc.text = itemDescriptionToShow;
                                        }
                                        SelectCharacter(root.Q<VisualElement>("character" + currentCharacter.ToString()));
                                        itemInfo.visible = true;
                                    }
                                    else if (uiInput.moveleft)
                                    {
                                        AudioManager.playSound("menuchange");
                                        UnselectButton(itemsButton);
                                        SelectButton(equipButton);
                                        currentSelection = PauseMenuSelectables.Equip;

                                    }
                                    else if (uiInput.moveright)
                                    {
                                        AudioManager.playSound("menuchange");
                                        UnselectButton(itemsButton);
                                        SelectButton(skillsButton);
                                        currentSelection = PauseMenuSelectables.Skills;
                                    }
                                break;
                                case PauseMenuSelectables.Skills:
                                    VisualElement currentSkills = skillInfo.Q<VisualElement>("current_skills");
                                    VisualElement otherSkills = skillInfo.Q<VisualElement>("other_skills");
                                    Label skillDesc = skillInfo.Q<Label>("skill_desc");
                                    VisualElement skillsQuickMenu = root.Q<VisualElement>("skills_quickmenu");
                                    Label quickSwitchSkills = skillsQuickMenu.Q<Label>("switch");
                                    Label quickCancelSkills = skillsQuickMenu.Q<Label>("cancel");
                                    DynamicBuffer<SkillData> skills = GetBuffer<SkillData>(characterEntities[currentCharacter - 1]);
                                    DynamicBuffer<EquipedSkillData> equipedSkills = GetBuffer<EquipedSkillData>(characterEntities[currentCharacter -1]);

                                    if (isSelected)
                                    {
                                        if(onCharacterSelect){
                                            //select a character
                                            if (uiInput.goback) { 
                                                AudioManager.playSound("menuback");
                                                isSelected = false;
                                                skillInfo.visible = false;
                                            }
                                        }
                                        else if(onItemSwitch){
                                            if(uiInput.goselected){
                                                AudioManager.playSound("menuselect");
                                                //switchskill
                                                EquipedSkillData newSkill = new EquipedSkillData{skill = skills[currentSwitchItem - 1].skill};
                                                if(currentItem > equipedSkills.Length){
                                                    // nothing equiped, so nothing to add to the unequiped skills
                                                    skills.RemoveAt(currentSwitchItem - 1);

                                                }
                                                else{
                                                    SkillData currentSkill = new SkillData{skill = equipedSkills[currentItem - 1].skill};

                                                    equipedSkills.RemoveAt(currentItem - 1);
                                                    skills.RemoveAt(currentSwitchItem - 1);
                                                    skills.Add(currentSkill);
                                                }
                                                equipedSkills.Insert(currentItem -1, newSkill);

                                                currentSkills.Q<Label>("skill" + currentItem.ToString()).text = newSkill.skill.name.ToString();

                                                onItemSwitch = false;
                                                currentSkills.visible = true;
                                                otherSkills.visible = false;
                                                UnSelectItem(otherSkills.Q<Label>("skill"+ currentSwitchItem.ToString()));
                                            }
                                            else if(uiInput.goback){
                                                AudioManager.playSound("menuback");
                                                onItemSwitch = false;
                                                currentSkills.visible = true;
                                                otherSkills.visible = false;
                                                UnSelectItem(otherSkills.Q<Label>("skill"+ currentSwitchItem.ToString()));
                                            }
                                            else if(uiInput.moveup && currentSwitchItem > 1){
                                                AudioManager.playSound("menuchange");
                                                if(currentSwitchItem > 5){

                                                }
                                                else{
                                                    UnSelectItem(otherSkills.Q<Label>("skill" + (currentSwitchItem).ToString()));
                                                    currentSwitchItem--;
                                                    SelectItem(otherSkills.Q<Label>("skill" + (currentSwitchItem).ToString()));
                                                    // updating the equipment information based on what is currently selected
                                                    skillDesc.text = skills[currentSwitchItem - 1].skill.description.ToString();
                                                }
                                            }
                                            else if(uiInput.movedown && currentSwitchItem < skills.Length){
                                                AudioManager.playSound("menuchange");
                                                if(currentSwitchItem > 5){

                                                }
                                                else{
                                                    UnSelectItem(otherSkills.Q<Label>("skill" + (currentSwitchItem).ToString()));
                                                    currentSwitchItem++;
                                                    SelectItem(otherSkills.Q<Label>("skill" + (currentSwitchItem).ToString()));
                                                    // updating the equipment information based on what is currently selected
                                                    skillDesc.text = skills[currentSwitchItem - 1].skill.description.ToString();
                                                }
                                            }
                                        }
                                        else if(onQuickMenu){
                                            if(uiInput.goback){
                                                AudioManager.playSound("menuback");
                                                UnSelectQuickMenuButton(quickCancelSkills);
                                                UnSelectQuickMenuButton(quickSwitchSkills);
                                                skillsQuickMenu.visible = false;
                                                onQuickMenu = false;
                                            }
                                            else if(uiInput.goselected){
                                                AudioManager.playSound("menuselect");
                                                switch(currentQuickMenuSelection){
                                                    case QuickMenuSelectables.Switch:
                                                        //go into skill selection
                                                        onItemSwitch = true;
                                                        onQuickMenu = false;
                                                        // make sure all ui have the right visability
                                                        skillsQuickMenu.visible = false;
                                                        UnSelectQuickMenuButton(quickSwitchSkills);
                                                        currentSkills.visible = false;
                                                        otherSkills.visible = true;
                                                        // can't switch unless there is at least one skill, who's description should be displayed
                                                        skillDesc.text = skills[0].skill.description.ToString();
                                                        SelectItem(otherSkills.Q<Label>("skill1"));
                                                        currentSwitchItem = 1;

                                                        // set up all of the other skills
                                                        for(int i = 1 ; i <= 6; i++){
                                                            if(i <= skills.Length){
                                                                otherSkills.Q<Label>("skill" + i.ToString()).text = skills[i - 1].skill.name.ToString();
                                                            }
                                                            else{
                                                                otherSkills.Q<Label>("skill" + i.ToString()).text = "None";
                                                            }
                                                        }
                                                    break;
                                                    case QuickMenuSelectables.Cancel:
                                                        UnSelectQuickMenuButton(quickCancelSkills);
                                                        UnSelectQuickMenuButton(quickSwitchSkills);
                                                        skillsQuickMenu.visible = false;
                                                        onQuickMenu = false;
                                                    break;
                                                }
                                            }
                                            else if(uiInput.movedown){
                                                AudioManager.playSound("menuchange");
                                                QuickMenuDown(skillsQuickMenu);
                                            }
                                            else if(uiInput.moveup){
                                                AudioManager.playSound("menuchange");
                                                QuickMenuUp(skillsQuickMenu);
                                            }
                                        }
                                        else{
                                            
                                            if(uiInput.goselected){
                                                AudioManager.playSound("menuselect");
                                                
                                                skillsQuickMenu.visible = true;
                                                onQuickMenu = true;
                                                placeQuickMenu(currentSkills.Q<Label>("skill" + currentItem), skillsQuickMenu);
                                                useableQuickSelectables.Clear();
                                                useableQuickSelectables.Add(QuickMenuSelectables.Cancel);
                                                if(skills.Length == 0){
                                                    // if there is no skills available, you shouldn't be able to switch
                                                    SelectQuickMenuButton(quickCancelSkills);
                                                    UnSelectableQuickMenuButton(quickSwitchSkills);
                                                    currentQuickMenuSelection = QuickMenuSelectables.Cancel;
                                                }
                                                else{
                                                    SelectableQuickMenuButton(quickSwitchSkills);
                                                    SelectQuickMenuButton(quickSwitchSkills);
                                                    useableQuickSelectables.Add(QuickMenuSelectables.Switch);
                                                    currentQuickMenuSelection = QuickMenuSelectables.Switch;
                                                }
                                                
                                                
                                            }
                                            else if(uiInput.goback){
                                                AudioManager.playSound("menuback");
                                                UnSelectItem(currentSkills.Q<Label>("skill" + currentItem));
                                                if(playerParty.Length == 1){
                                                    
                                                    isSelected = false;
                                                    skillInfo.visible = false;
                                                    currentSkills.visible = false;
                                                    UnselectCharacter(root.Q<VisualElement>("character1"));
                                                    currentItem = 1;
                                                }
                                                else{
                                                    onCharacterSelect = true;
                                                }
                                                UnselectInfoTab(skillInfo.Q<VisualElement>("current_skills"));
                                            }
                                            else if(uiInput.movedown){
                                                AudioManager.playSound("menuchange");
                                                if(currentItem < 5){
                                                    UnSelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                    currentItem++;
                                                    SelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                    if(equipedSkills.Length < currentItem){
                                                        skillDesc.text = "No description";
                                                    }
                                                    else{
                                                        string descToDisplay = equipedSkills[currentItem - 1].skill.description.ToString();
                                                        if(descToDisplay == ""){
                                                            skillDesc.text = "No description";
                                                        }
                                                        else{
                                                            skillDesc.text = descToDisplay;
                                                        }
                                                    }
                                                }
                                            }
                                            else if(uiInput.moveup){
                                                AudioManager.playSound("menuchange");
                                                if(currentItem > 1){
                                                    UnSelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                    currentItem--;
                                                    SelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                    if(equipedSkills.Length < currentItem){
                                                        skillDesc.text = "No description";
                                                    }
                                                    else{
                                                        string descToDisplay = equipedSkills[currentItem - 1].skill.description.ToString();
                                                        if(descToDisplay == ""){
                                                            skillDesc.text = "No description";
                                                        }
                                                        else{
                                                            skillDesc.text = descToDisplay;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (uiInput.goselected)
                                    {
                                        AudioManager.playSound("menuselect");
                                        isSelected = true;
                                        if(playerParty.Length == 1){
                                            onCharacterSelect = false;
                                            currentSkills.visible = true;
                                            SelectInfoTab(currentSkills);
                                            SelectItem(currentSkills.Q<Label>("skill1"));
                                        }
                                        else{
                                            onCharacterSelect = true;
                                        }
                                        int i = 1;
                                        foreach(EquipedSkillData skillData in equipedSkills){
                                            Skill skill = skillData.skill;
                                            if(skill.name == ""){
                                                currentSkills.Q<Label>("skill" + i.ToString()).text = "None";
                                            }
                                            else{
                                                currentSkills.Q<Label>("skill" + i.ToString()).text = skill.name.ToString();
                                            }
                                            i++;
                                        }
                                        isSelected = true;

                                        string descToDisplay = equipedSkills[0].skill.description.ToString();
                                        if(descToDisplay == ""){
                                            skillDesc.text = "No description";
                                        }
                                        else{
                                            skillDesc.text = descToDisplay;
                                        }
                                        SelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                        SelectCharacter(root.Q<VisualElement>("character" + currentCharacter.ToString()));
                                        skillInfo.visible = true;
                                    }
                                    else if (uiInput.moveleft)
                                    {
                                        AudioManager.playSound("menuchange");
                                        UnselectButton(skillsButton);
                                        SelectButton(itemsButton);
                                        currentSelection = PauseMenuSelectables.Items;
                                    }
                                    else if (uiInput.moveright)
                                    {
                                        AudioManager.playSound("menuchange");
                                        UnselectButton(skillsButton);
                                        SelectButton(optionsButton);
                                        currentSelection = PauseMenuSelectables.Options;
                                    }
                                break;
                                case PauseMenuSelectables.Options:
                                    if (isSelected)
                                    {
                                        
                                    }
                                    else if (uiInput.goselected)
                                    {
                                        AudioManager.playSound("menuselect");
                                        //select the ui
                                        //isSelected = true
                                        // temporarly goes to the title screen
                                        isSelected = true;
                                        settingsMenuSystem.OnSettingsExit += EnableMenu_OnSettingsExit;
                                        settingsMenuSystem.ActivateMenu();
                                        //isPaused = false;
                                        pauseBackground.visible = false;
                                    }
                                    else if (uiInput.moveleft)
                                    {
                                        AudioManager.playSound("menuchange");
                                        UnselectButton(optionsButton);
                                        SelectButton(skillsButton);
                                        currentSelection = PauseMenuSelectables.Skills;
                                    }
                                break;
                            }
                            
                    }
                  }
                characterEntities.Dispose();
                characterStatsList.Dispose();
            }).Run();
        
      }
    private void DisableMenu_OnTitleReturn(System.Object sender, System.EventArgs e){
        pauseSystem.UnPause();
        isPaused = false;
        currentCharacter = 1;
        currentItem = 1;
        isSelected = false;
        onCharacterSelect = false;

        currentSelection = PauseMenuSelectables.Party;
        healingSystem.OnHealthChange -= UpdateCharacterInfo_OnStatsUpdate;
    }
    private void EnableMenu_OnSettingsExit(System.Object sender, System.EventArgs e){
        Debug.Log("enabling menu");
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((in UIDocument UIDoc,in PauseMenuTag pausMenuTag) =>{
            VisualElement root = UIDoc.rootVisualElement;
            VisualElement pauseBackground = root.Q<VisualElement>("pause_background");
            pauseBackground.visible = true;
        }).Run();
        isSelected = false;
      }
    private void UnselectButton(Button button)
    {
        button.RemoveFromClassList("button_selected");
        button.AddToClassList("button_unselected");
    }
    private void SelectButton(Button button)
    {
        button.RemoveFromClassList("button_unselected");
        button.AddToClassList("button_selected");
    }

    private void UnselectInfoTab(VisualElement tab){
        tab.RemoveFromClassList("tab_selected");
        tab.AddToClassList("tab_unselected");
    }
    private void SelectInfoTab(VisualElement tab){
        tab.RemoveFromClassList("tab_unselected");
        tab.AddToClassList("tab_selected");
    }
    private void UnselectCharacter(VisualElement character){
        character.RemoveFromClassList("character_selected");
        character.AddToClassList("character_unselected");
    }
    private void SelectCharacter(VisualElement character){
        character.RemoveFromClassList("character_unselected");
        character.AddToClassList("character_selected");
    }

    private void SelectItem(Label item){
        item.RemoveFromClassList("item_unselected");
        item.AddToClassList("item_selected");
    }
    private void UnSelectItem(Label item){
        item.RemoveFromClassList("item_selected");
        item.AddToClassList("item_unselected");
    }
    private void SelectQuickMenuButton(Label button){
        button.RemoveFromClassList("quickmenu_button_unselected");
        button.AddToClassList("quickmenu_button_selected");
    }
    private void UnSelectQuickMenuButton(Label button){
        button.RemoveFromClassList("quickmenu_button_selected");
        button.AddToClassList("quickmenu_button_unselected");
    }
    private void SelectableQuickMenuButton(Label button){
        button.RemoveFromClassList("quickmenu_button_unselectable");
        button.AddToClassList("quickmenu_button_unselected");
    }
    private void UnSelectableQuickMenuButton(Label button){
        button.RemoveFromClassList("quickmenu_button_unselected");
        button.AddToClassList("quickmenu_button_unselectable");
    }
    private void placeQuickMenu(Label item, VisualElement quickMenu){
        quickMenu.style.right = item.contentRect.width + item.worldBound.x;
        quickMenu.style.top = item.worldBound.y;
    }
    private void SelectCharacter(UIInputData input, int length, VisualElement characterSelection){
            if(input.moveright){
                if(!(currentCharacter == length)){
                    VisualElement oldCharacter = characterSelection.Q<VisualElement>("character" + currentCharacter.ToString());
                    currentCharacter++;
                    VisualElement newCharacter = characterSelection.Q<VisualElement>("character" + currentCharacter.ToString());
                    UnselectCharacter(oldCharacter);
                    SelectCharacter(newCharacter);
                }
            }
            else if(input.moveleft){
                if(!(currentCharacter == 1)){
                    VisualElement oldCharacter = characterSelection.Q<VisualElement>("character" + currentCharacter.ToString());
                    currentCharacter--;
                    VisualElement newCharacter = characterSelection.Q<VisualElement>("character" + currentCharacter.ToString());
                    UnselectCharacter(oldCharacter);
                    SelectCharacter(newCharacter);
                }
            }
    }

    private void QuickMenuUp(VisualElement QuickMenu){
        Label currentQuickButton = QuickMenu.Q<Label>(currentQuickMenuSelection.ToString().ToLower());
        UnSelectQuickMenuButton(currentQuickButton);
        QuickMenuSelectables temp = currentQuickMenuSelection;
        // won't set current selection to a new value unless it is useable
        while(temp > 0){
            temp--;
            if(useableQuickSelectables.Contains(temp)){
                // found matching object, select it
                currentQuickMenuSelection = temp;
                break;
            }
        }
        Label newQuickMenuButton = QuickMenu.Q<Label>(currentQuickMenuSelection.ToString().ToLower());
        SelectQuickMenuButton(newQuickMenuButton);  
    }
    private void QuickMenuDown(VisualElement QuickMenu){
        Label currentQuickButton = QuickMenu.Q<Label>(currentQuickMenuSelection.ToString().ToLower());
        UnSelectQuickMenuButton(currentQuickButton);
        QuickMenuSelectables temp = currentQuickMenuSelection;
        // won't set current selection to a new value unless it is useable
        while(temp < QuickMenuSelectables.Cancel){
            temp++;
            if(useableQuickSelectables.Contains(temp)){
                // found matching object, select it
                currentQuickMenuSelection = temp;
                break;
            }
        }
        Label newQuickMenuButton = QuickMenu.Q<Label>(currentQuickMenuSelection.ToString().ToLower());
        SelectQuickMenuButton(newQuickMenuButton);  
    }
    private void UpdateCharacterInfo_OnStatsUpdate(System.Object sender, System.EventArgs e){
        EntityQuery characterStatsQuery = GetEntityQuery(typeof(CharacterStats), typeof(PlayerTag));
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((in UIDocument UIDoc,in PauseMenuTag pausMenuTag) =>{
            VisualElement root = UIDoc.rootVisualElement;
            VisualElement pauseBackground = root.Q<VisualElement>("pause_background");
            VisualElement currentCharacterUI = pauseBackground.Q<VisualElement>("character" + (currentCharacter).ToString());
            Label healthBarText = currentCharacterUI.Q<Label>("health_text");
            VisualElement healthBarBase = currentCharacterUI.Q<VisualElement>("health_bar_base");
            VisualElement healthBar = currentCharacterUI.Q<VisualElement>("health_bar");
            //VisualElement bloodBar = currentCharacterUI.Q<VisualElement>("blood");
            CharacterStats characterStats = characterStatsList[currentCharacter - 1];
            healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
            healthBarText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();
        }).Run();
        characterStatsList.Dispose();
    }
}
public enum Equipment{
    Weapon,
    Armor,
    Charm
}
public enum QuickMenuSelectables{
    Switch,
    Use,
    Drop,
    Give,
    Cancel
}
public enum PauseMenuSelectables{
      Party,
      Equip,
      Items,
      Skills,
      Options
}

