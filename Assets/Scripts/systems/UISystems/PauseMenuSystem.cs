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
      
      private Entity pauseMenuSubScene;

      private bool isPaused;

      private int currentCharacter;
      private bool isSelected;
      private bool onCharacterSelect;
      private bool onQuickMenu;
      private QuickMenuSelectables currentQuickMenuSelection;
      private int currentItem;
      private bool onRightTab;
      private Equipment selectedEquipment;

      protected override void OnStartRunning()
      {
            base.OnStartRunning();
            sceneSystem = World.GetOrCreateSystem<SceneSystem>();
            currentCharacter = 1;
            currentItem = 1;

            currentSelection = PauseMenuSelectables.Party;
      }

      protected override void OnUpdate()
      {
        EntityQuery overworldInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData overworldInput = overworldInputQuery.GetSingleton<OverworldInputData>();

        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData uiInput = uiInputQuery.GetSingleton<UIInputData>();

        DynamicBuffer<PlayerPartyData> playerParty = GetBuffer<PlayerPartyData>(GetSingletonEntity<PlayerTag>());

        EntityQuery characterStatsQuery = GetEntityQuery(typeof(CharacterStats), typeof(CharacterInventoryData));
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
        NativeArray<Entity> characterStatsEntities = characterStatsQuery.ToEntityArray(Allocator.Temp);
        List<CharacterInventoryData> characterInventories = new List<CharacterInventoryData>();
        foreach(Entity ent in characterStatsEntities){
            characterInventories.Add(EntityManager.GetComponentObject<CharacterInventoryData>(ent));
        }
        characterStatsEntities.Dispose();

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((in UIDocument UIDoc,in PauseMenuTag pausMenuTag) =>{
                  VisualElement root = UIDoc.rootVisualElement;
                  if(root == null){

                  }
                  else{
                    VisualElement pauseBackground = root.Q<VisualElement>("pause_background");
                    TemplateContainer settingsUI = root.Q<TemplateContainer>("SettingsUI");

                    if(!isPaused && overworldInput.escape){
                        

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
                                    VisualElement bloodBar = currentCharacter.Q<VisualElement>("blood");
                                    healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
                                    healthBarText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();
                                }
                                i++;
                            }
                            
                        }
                    }
                    else if(isPaused && !isSelected && uiInput.goback)
                        {
                            isPaused = false;
                            pauseBackground.visible = false;
                            InputGatheringSystem.currentInput = CurrentInput.overworld;
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
                                        UnselectButton(partyButton);
                                        SelectButton(equipButton);
                                        currentSelection = PauseMenuSelectables.Equip;
                                    }
                                break;
                                case PauseMenuSelectables.Equip:
                                    Label currentWeaponLabel = equipmentInfo.Q<Label>("current_weapon");
                                    Label currentArmorLabel = equipmentInfo.Q<Label>("current_armor");
                                    Label currentCharmLabel = equipmentInfo.Q<Label>("current_charm");
                                    Label equipmentDesc = equipmentInfo.Q<Label>("equipment_text");
                                    VisualElement equipQuickMenu = root.Q<VisualElement>("equipment_quickmenu");
                                    Label quickSwitch = equipQuickMenu.Q<Label>("switch");
                                    Label quickCancel = equipQuickMenu.Q<Label>("cancel");
                                    if (isSelected)
                                    {
                                        if(onCharacterSelect){
                                            if(uiInput.goselected){
                                                onCharacterSelect = false;
                                                SelectInfoTab(equipmentInfo.Q<VisualElement>("current_equipment"));
                                                SelectItem(equipmentInfo.Q<Label>("current_weapon"));
                                                selectedEquipment = Equipment.Weapon;
                                            }
                                            else if (uiInput.goback)
                                            {
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
                                        else if(onQuickMenu){
                                            if(uiInput.goback){
                                                UnSelectQuickMenuButton(quickCancel);
                                                UnSelectQuickMenuButton(quickSwitch);
                                                equipQuickMenu.visible = false;
                                                onQuickMenu = false;
                                            }
                                            switch(currentQuickMenuSelection){
                                                    case QuickMenuSelectables.Switch:
                                                        if(uiInput.goselected){
                                                            //go into item selection
                                                        }
                                                        else if(uiInput.movedown){
                                                            currentQuickMenuSelection = QuickMenuSelectables.Cancel;
                                                            UnSelectQuickMenuButton(quickSwitch);
                                                            SelectQuickMenuButton(quickCancel);
                                                        }
                                                    break;
                                                    case QuickMenuSelectables.Cancel:
                                                        if(uiInput.goselected){
                                                            UnSelectQuickMenuButton(quickCancel);
                                                            UnSelectQuickMenuButton(quickSwitch);
                                                            equipQuickMenu.visible = false;
                                                            onQuickMenu = false;
                                                        }
                                                        else if(uiInput.moveup){
                                                            currentQuickMenuSelection = QuickMenuSelectables.Switch;
                                                            UnSelectQuickMenuButton(quickCancel);
                                                            SelectQuickMenuButton(quickSwitch);
                                                        }
                                                    break;
                                                }
                                        }
                                        else{
                                            if(uiInput.goselected){
                                                //activate quickmenu
                                                currentQuickMenuSelection = QuickMenuSelectables.Switch;
                                                SelectQuickMenuButton(quickSwitch);
                                                equipQuickMenu.visible = true;
                                                onQuickMenu = true;
                                                
                                            }
                                            else if (uiInput.goback)
                                            {
                                                if(playerParty.Length == 1){
                                                    isSelected = false;
                                                    equipmentInfo.visible = false;
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
                                                switch(selectedEquipment){
                                                    case Equipment.Weapon:
                                                        selectedEquipment = Equipment.Armor;
                                                        UnSelectItem(currentWeaponLabel);
                                                        SelectItem(currentArmorLabel);
                                                        equipmentDesc.text = characterInventories[currentCharacter - 1].equipedArmor.description;
                                                    break;
                                                    case Equipment.Armor:
                                                        selectedEquipment = Equipment.Charm;
                                                        UnSelectItem(currentArmorLabel);
                                                        SelectItem(currentCharmLabel);
                                                        equipmentDesc.text = characterInventories[currentCharacter - 1].equipedCharm.description;
                                                    break;
                                                    case Equipment.Charm:
                                                        //selectedEquipment = Equipment.Armor;
                                                        //UnSelectItem(currentCharm);
                                                        //SelectItem(currentArmor);
                                                    break;
                                                }
                                            }
                                            else if(uiInput.moveup){
                                                switch(selectedEquipment){
                                                    case Equipment.Weapon:
                                                        //UnSelectItem(currentWeapon);
                                                        //SelectItem(currentArmor);
                                                    break;
                                                    case Equipment.Armor:
                                                        selectedEquipment = Equipment.Weapon;
                                                        UnSelectItem(currentArmorLabel);
                                                        SelectItem(currentWeaponLabel);
                                                        equipmentDesc.text = characterInventories[currentCharacter - 1].equipedWeapon.description;
                                                    break;
                                                    case Equipment.Charm:
                                                        selectedEquipment = Equipment.Armor;
                                                        UnSelectItem(currentCharmLabel);
                                                        SelectItem(currentArmorLabel);
                                                        equipmentDesc.text = characterInventories[currentCharacter - 1].equipedArmor.description;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (uiInput.goselected)
                                    {
                                        isSelected = true;
                                        if(playerParty.Length == 1){
                                            currentWeaponLabel.text = "Weapon: " + characterInventories[currentCharacter - 1].equipedWeapon.name;
                                            currentArmorLabel.text = "Armor: " + characterInventories[currentCharacter - 1].equipedArmor.name;
                                            currentCharmLabel.text = "Charm: " + characterInventories[currentCharacter - 1].equipedCharm.name;
                                            equipmentDesc.text = characterInventories[currentCharacter - 1].equipedWeapon.description;
                                            onCharacterSelect = false;
                                            SelectInfoTab(equipmentInfo.Q<VisualElement>("current_equipment"));
                                            SelectItem(equipmentInfo.Q<Label>("current_weapon"));
                                            selectedEquipment = Equipment.Weapon;
                                        }
                                        else{
                                            onCharacterSelect = true;
                                        }
                                        SelectCharacter(root.Q<VisualElement>("character" + currentCharacter.ToString()));
                                        equipmentInfo.visible = true;
                                    }
                                    else if (uiInput.moveleft)
                                    {
                                        UnselectButton(equipButton);
                                        SelectButton(partyButton);
                                        currentSelection = PauseMenuSelectables.Party;
                                    }
                                    else if (uiInput.moveright)
                                    {
                                        UnselectButton(equipButton);
                                        SelectButton(itemsButton);
                                        currentSelection = PauseMenuSelectables.Items;
                                    }
                                break;
                                case PauseMenuSelectables.Items:
                                    VisualElement itemList = itemInfo.Q<VisualElement>("item_list");
                                    Label itemDesc = itemInfo.Q<Label>("item_desc");
                                    if (isSelected)
                                    {
                                        if(onCharacterSelect){
                                            //select a character
                                            if (uiInput.goback) { 
                                                isSelected = false;
                                                itemInfo.visible = false;
                                            }
                                        }
                                        else{
                                            
                                            if(uiInput.goselected){
                                                //do stuff with items
                                            }
                                            else if(uiInput.goback){
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
                                                if(currentItem < 10){
                                                    UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    currentItem++;
                                                    SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    Debug.Log(currentCharacter);
                                                    string itemDescriptionToShow = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                                    if(itemDescriptionToShow == ""){
                                                        itemDesc.text = "no description";
                                                    }
                                                    else{
                                                        itemDesc.text = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                                    }
                                                }
                                            }
                                            else if(uiInput.moveup){
                                                if(currentItem > 1){
                                                    UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    currentItem--;
                                                    SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    string itemDescriptionToShow = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                                    if(itemDescriptionToShow == ""){
                                                        itemDesc.text = "no description";
                                                    }
                                                    else{
                                                        itemDesc.text = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                                    }
                                                }
                                            }
                                            else if(uiInput.moveleft){
                                                if(currentItem > 5){
                                                    UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    currentItem-= 5;
                                                    SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    string itemDescriptionToShow = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                                    if(itemDescriptionToShow == ""){
                                                        itemDesc.text = "no description";
                                                    }
                                                    else{
                                                        itemDesc.text = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                                    }
                                                }
                                            }
                                            else if(uiInput.moveright){
                                                if(currentItem < 6){
                                                    UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    currentItem+= 5;
                                                    SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                    string itemDescriptionToShow = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                                    if(itemDescriptionToShow == ""){
                                                        itemDesc.text = "no description";
                                                    }
                                                    else{
                                                        itemDesc.text = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                                    }
                                                }
                                            }
                                        }
                                        
                                    }
                                    else if (uiInput.goselected)
                                    {
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
                                        foreach(Item item in characterInventories[currentCharacter - 1].inventory){
                                            
                                            if(item.name == ""){
                                                itemList.Q<Label>("item" + (i + 1).ToString()).text = "None";
                                            }
                                            else{
                                                itemList.Q<Label>("item" + (i + 1).ToString()).text = item.name;
                                            }
                                            i++;
                                        }
                                        string itemDescriptionToShow = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                        if(itemDescriptionToShow == ""){
                                            itemDesc.text = "no description";
                                        }
                                        else{
                                            itemDesc.text = characterInventories[currentCharacter - 1].inventory[currentItem - 1].description;
                                        }
                                        SelectCharacter(root.Q<VisualElement>("character" + currentCharacter.ToString()));
                                        itemInfo.visible = true;
                                    }
                                    else if (uiInput.moveleft)
                                    {
                                        UnselectButton(itemsButton);
                                        SelectButton(equipButton);
                                        currentSelection = PauseMenuSelectables.Equip;

                                    }
                                    else if (uiInput.moveright)
                                    {
                                        UnselectButton(itemsButton);
                                        SelectButton(skillsButton);
                                        currentSelection = PauseMenuSelectables.Skills;
                                    }
                                break;
                                case PauseMenuSelectables.Skills:
                                    VisualElement currentSkills = skillInfo.Q<VisualElement>("current_skills");
                                    Label skillDesc = skillInfo.Q<Label>("skill_desc");
                                    if (isSelected)
                                    {
                                        if(onCharacterSelect){
                                            //select a character
                                            if (uiInput.goback) { 
                                                isSelected = false;
                                                skillInfo.visible = false;
                                            }
                                        }
                                        else{
                                            if(uiInput.goselected){
                                                //do stuff with items
                                            }
                                            else if(uiInput.goback){
                                                if(playerParty.Length == 1){
                                                    
                                                    isSelected = false;
                                                    skillInfo.visible = false;
                                                    UnselectCharacter(root.Q<VisualElement>("character1"));
                                                    currentItem = 1;
                                                }
                                                else{
                                                    onCharacterSelect = true;
                                                }
                                                UnselectInfoTab(skillInfo.Q<VisualElement>("current_skills"));
                                            }
                                            else if(uiInput.movedown){
                                                if(currentItem < 5){
                                                    UnSelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                    currentItem++;
                                                    SelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                    if(characterInventories[currentCharacter - 1].equipedSkills[0].description == ""){
                                                        skillDesc.text = "No description";
                                                    }
                                                    else{
                                                        skillDesc.text = characterInventories[currentCharacter - 1].equipedSkills[0].description;
                                                    }
                                                }
                                            }
                                            else if(uiInput.moveup){
                                                if(currentItem > 1){
                                                    UnSelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                    currentItem--;
                                                    SelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                    if(characterInventories[currentCharacter - 1].equipedSkills[0].description == ""){
                                                        skillDesc.text = "No description";
                                                    }
                                                    else{
                                                        skillDesc.text = characterInventories[currentCharacter - 1].equipedSkills[0].description;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (uiInput.goselected)
                                    {
                                        
                                        isSelected = true;
                                        if(playerParty.Length == 1){
                                            onCharacterSelect = false;
                                            SelectInfoTab(currentSkills);
                                            SelectItem(currentSkills.Q<Label>("skill1"));
                                        }
                                        else{
                                            onCharacterSelect = true;
                                        }
                                        int i = 1;
                                        foreach(Skill skill in characterInventories[currentCharacter - 1].equipedSkills){
                                            if(skill.name == ""){
                                                currentSkills.Q<Label>("skill" + i.ToString()).text = "None";
                                            }
                                            else{
                                                currentSkills.Q<Label>("skill" + i.ToString()).text = skill.name;
                                            }
                                            i++;
                                        }
                                        isSelected = true;
                                        if(characterInventories[currentCharacter - 1].equipedSkills[0].description == ""){
                                            skillDesc.text = "No description";
                                        }
                                        else{
                                            skillDesc.text = characterInventories[currentCharacter - 1].equipedSkills[0].description;
                                        }
                                        SelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                        SelectCharacter(root.Q<VisualElement>("character" + currentCharacter.ToString()));
                                        skillInfo.visible = true;
                                    }
                                    else if (uiInput.moveleft)
                                    {
                                        UnselectButton(skillsButton);
                                        SelectButton(itemsButton);
                                        currentSelection = PauseMenuSelectables.Items;
                                    }
                                    else if (uiInput.moveright)
                                    {
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
                                        //select the ui
                                        //isSelected = true
                                    }
                                    else if (uiInput.moveleft)
                                    {
                                        UnselectButton(optionsButton);
                                        SelectButton(skillsButton);
                                        currentSelection = PauseMenuSelectables.Skills;
                                    }
                                break;
                            }
                            
                    }
                  }
                  
            }).Run();
        characterStatsList.Dispose();
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
    private void placeQuickMenu(Label item, VisualElement quickMenu){
        
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

}
public enum Equipment{
    Weapon,
    Armor,
    Charm
}
public enum QuickMenuSelectables{
    Switch,
    Use,
    Give,
    Drop,
    Cancel
}
public enum PauseMenuSelectables{
      Party,
      Equip,
      Items,
      Skills,
      Options
}

