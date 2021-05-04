using Unity.Entities;
using UnityEngine.UIElements;
using Unity.Scenes;
using Unity.Collections;
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

        EntityQuery characterStatsQuery = GetEntityQuery(typeof(CharacterStats));
        NativeArray<CharacterStats> characterStatsList = characterStatsQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((in UIDocument UIDoc,in PauseMenuTag pausMenuTag) =>{
                  VisualElement root = UIDoc.rootVisualElement;
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

                                            }
                                            else{
                                                SelectCharacter(uiInput, playerParty.Length, characterSelection);
                                            }
                                        }
                                    }
                                    else{
                                        Label currentWeapon = equipmentInfo.Q<Label>("current_weapon");
                                        Label currentArmor = equipmentInfo.Q<Label>("current_armor");
                                        Label currentCharm = equipmentInfo.Q<Label>("current_charm");
                                        if(uiInput.goselected){
                                            //do stuff with the item
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
                                                    UnSelectItem(currentWeapon);
                                                break;
                                                case Equipment.Armor:
                                                    UnSelectItem(currentArmor);
                                                break;
                                                case Equipment.Charm:
                                                    UnSelectItem(currentCharm);
                                                break;
                                            }
                                        }
                                        else if(uiInput.movedown){
                                            switch(selectedEquipment){
                                                case Equipment.Weapon:
                                                    selectedEquipment = Equipment.Armor;
                                                    UnSelectItem(currentWeapon);
                                                    SelectItem(currentArmor);
                                                break;
                                                case Equipment.Armor:
                                                    selectedEquipment = Equipment.Charm;
                                                    UnSelectItem(currentArmor);
                                                    SelectItem(currentCharm);
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
                                                    UnSelectItem(currentArmor);
                                                    SelectItem(currentWeapon);
                                                break;
                                                case Equipment.Charm:
                                                    selectedEquipment = Equipment.Armor;
                                                    UnSelectItem(currentCharm);
                                                    SelectItem(currentArmor);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (uiInput.goselected)
                                {
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
                                        VisualElement itemList = itemInfo.Q<VisualElement>("item_list");
                                        if(uiInput.goselected){
                                            //do stuff with items
                                        }
                                        else if(uiInput.goback){
                                            if(playerParty.Length == 1){
                                                isSelected = false;
                                                itemInfo.visible = false;
                                                UnselectCharacter(root.Q<VisualElement>("character1"));
                                            }
                                            else{
                                                onCharacterSelect = true;
                                            }
                                            UnselectInfoTab(itemInfo.Q<VisualElement>("item_list"));
                                        }
                                        else if(uiInput.movedown){
                                            if(currentItem < 6){
                                                UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                currentItem++;
                                                SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                            }
                                        }
                                        else if(uiInput.moveup){
                                            if(currentItem > 1){
                                                UnSelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                                currentItem--;
                                                SelectItem(itemList.Q<Label>("item" + currentItem.ToString()));
                                            }
                                        }
                                    }
                                    
                                }
                                else if (uiInput.goselected)
                                {
                                    VisualElement itemList = itemInfo.Q<VisualElement>("item_list");
                                    isSelected = true;
                                    if(playerParty.Length == 1){
                                        onCharacterSelect = false;
                                        SelectInfoTab(itemList);
                                        SelectItem(itemList.Q<Label>("item1"));
                                    }
                                    else{
                                        onCharacterSelect = true;
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
                                        VisualElement currentSkills = skillInfo.Q<VisualElement>("current_skills");
                                        if(uiInput.goselected){
                                            //do stuff with items
                                        }
                                        else if(uiInput.goback){
                                            if(playerParty.Length == 1){
                                                
                                                isSelected = false;
                                                skillInfo.visible = false;
                                                UnselectCharacter(root.Q<VisualElement>("character1"));
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
                                            }
                                        }
                                        else if(uiInput.moveup){
                                            if(currentItem > 1){
                                                UnSelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                                currentItem--;
                                                SelectItem(currentSkills.Q<Label>("skill" + currentItem.ToString()));
                                            }
                                        }
                                    }
                                }
                                else if (uiInput.goselected)
                                {
                                    VisualElement currentSkills = skillInfo.Q<VisualElement>("current_skills");
                                    isSelected = true;
                                    if(playerParty.Length == 1){
                                        onCharacterSelect = false;
                                        SelectInfoTab(currentSkills);
                                        SelectItem(currentSkills.Q<Label>("skill1"));
                                    }
                                    else{
                                        onCharacterSelect = true;
                                    }
                                    isSelected = true;
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
public enum PauseMenuSelectables{
      Party,
      Equip,
      Items,
      Skills,
      Options
}

