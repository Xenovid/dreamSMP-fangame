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
    
    protected override void OnCreate()
    {
        isSaving = false;
        uISystem = World.GetOrCreateSystem<UISystem>();
        sceneSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SceneSystem>();
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
        LoadPlayers();
        LoadSubSceneAssests();
    }
 
    protected override void OnUpdate()
    {

        LoadSubSceneAssests();
        LoadPlayers();
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
    public void SaveSubsceneAssets(){
        
        //Entity sceneEntity = sceneSystem.GetSceneEntity(subScene.SceneGUID);
        Entities
        .WithoutBurst()
        .ForEach((Animator animator, in ToSaveTag saveTag, in ChestTag chestTag)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/chestsave" + saveTag.saveID.ToString();
            ChestSaveData chestSaveData = new ChestSaveData{isOpen = chestTag.isOpen,animationSaveData = new AnimationSaveData{ shortNameHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash, normilizedtime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime}};
            string jsonString = JsonUtility.ToJson(chestSaveData);
            File.WriteAllText(savePath, jsonString);
        }).Run();
    }
    public void LoadSubSceneAssests(){
        Entities
        .WithAll<ChestTag>()
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, Animator animator,ref ChestTag chestTag,in ChestAnimationData animationData, in ToLoadData loadData)=>{
            if(animator.isActiveAndEnabled){
                string savePath = Application.persistentDataPath + "/tempsave" + "/chestsave" + loadData.saveID.ToString();
                if(File.Exists(savePath)){
                    string jsonString = File.ReadAllText(savePath);
                    ChestSaveData chestSaveData = JsonUtility.FromJson<ChestSaveData>(jsonString);
                    animator.Play(chestSaveData.animationSaveData.shortNameHash, 0 , chestSaveData.animationSaveData.normilizedtime);  
                    chestTag.isOpen = chestSaveData.isOpen;
                }
                EntityManager.AddComponentData(entity, new ToSaveTag{saveID = loadData.saveID});
                EntityManager.RemoveComponent<ToLoadData>(entity);
            }
        }).Run();
    }
    public void LoadPlayers(){
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<PlayerTag>()
        .ForEach((Entity entity,Animator Animator,ref MovementData movementData, ref Translation translation, ref CharacterStats characterStats, ref DynamicBuffer<SkillData> skills, ref DynamicBuffer<ItemData> items, ref ToLoadData loadData) =>{
            if(Animator.isActiveAndEnabled){
                string savePath = Application.persistentDataPath +"/tempsave" + "/player" + loadData.saveID.ToString();
                if(File.Exists(savePath)){
                    string jsonString = File.ReadAllText(savePath);
                    PlayerSaveData playerSaveData = JsonUtility.FromJson<PlayerSaveData>(jsonString);
                    
                    Animator.Play(playerSaveData.animationSaveData.shortNameHash, 0 , playerSaveData.animationSaveData.normilizedtime);
                    movementData = playerSaveData.movementData;
                    translation.Value = playerSaveData.trasition;
                    characterStats = playerSaveData.characterStats;
                    skills.Clear();
                    foreach (var skill in playerSaveData.skills) skills.Add(skill);
                    items.Clear();
                    foreach(var item in playerSaveData.itemInventory) items.Add(item);
                }
                EntityManager.AddComponentData(entity, new ToSaveTag{saveID = loadData.saveID});
                EntityManager.RemoveComponent(entity, typeof(ToLoadData));
            }
            
        }).Run();
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<CaravanTag>()
        .ForEach((Entity entity, ref DynamicBuffer<WeaponData> weapons,ref DynamicBuffer<ArmorData> armors,ref DynamicBuffer<CharmData> charms,in ToLoadData loadData ) =>{
            string savePath = Application.persistentDataPath + "/tempsave"+ "/caravan" + loadData.saveID.ToString();
            if(File.Exists(savePath)){
                string jsonString = File.ReadAllText(savePath);
                CaravanSaveData caravanSaveData = JsonUtility.FromJson<CaravanSaveData>(jsonString);
                weapons.Clear();
                foreach(var weapon in caravanSaveData.weaponDatas)weapons.Add(weapon);
                armors.Clear();
                foreach(var armor in caravanSaveData.armorDatas) armors.Add(armor);
                charms.Clear();
                foreach(var charm in caravanSaveData.charmDatas) charms.Add(charm);
            }
            EntityManager.AddComponentData(entity, new ToSaveTag{saveID = loadData.saveID});
            EntityManager.RemoveComponent(entity, typeof(ToLoadData));
        }).Run();
    }
    public void SavePlayers(){
        Entities
        .WithoutBurst()
        .WithAll<PlayerTag>()
        .WithStructuralChanges()
        .ForEach((Animator Animator,in MovementData movementData, in Translation translation, in CharacterStats characterStats, in DynamicBuffer<SkillData> skills, in DynamicBuffer<ItemData> items, in ToSaveTag saveTag) =>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/player" + saveTag.saveID.ToString();
            Skill[] skillssave = new Skill[skills.Length];
            int i = 0;
            foreach(SkillData skillData in skills){
                skillssave[i] = skillData.skill;
                i++;
            }

            PlayerSaveData playerSaveData = new PlayerSaveData{
                animationSaveData = new AnimationSaveData{shortNameHash = Animator.GetCurrentAnimatorStateInfo(0).shortNameHash, normilizedtime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime},
                itemInventory = items.ToNativeArray(Unity.Collections.Allocator.Temp).ToArray(),
                skills = skills.ToNativeArray(Allocator.Temp).ToArray(),
                characterStats = characterStats,
                movementData = movementData,
                trasition = translation.Value
            };
            string jsonString = JsonUtility.ToJson(playerSaveData);
            File.WriteAllText(savePath, jsonString);
        }).Run();
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<CaravanTag>()
        .ForEach((in DynamicBuffer<WeaponData> weapons,in DynamicBuffer<ArmorData> armors,in DynamicBuffer<CharmData> charms,in ToSaveTag saveTag ) =>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/caravan" + saveTag.saveID.ToString();
            CaravanSaveData caravanSaveData = new CaravanSaveData{
                weaponDatas = weapons.ToNativeArray(Allocator.Temp).ToArray(),
                armorDatas = armors.ToNativeArray(Allocator.Temp).ToArray(),
                charmDatas = charms.ToNativeArray(Allocator.Temp).ToArray()
            };
            string jsonString = JsonUtility.ToJson(caravanSaveData);
            File.WriteAllText(savePath, jsonString);
        }).Run();
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
        Debug.Log(savePath);
        string jsonString = File.ReadAllText(savePath);
        CurrentSubScenesData subScenesData = JsonUtility.FromJson<CurrentSubScenesData>(jsonString);
        foreach(SubScene subScene in subScenesData.subScenes){
            sceneSystem.LoadSceneAsync(subScene.SceneGUID);
        }

    }
    public void LoadGame(object sender, StartGameEventArgs e){
        string savePath = Application.persistentDataPath + "/save" + e.saveFileNumber.ToString();
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
    public void SaveGame(int saveFileNumber){
        // To do save everything
        SaveAtmosphere();
        SavePlayers();
        SaveSubsceneAssets();
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
        foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/tempsave")){
            File.Delete(file);
        }
        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.EssentialsSubScene.SceneGUID);
    }

    public void LoadSaveUI(System.Object sender, SavePointEventArg e){

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



