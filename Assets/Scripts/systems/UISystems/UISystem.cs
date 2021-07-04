using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.IO;
using Unity.Scenes;
[AlwaysUpdateSystem]
public class UISystem : SystemBase
{
    public event StartGameEventHandler StartGame;
    public event EventHandler StartNewGame;
    SceneSystem sceneSystem;
    SettingsMenuSystem settingsMenuSystem;
    VisualElement root;
    SaveAndLoadSystem saveAndLoadSystem;

    VisualElement titleBackground;
    VisualElement creditsBackground;
    VisualElement fileSelectBackground;
    VisualElement settingsBackground;
    Button settingsBackButton;
    EntityQuery caravanQuery;
    protected override void OnCreate()
    {
        caravanQuery = GetEntityQuery(typeof(CaravanTag));
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        saveAndLoadSystem = World.GetOrCreateSystem<SaveAndLoadSystem>();

        settingsMenuSystem = World.GetOrCreateSystem<SettingsMenuSystem>();
    }
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .WithAll<TitleMenuTag, UILoadTag>()
        .WithStructuralChanges()
        .ForEach((Entity entity, UIDocument UIDoc) =>{
            if(UIDoc.rootVisualElement != null){
                AudioManager.playSong("menuMusic");
                root = UIDoc.rootVisualElement;
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

                fileSelectBackground.Q<TemplateContainer>("save_file1").Q<Button>("background").clicked += () => SaveButton(1);
                fileSelectBackground.Q<TemplateContainer>("save_file2").Q<Button>("background").clicked += () => SaveButton(2);
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
                bindingsButton.RegisterCallback<FocusEvent>(ev => ActivateSettingsTab(settingsBackground.Q<VisualElement>("bindings_controls")));
                Button othersButton = settingsBackground.Q<Button>("others_button");
                othersButton.RegisterCallback<FocusEvent>(ev => ActivateSettingsTab(settingsBackground.Q<VisualElement>("other_controls")));
                }
                // for pause menu
                
                
                EntityManager.RemoveComponent<UILoadTag>(entity);
            }
        }).Run();
    }
    public void testfunction(){
        Debug.Log("zoging");
    }
    public void StartButton(){
        SetSingleton<OverworldAtmosphereData>(new OverworldAtmosphereData{songName = "CalmMountain"});
        AudioManager.playSound("menuselect");
        StartNewGame?.Invoke(this, EventArgs.Empty);
        AudioManager.playSong("CalmMountain");
        InputGatheringSystem.currentInput = CurrentInput.overworld;
        sceneSystem.UnloadScene(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
        AudioManager.stopSong("menuMusic");
        //isLinked = false;
    }
    public void continueButton(){
        Entities
        .WithoutBurst()
        .ForEach((in TitleMenuTag titleUIData) =>{
            AudioManager.playSound("menuselect");
            UpdateSaveFileUI(fileSelectBackground);                
                                
            fileSelectBackground.visible = true;
            titleBackground.visible = false;
        }).Run();    
    }
    public void OptionsButton(){
        AudioManager.playSound("menuselect");

        settingsBackButton.clicked += ToTitleSettingsBackButton;

        titleBackground.visible = false;
        settingsBackground.visible = true;
        //settingsMenuSystem.ActivateMenu();
    }
    public void ToTitleSettingsBackButton(){
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
    public void UpdateSaveFileUI(VisualElement saveFileUI){
        bool selectedFile = false;
        for(int i = 1; i <= 2; i++){
            TemplateContainer fileContainer = saveFileUI.Q<TemplateContainer>("save_file" + i.ToString());
            VisualElement currentFile = fileContainer.Q<VisualElement>("background");
            if(File.Exists(Application.persistentDataPath + "/save" + i.ToString() + "/SavePointData")){
                Label currentTime = currentFile.Q<Label>("time");
                
                string savePath = Application.persistentDataPath + "/save" + i.ToString() + "/SavePointData";
                string jsonString = File.ReadAllText(savePath);
                SavePointData savePointData = JsonUtility.FromJson<SavePointData>(jsonString);

                float remainder = savePointData.timePassed;
                int hours = (int) remainder / 3600;
                remainder -= (hours * 3600);
                int minutes = (int) remainder / 60;
                remainder -= minutes * 60;
                int seconds = (int) remainder;
                                    
                currentTime.text = "Time: " + hours.ToString() + " : " + minutes.ToString() + " : " + seconds.ToString();

                Label location = currentFile.Q<Label>("location");
                location.text = savePointData.savePointName.ToString();

                if(!selectedFile){
                    currentFile.Focus();
                    selectedFile = true;
                }
            }
            else{
                currentFile.focusable = false;
            }

        }
    }
    public void SaveButton(int saveFileNubmer){
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
    public void EquipmentButton(int characterNumber){
        Entity caravan = caravanQuery.GetSingletonEntity();
        DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
        DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
        DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);
        AudioManager.playSound("menuselect");
            if(playerParty.Length == 1){
                                        
                onCharacterSelect = false;
                SelectInfoTab(equipmentInfo.Q<VisualElement>("current_equipment"));
                SelectItem(equipmentInfo.Q<Label>("current_weapon"));
                selectedEquipment = Equipment.Weapon;
            }
            else{
            }
            currentWeaponLabel.text = "Weapon: " + characterStatsList[currentCharacter -1].equipedWeapon.name;
            currentArmorLabel.text = "Armor: " + characterStatsList[currentCharacter -1].equipedArmor.name;
            currentCharmLabel.text = "Charm: " + characterStatsList[currentCharacter -1].equipedCharm.name;
            currentEquipment.visible = true;
            equipmentDesc.text = characterStatsList[currentCharacter - 1].equipedWeapon.description.ToString();
            equipmentInfo.visible = true;
    }
    public delegate void StartGameEventHandler(object sender, StartGameEventArgs e);
}
