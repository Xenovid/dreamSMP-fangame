using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Scenes;
using Unity.Physics.Systems;
using System.IO;
using Unity.Transforms;
using UnityEngine.UIElements;
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SaveAndLoadSystem : SystemBase
{
    ReferencedUnityObjects g;
    ReferencedUnityObjects test;
    StepPhysicsWorld physicsWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    string savePointName;
    SceneSystem sceneSystem;
    UISystem uISystem;
    int currentSaveFile;
    SaveTriggerSystem saveTriggerSystem;
    bool isSaving;
    PauseSystem pauseSystem;
    InkDisplaySystem inkDisplaySystem;
    
    protected override void OnCreate()
    {
        isSaving = false;
        uISystem = World.GetOrCreateSystem<UISystem>();
        sceneSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SceneSystem>();
        inkDisplaySystem = World.GetOrCreateSystem<InkDisplaySystem>();
        saveTriggerSystem = World.GetOrCreateSystem<SaveTriggerSystem>();

        uISystem.StartGame += LoadGame;
        uISystem.StartNewGame += LoadNewGame;
        // creates save folders if they don't exist
        CreateSaveFiles();
        foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/tempsave")){
            File.Delete(file);
        }
    }
    protected override void OnStartRunning()
    {
        pauseSystem = World.GetOrCreateSystem<PauseSystem>();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        
        g = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
        test = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
        LoadAssets();
    }
 
    protected override void OnUpdate()
    {
        LoadAssets();
        //LoadSubSceneAssests();
        //LoadPlayers();
    }
    public void ClearSaves(){
        if(Directory.Exists(Application.persistentDataPath + "/save1")){
            Debug.Log("deleting");
            foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/save1")) File.Delete(file);
            Directory.Delete(Application.persistentDataPath + "/save1");
        }
        if(Directory.Exists(Application.persistentDataPath + "/save2")){
            Debug.Log("deleting");
            foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/save1")) File.Delete(file);
            Directory.Delete(Application.persistentDataPath + "/save2");
        }
    }
    public void LoadTime(int saveFileNumber){
        string savePath = Application.persistentDataPath + "/save" + saveFileNumber.ToString() + "/SavePointData";
        string jsonString = File.ReadAllText(savePath);
        SavePointData savePointData = JsonUtility.FromJson<SavePointData>(jsonString);
        SetSingleton<SavePointData>(savePointData);
    }
    public void SaveTime(){
        string savePath = Application.persistentDataPath + "/tempsave"+ "/SavePointData";
        
        SavePointData savePointData = GetSingleton<SavePointData>();
        savePointData.savePointName = savePointName;
        string jsonString = JsonUtility.ToJson(savePointData);
        File.WriteAllText(savePath, jsonString);
    }
    public void SaveCurrentSubscenes(){
        List<SubScene> subScenes = new List<SubScene>();
        Entities
        .WithoutBurst()
        .WithNone<EssentialsSubSceneTag>()
        .ForEach((Entity entity, in SubScene subScene) =>{
            if(sceneSystem.IsSceneLoaded(entity)){
                subScenes.Add(subScene);
            }
        }).Run();
        CurrentSubScenesData subScenesData = new CurrentSubScenesData{subScenes = subScenes.ToArray()};
        string savePath = Application.persistentDataPath+ "/tempsave" + "/loadedscenes";
        string jsonString = JsonUtility.ToJson(subScenesData);
        File.WriteAllText(savePath, jsonString);
    }
    public void LoadCurrentSubscenes(int saveFileNumber){
        string savePath = Application.persistentDataPath + "/save" + saveFileNumber.ToString()+ "/loadedscenes";
        string jsonString = File.ReadAllText(savePath);
        CurrentSubScenesData subScenesData = JsonUtility.FromJson<CurrentSubScenesData>(jsonString);
        foreach(SubScene subScene in subScenesData.subScenes){
            sceneSystem.LoadSceneAsync(subScene.SceneGUID);
        }

    }
    public void LoadGame(object sender, StartGameEventArgs e){
        string savePath = Application.persistentDataPath + "/save" + e.saveFileNumber.ToString();
        CameraData cameraData = GetSingleton<CameraData>();
        cameraData.currentState = CameraState.FollingPlayer;
        SetSingleton(cameraData);
        LoadTime(e.saveFileNumber);
        // clear the temp file
        CreateSaveFiles();
        foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/tempsave")){
            File.Delete(file);
        }
        // load the temp file up with the data
        foreach(string file in Directory.GetFiles(savePath)){
            string fileSavePath = Path.Combine(Application.persistentDataPath + "/tempsave", Path.GetFileName(file));
            File.Copy(file, fileSavePath);
        }
        LoadCurrentSubscenes(e.saveFileNumber);
        LoadAtmosphere();
        LoadStory();
        // loads the players
        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.EssentialsSubScene.SceneGUID);
        
    }
    public void CreateSaveFiles(){
        if(!Directory.Exists(Application.persistentDataPath + "/tempsave")) Directory.CreateDirectory(Application.persistentDataPath + "/tempsave");
        if(!Directory.Exists(Application.persistentDataPath + "/save1")) Directory.CreateDirectory(Application.persistentDataPath + "/save1");
    }
    public void UnLoadSubScenes(){
        
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, in SubScene subScene) =>{
            if(!(subScene.SceneGUID == SubSceneReferences.Instance.EssentialsSubScene.SceneGUID)){
                sceneSystem.UnloadScene(subScene.SceneGUID);
            }
        }).Run();
    }
    public void LoadLastSavePoint(){
        Entities
        .WithAll<PlayerTag>()
        .WithoutBurst()
        .ForEach((ref Translation translation, in ToSaveTag saveTag) =>{
            string savePath = Application.persistentDataPath + "/tempsave" +  "/player" + saveTag.saveID.ToString();
            string jsonString = File.ReadAllText(savePath);
            PlayerSaveData playerSaveData = JsonUtility.FromJson<PlayerSaveData>(jsonString);
            translation.Value = playerSaveData.trasition;
        }).Run();

        LoadAtmosphere();
        string savePath = Application.persistentDataPath + "/tempsave" +  "/loadedscenes";
        string jsonString = File.ReadAllText(savePath);
        CurrentSubScenesData subScenesData = JsonUtility.FromJson<CurrentSubScenesData>(jsonString);
        foreach(SubScene subScene in subScenesData.subScenes){
            sceneSystem.LoadSceneAsync(subScene.SceneGUID);
        }
    }
    public void SaveAssets(){
        //save position
        Entities
        .WithoutBurst()
        .ForEach((in Translation translation, in ToSaveTag saveTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/traslationsave" + saveTag.saveID.ToString();
            string jsonString = JsonUtility.ToJson(translation);
            File.WriteAllText(savePath, jsonString);
        }).Run();
        //save character stats
        Entities
        .WithoutBurst()
        .ForEach((in CharacterStats characterStats, in ToSaveTag saveTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/charactersave" + saveTag.saveID.ToString();
            string jsonString = JsonUtility.ToJson(characterStats);
            File.WriteAllText(savePath, jsonString);
        }).Run();
        //save skills
        Entities
        .WithoutBurst()
        .ForEach((in DynamicBuffer<PolySkillData> skills, in ToSaveTag saveTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/skillsave" + saveTag.saveID.ToString();
            SkillSaveData skillSaveData = new SkillSaveData{polySkillDatas = skills.ToNativeArray(Allocator.Temp).ToArray()};
            string jsonString = JsonUtility.ToJson(skillSaveData);
            File.WriteAllText(savePath, jsonString);

        }).Run();
        //save inventory
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<CaravanTag>()
        .ForEach((in DynamicBuffer<WeaponData> weapons,in DynamicBuffer<ArmorData> armors,in DynamicBuffer<CharmData> charms, in DynamicBuffer<ItemData> items, in ToSaveTag saveTag ) =>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/caravan" + saveTag.saveID.ToString();
            CaravanSaveData caravanSaveData = new CaravanSaveData{
                weaponDatas = weapons.ToNativeArray(Allocator.Temp).ToArray(),
                armorDatas = armors.ToNativeArray(Allocator.Temp).ToArray(),
                charmDatas = charms.ToNativeArray(Allocator.Temp).ToArray(),
                itemDatas = items.ToNativeArray(Allocator.Temp).ToArray()
            };
            string jsonString = JsonUtility.ToJson(caravanSaveData);
            File.WriteAllText(savePath, jsonString);
        }).Run();
        //save iteractives
        Entities
        .WithoutBurst()
        .ForEach((in PolyInteractiveData interactive, in ToSaveTag saveTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/interactivesave" + saveTag.saveID.ToString();
            string jsonString = JsonUtility.ToJson(interactive);
            File.WriteAllText(savePath, jsonString);
        }).Run();
        //save movement data
        Entities
        .WithoutBurst()
        .ForEach((in MovementData movementData, in ToSaveTag saveTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/interactivesave" + saveTag.saveID.ToString();
            string jsonString = JsonUtility.ToJson(movementData);
            File.WriteAllText(savePath, jsonString);
        }).Run();
        //save cutscenetriggerdata
        Entities
        .WithoutBurst()
        .ForEach((in CutsceneTriggerTag cutsceneTriggerTag, in ToSaveTag saveTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/cutscenetriggersave" + saveTag.saveID.ToString();
            string jsonString = JsonUtility.ToJson(cutsceneTriggerTag);
            File.WriteAllText(savePath, jsonString);
        }).Run();
        //save animation
        Entities
        .WithoutBurst()
        .ForEach((Animator animator, in ToSaveTag saveTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/animationsave" + saveTag.saveID.ToString();
            AnimationSaveData animationSaveData = new AnimationSaveData{ name = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, normilizedtime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime};
            string jsonString = JsonUtility.ToJson(animationSaveData);
            File.WriteAllText(savePath, jsonString);
        }).Run();
    }
    public void LoadAssets(){
        //load character stats
        Entities
        .WithoutBurst()
        .ForEach((ref CharacterStats characterStats, in ToLoadData loadTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/charactersave" + loadTag.saveID.ToString();
            if(File.Exists(savePath) && loadTag.waitedFrame){
                
                string jsonString = File.ReadAllText(savePath);
                characterStats = JsonUtility.FromJson<CharacterStats>(jsonString);
                File.WriteAllText(savePath, jsonString);
            }
        }).Run();
        //load position
        Entities
        .WithoutBurst()
        .ForEach((ref Translation translation, in ToLoadData loadTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/traslationsave" + loadTag.saveID.ToString();
            if(File.Exists(savePath) && loadTag.waitedFrame){
                string jsonString =  File.ReadAllText(savePath);
                translation = JsonUtility.FromJson<Translation>(jsonString);
            }
        }).Run();
        //load skills
        Entities
        .WithoutBurst()
        .ForEach((ref DynamicBuffer<PolySkillData> skills, in ToLoadData loadTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/skillsave" + loadTag.saveID.ToString();
            if(File.Exists(savePath) && loadTag.waitedFrame){
                string jsonString = File.ReadAllText(savePath);
                skills.Clear();
                SkillSaveData savedSkills = JsonUtility.FromJson<SkillSaveData>(jsonString);
                foreach(PolySkillData skill in savedSkills.polySkillDatas){
                    skills.Add(skill);
                }
            }
        }).Run();
        //load inventory
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<CaravanTag>()
        .ForEach((ref DynamicBuffer<WeaponData> weapons,ref DynamicBuffer<ArmorData> armors,ref DynamicBuffer<CharmData> charms, ref DynamicBuffer<ItemData> items, in ToLoadData loadTag ) =>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/caravan" + loadTag.saveID.ToString();
            if(File.Exists(savePath) && loadTag.waitedFrame){
                string jsonString = File.ReadAllText(savePath);
                CaravanSaveData caravanSaveData = JsonUtility.FromJson<CaravanSaveData>(jsonString);
                weapons.Clear();
                foreach(WeaponData weapon in caravanSaveData.weaponDatas){
                    weapons.Add(weapon);
                }
                armors.Clear();
                foreach(ArmorData armor in caravanSaveData.armorDatas){
                    armors.Add(armor);
                }
                charms.Clear();
                foreach(CharmData charm in caravanSaveData.charmDatas){
                    charms.Add(charm);
                }
                items.Clear();
                foreach(ItemData item in caravanSaveData.itemDatas){
                    items.Add(item);
                }
            }
        }).Run();
        //load iteractives
        Entities
        .WithoutBurst()
        .ForEach((Entity entity, PolyInteractiveData interactive, in ToLoadData loadTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/interactivesave" + loadTag.saveID.ToString();
            if(File.Exists(savePath) && loadTag.waitedFrame){
                string jsonString = File.ReadAllText(savePath);
                interactive = JsonUtility.FromJson<PolyInteractiveData>(jsonString);
                EntityManager.SetComponentData(entity, interactive);
                //Debug.Log(interactive.SharedInteractiveData.isTriggered);
            }
        }).Run();
        //load movement data
        Entities
        .WithoutBurst()
        .ForEach((ref MovementData movementData, in ToLoadData loadTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/interactivesave" + loadTag.saveID.ToString();
            if(File.Exists(savePath) && loadTag.waitedFrame){
                string jsonString = File.ReadAllText(savePath);
                movementData = JsonUtility.FromJson<MovementData>(jsonString);
            }
        }).Run();
        //load animation
        Entities
        .WithoutBurst()
        .ForEach((Entity entity, Animator animator, in ToLoadData loadTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/animationsave" + loadTag.saveID.ToString();
            if(File.Exists(savePath) && loadTag.waitedFrame){
                string jsonString = File.ReadAllText(savePath);
                AnimationSaveData animationSaveData = JsonUtility.FromJson<AnimationSaveData>(jsonString);
                animator.Play(animationSaveData.name, 0, animationSaveData.normilizedtime);
            }
        }).Run();
        Entities
        .WithoutBurst()
        .ForEach((ref CutsceneTriggerTag cutsceneTriggerTag, in ToLoadData loadTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/cutscenetriggersave" + loadTag.saveID.ToString();
            if(File.Exists(savePath) && loadTag.waitedFrame){
                string jsonString = File.ReadAllText(savePath);
                CutsceneTriggerTag triggersave = JsonUtility.FromJson<CutsceneTriggerTag>(jsonString);
                cutsceneTriggerTag = triggersave;
            }
        }).Run();
        Entities
        .WithStructuralChanges()
        .WithoutBurst()
        .ForEach((Entity entity, ref ToLoadData loadTag) =>{
            if(loadTag.waitedFrame){
                EntityManager.AddComponentData(entity, new ToSaveTag{saveID = loadTag.saveID});
                EntityManager.RemoveComponent<ToLoadData>(entity);
            }
            else{
                loadTag.waitedFrame = true;
            }
            
        }).Run();   
    }
    public void SaveStory(){
        //save story
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((InkManagerData inkManager) => {
            string savePath = Application.persistentDataPath + "/tempsave" + "/storysave";
            string jsonString = inkManager.inkStory.state.ToJson();
            File.WriteAllText(savePath, jsonString);
        }).Run();
    }
    public void LoadStory(){
        //load story
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((InkManagerData inkManager) => {
            string savePath = Application.persistentDataPath + "/tempsave" + "/storysave";
            string jsonString = File.ReadAllText(savePath);
            inkManager.inkStory.state.LoadJson(jsonString);
        }).Run();
    }
    public void SaveGame(int saveFileNumber){
        // To do save everything
        SaveAtmosphere();
        SaveAssets();
        SaveStory();
        SaveCurrentSubscenes();
        SaveTime();
        // gets everything in the temp folder and pastes it into the save
        
        string savePath = Application.persistentDataPath + "/save" + saveFileNumber.ToString();
        foreach(string file in Directory.GetFiles(savePath)){
            File.Delete(file);
        }
        foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/tempsave")){
            string fileSavePath = Path.Combine(savePath, Path.GetFileName(file));
            File.Copy(file, fileSavePath);
        }
    }

    public void SaveActionMap(ActionMap actionMap){

    }
    public void SaveAtmosphere(){
        string savePath = Application.persistentDataPath+ "/tempsave" + "/atmosphere.json";
        OverworldAtmosphereData overworldAtmosphereData = GetSingleton<OverworldAtmosphereData>();
        string jsonString = JsonUtility.ToJson(overworldAtmosphereData);
        File.WriteAllText(savePath, jsonString);
    }
    public void LoadAtmosphere(){
        string savePath = Application.persistentDataPath+ "/tempsave" + "/atmosphere.json";
        string jsonString = File.ReadAllText(savePath); 
        OverworldAtmosphereData overworldAtmosphereData = JsonUtility.FromJson<OverworldAtmosphereData>(jsonString);;//songName = saveData.songName};
        SetSingleton<OverworldAtmosphereData>(overworldAtmosphereData);
        AudioManager.playSong(overworldAtmosphereData.songName.ToString());
    }
    public void LoadNewGame(object sender, System.EventArgs e){
        CameraData cameraData = GetSingleton<CameraData>();
        cameraData.currentState = CameraState.FreeForm;
        SetSingleton(cameraData);
        foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/tempsave")){
            File.Delete(file);
        }
        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.EssentialsSubScene.SceneGUID);
    }

    
    public void LoadSaveUI(System.Object sender, SavePointEventArg e){
        inkDisplaySystem.DisplayingChoices = false;
        inkDisplaySystem.DisableTextboxUI();
        uISystem.overworldOverlay.visible = false;
        savePointName = e.savePointName;
        // pause the world
        
        pauseSystem.Pause();

        VisualElement root = uISystem.root;
        VisualElement fileSelectUI = root.Q<VisualElement>("overworld_file_select");

        UpdateSaveFileUI(fileSelectUI);
        fileSelectUI.visible = true;
    }
    public void UpdateSaveFileUI(VisualElement saveFileUI){
        saveFileUI.visible = true;
        bool selectedFile = false;
        for(int i = 1; i <= 2; i++){
            TemplateContainer fileContainer = saveFileUI.Q<TemplateContainer>("save_file" + i.ToString());
            Button currentFile = fileContainer.Q<Button>("background");
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
                }
            }
            else{
                currentFile.SetEnabled(false);
            }

        }
    }
    public void UpdateSaveFile(Button currentFile, int saveFileNumber){
        AudioManager.playSound("menuselect");
        Label currentTime = currentFile.Q<Label>("time");
        string savePath = Application.persistentDataPath + "/save" + saveFileNumber.ToString() + "/SavePointData";
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
    }
}
[System.Serializable]
public class SkillSaveData{
    public PolySkillData[] polySkillDatas;
}


