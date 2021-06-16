using UnityEngine;
using Unity.Entities;
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

    protected override void OnStartRunning()
    {
        Debug.Log("initializing");
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        sceneSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SceneSystem>();
        //dataHold = new Data();
        g = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
        test = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
        LoadPlayers();

    }
 
    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;

        if(input.select){
            EntityQuery playerQuery = GetEntityQuery(typeof(PlayerTag), typeof(MovementData));
            MovementData playerMovment = playerQuery.GetSingleton<MovementData>();

            Entity caravan = GetSingletonEntity<CaravanTag>();
            DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
            DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
            DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);

            Entity messageBoard = GetSingletonEntity<OverworldUITag>();
            DynamicBuffer<Text> texts = GetBuffer<Text>(messageBoard);

            foreach(TriggerEvent triggerEvent in triggerEvents)
            {
                
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                if (HasComponent<SavePointTag>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
                {
                    SaveSubscene(EntityManager.GetSharedComponentData<SceneTag>(entityA).SceneEntity);
                    
                }
                else if (HasComponent<SavePointTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
                {
                    SaveSubscene(EntityManager.GetSharedComponentData<SceneTag>(entityB).SceneEntity);
                }
            }
            m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
        else if(Input.GetKeyDown(KeyCode.I)){
            LoadSubScene(sceneSystem.GetSceneEntity(SubSceneReferences.Instance.WorldSubScene.SceneGUID));
        }
    }
    public void SaveSubscene(Entity sceneEntity){
        //Entity sceneEntity = sceneSystem.GetSceneEntity(subScene.SceneGUID);
        Debug.Log("saving");
        Entities
        .WithoutBurst()
        .ForEach((Animator animator, in ToSaveTag saveTag, in ChestTag chestTag)=>{
            string savePath = Application.persistentDataPath + "/chestsave" + saveTag.saveID.ToString();
            ChestSaveData chestSaveData = new ChestSaveData{isOpen = chestTag.isOpen,animationSaveData = new AnimationSaveData{ shortNameHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash, normilizedtime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime}};
            string jsonString = JsonUtility.ToJson(chestSaveData);
            File.WriteAllText(savePath, jsonString);
        }).Run();
        Entities
        .WithoutBurst()
        .WithAll<PlayerTag>()
        .ForEach((Animator Animator,in MovementData movementData, in Translation translation, in CharacterStats characterStats, in DynamicBuffer<SkillData> skills, in DynamicBuffer<ItemData> items, in ToSaveTag saveTag) =>{
            string savePath = Application.persistentDataPath + "/player" + saveTag.saveID.ToString();
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
    public void LoadSubScene(Entity sceneEntity){
        Entities
        .WithAll<ChestTag>()
        .WithoutBurst()
        .ForEach((Animator animator,ref ChestTag chestTag, in ToSaveTag saveTag)=>{
            Debug.Log("loading");
            
            string savePath = Application.persistentDataPath + "/chestsave" + saveTag.saveID.ToString();
            string jsonString = File.ReadAllText(savePath);
            ChestSaveData chestSaveData = JsonUtility.FromJson<ChestSaveData>(jsonString);
            animator.Play(chestSaveData.animationSaveData.shortNameHash, 0 , chestSaveData.animationSaveData.normilizedtime);
            chestTag.isOpen = chestSaveData.isOpen;
        }).Run();
        
    }
    public void LoadPlayers(){
        Entities
        .WithoutBurst()
        .WithAll<PlayerTag>()
        .ForEach((Entity entity,Animator Animator,ref MovementData movementData, ref Translation translation, ref CharacterStats characterStats, ref DynamicBuffer<SkillData> skills, ref DynamicBuffer<ItemData> items, ref ToLoadData loadData) =>{
            string savePath = Application.persistentDataPath + "/player" + loadData.saveID.ToString();
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
    
}

