using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using Unity.Entities.Serialization;
using Unity.Collections;
using Unity.Scenes;
using Unity.Physics;
using Unity.Physics.Systems;
using System.IO;
using Unity.Transforms;
public class SaveAndLoadSystem : SystemBase
{
    ReferencedUnityObjects g;
    ReferencedUnityObjects test;
    StepPhysicsWorld physicsWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    string json;
    SceneSystem sceneSystem;
    TitleScreenSystem titleSystem;
    protected override void OnCreate()
    {
        titleSystem = World.GetOrCreateSystem<TitleScreenSystem>();
        sceneSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SceneSystem>();

        titleSystem.StartGame += LoadGame;
        titleSystem.StartNewGame += LoadNewGame;
        // creates save folders if they don't exist
        CreateSaveFiles();
        foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/tempsave")){
            File.Delete(file);
        }
    }
    protected override void OnStartRunning()
    {
        Debug.Log("initializing");
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        
        //dataHold = new Data();
        g = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
        test = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
        LoadPlayers();
        LoadSubSceneAssests();
    }
 
    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;

        if(input.select){
            EntityQuery playerQuery = GetEntityQuery(typeof(PlayerTag), typeof(MovementData));
            MovementData playerMovment = playerQuery.GetSingleton<MovementData>();


            foreach(TriggerEvent triggerEvent in triggerEvents)
            {
                
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                if (HasComponent<SavePointTag>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
                {
                    SaveGame(1);
                    
                }
                else if (HasComponent<SavePointTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
                {
                    SaveGame(1);
                }
            }
            m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
        if(Input.GetKeyDown(KeyCode.I)){
            tempReLoad();
        }
        if(Input.GetKeyDown(KeyCode.F1)){
            ClearSaves();
        }
        LoadSubSceneAssests();
        LoadPlayers();
    }
    public void ClearSaves(){
        Directory.Delete(Application.persistentDataPath + "/save1");
        Directory.Delete(Application.persistentDataPath + "/save2");
    }
    public void SaveSubsceneAssets(){
        //Entity sceneEntity = sceneSystem.GetSceneEntity(subScene.SceneGUID);
        Debug.Log("saving");
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
        .ForEach((Entity entity, Animator animator,ref ChestTag chestTag, in ToLoadData loadData)=>{
            string savePath = Application.persistentDataPath + "/tempsave" + "/chestsave" + loadData.saveID.ToString();
            if(File.Exists(savePath)){
                string jsonString = File.ReadAllText(savePath);
                ChestSaveData chestSaveData = JsonUtility.FromJson<ChestSaveData>(jsonString);
                animator.Play(chestSaveData.animationSaveData.shortNameHash, 0 , chestSaveData.animationSaveData.normilizedtime);
                chestTag.isOpen = chestSaveData.isOpen;
            }
            EntityManager.AddComponentData(entity, new ToSaveTag{saveID = loadData.saveID});
            EntityManager.RemoveComponent<ToLoadData>(entity);
        }).Run();
    }
    public void LoadPlayers(){
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<PlayerTag>()
        .ForEach((Entity entity,Animator Animator,ref MovementData movementData, ref Translation translation, ref CharacterStats characterStats, ref DynamicBuffer<SkillData> skills, ref DynamicBuffer<ItemData> items, ref ToLoadData loadData) =>{
            string savePath = Application.persistentDataPath +"/tempsave" + "/player" + loadData.saveID.ToString();
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
            EntityManager.AddComponentData(entity, new ToSaveTag{saveID = loadData.saveID});
            EntityManager.RemoveComponent(entity, typeof(ToLoadData));
        }).Run();
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<CaravanTag>()
        .ForEach((Entity entity, ref DynamicBuffer<WeaponData> weapons,ref DynamicBuffer<ArmorData> armors,ref DynamicBuffer<CharmData> charms,in ToLoadData loadData ) =>{
            string savePath = Application.persistentDataPath + "/caravan" + loadData.saveID.ToString();
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
            string savePath = Application.persistentDataPath + "/caravan" + saveTag.saveID.ToString();
            CaravanSaveData caravanSaveData = new CaravanSaveData{
                weaponDatas = weapons.ToNativeArray(Allocator.Temp).ToArray(),
                armorDatas = armors.ToNativeArray(Allocator.Temp).ToArray(),
                charmDatas = charms.ToNativeArray(Allocator.Temp).ToArray()
            };
            string jsonString = JsonUtility.ToJson(caravanSaveData);
            File.WriteAllText(savePath, jsonString);
        }).Run();
    }
    public void SaveCurrentSubscenes(){
        List<SubScene> subScenes = new List<SubScene>();
        Entities
        .WithoutBurst()
        .ForEach((Entity entity, in SubScene subScene) =>{
            if(sceneSystem.IsSceneLoaded(entity)){
                subScenes.Add(subScene);
            }
        }).Run();
        CurrentSubScenesData subScenesData = new CurrentSubScenesData{subScenes = subScenes.ToArray()};
        string savePath = Application.persistentDataPath + "/loadedscenes";
        string jsonString = JsonUtility.ToJson(subScenesData);
        File.WriteAllText(savePath, jsonString);
    }
    public void LoadCurrentSubscenes(){
        string savePath = Application.persistentDataPath + "/loadedscenes";
        string jsonString = File.ReadAllText(savePath);
        CurrentSubScenesData subScenesData = JsonUtility.FromJson<CurrentSubScenesData>(jsonString);
        foreach(SubScene subScene in subScenesData.subScenes){
            sceneSystem.LoadSceneAsync(subScene.SceneGUID);
        }

    }
    public void LoadGame(object sender, StartGameEventArgs e){
        string savePath = Application.persistentDataPath + "/save" + e.saveFileNumber.ToString();
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
        LoadCurrentSubscenes();
        // loads the players
        //sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.PlayerSubScene.SceneGUID);
        
    }
    public void CreateSaveFiles(){
        if(!Directory.Exists(Application.persistentDataPath + "/tempsave")) Directory.CreateDirectory(Application.persistentDataPath + "/tempsave");
        if(!Directory.Exists(Application.persistentDataPath + "/save1")) Directory.CreateDirectory(Application.persistentDataPath + "/save1");
    }
    public void tempReLoad(){
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, in SubScene subScene) =>{
            if(sceneSystem.IsSceneLoaded(entity)){
                sceneSystem.UnloadScene(subScene.SceneGUID);
            }
        }).Run();
        LoadGame(this, new StartGameEventArgs{saveFileNumber = 1});
    }
    public void SaveGame(int saveFileNumber){
        // To do save everything
        SavePlayers();
        SaveSubsceneAssets();
        SaveCurrentSubscenes();
        // gets everything in the temp folder and pastes it into the save
        
        string savePath = Application.persistentDataPath + "/save" + saveFileNumber.ToString();
        // creates the save file that can be used to load in the main menu
        if(!Directory.Exists(savePath)){
            Directory.CreateDirectory(savePath);
        }
        foreach(string file in Directory.GetFiles(savePath)){
            File.Delete(file);
        }
        foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/tempsave")){
            string fileSavePath = Path.Combine(savePath, Path.GetFileName(file));
            File.Copy(file, fileSavePath);
        }
    }

    public void LoadNewGame(object sender, System.EventArgs e){
        foreach(string file in Directory.GetFiles(Application.persistentDataPath + "/tempsave")){
            File.Delete(file);
        }
        sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
    }
}

