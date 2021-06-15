using UnityEngine;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;

public class SaveAndLoadSystem : SystemBase
{
    ReferencedUnityObjects g;
    ReferencedUnityObjects test;
    StepPhysicsWorld physicsWorld;
    saveData dataHold;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    string json;
    int i = 0;
    SceneSystem sceneSystem;

    protected override void OnStartRunning()
    {
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        sceneSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SceneSystem>();
        //dataHold = new Data();
        g = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
        dataHold = new saveData();
        test = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
    }
 
    protected override void OnUpdate()
    {/*
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
        */
    }
    public void SaveSubscene(Entity sceneEntity){
        //Entity sceneEntity = sceneSystem.GetSceneEntity(subScene.SceneGUID);
        

        var seriWorld = new World("Serialization");
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        seriWorld.EntityManager.CopyEntitiesFrom(entityManager, entityManager.GetAllEntities());

        EntityQueryDesc queryDesc = new EntityQueryDesc{
            None = new ComponentType[] { typeof(ToSaveTag)},
        };
        EntityQueryDesc queryDesc2 = new EntityQueryDesc{
            All = new ComponentType[] {typeof(ToSaveTag)},
        };
        var query = seriWorld.EntityManager.CreateEntityQuery(queryDesc);

        seriWorld.EntityManager.DestroyEntity(query);
        var query3 = EntityManager.CreateEntityQuery(typeof(ToSaveTag));
        var query2 = seriWorld.EntityManager.CreateEntityQuery(typeof(ToSaveTag));
        NativeArray<Entity> saveEntities = query2.ToEntityArray(Allocator.Temp);

        foreach(Entity entity in saveEntities){
            if(seriWorld.EntityManager.GetSharedComponentData<SceneTag>(entity).SceneEntity == sceneEntity){
                seriWorld.EntityManager.RemoveComponent<SceneTag>(entity);
                seriWorld.EntityManager.RemoveComponent<SceneSection>(entity);
                seriWorld.EntityManager.RemoveComponent<EditorRenderData>(entity);
                
            }
            else{
                //seriWorld.EntityManager.DestroyEntity(entity);
            }
        }
        saveEntities.Dispose();
        
        using (var writer = new StreamBinaryWriter(Application.persistentDataPath + "/save"))
            SerializeUtilityHybrid.Serialize(seriWorld.EntityManager, writer, out g);
        SerializeUtilityHybrid.SerializeObjectReferences(g.Array,out test);
        if (test != null)
        {
            
            dataHold.Array = g.Array;
            json = JsonUtility.ToJson(test);
            PlayerPrefs.SetString("Data", json);
        }
 
        seriWorld.Dispose();
    }
    public void LoadSubScene(Entity sceneEntity){
        if (g != null)
        {
            Debug.Log("MOOOO?");
            //var data = PlayerPrefs.GetString("Data");
            //g = JsonUtility.FromJson<ReferencedUnityObjects>(data);
            //g.Array = dataHold.Array;
        }
 
        var tempWorld = new World("temp");
        var tempManager = tempWorld.EntityManager;
        SerializeUtilityHybrid.DeserializeObjectReferences(test, out g.Array);
        using (var reader = new StreamBinaryReader(Application.persistentDataPath + "/save"))
            SerializeUtilityHybrid.Deserialize(tempManager, reader, g);
        EntityManager.MoveEntitiesFrom(tempManager);//, tempManager.GetAllEntities());
        tempWorld.Dispose();
    }
    public void save()
    {
        //TODO: get everything from the temp save file and copy it over to the chosen save file
        var seriWorld = new World("Serialization");
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        seriWorld.EntityManager.CopyEntitiesFrom(entityManager, entityManager.GetAllEntities());
 
        EntityQueryDesc queryDesc = new EntityQueryDesc{
            None = new ComponentType[] {typeof(EntityFollowEntityData), typeof(Camera)},
            All = new ComponentType[] {typeof(SceneTag)}
         };
        var query = seriWorld.EntityManager.CreateEntityQuery(queryDesc);
        NativeArray<Entity> sceneEntities = query.ToEntityArray(Allocator.Temp);
        Debug.Log(sceneEntities.Length);
        foreach(Entity ent in sceneEntities){
            entityManager.RemoveComponent<SceneTag>(ent);
            entityManager.RemoveComponent<SceneSection>(ent);
            entityManager.RemoveComponent<EditorRenderData>(ent);
        }
        //seriWorld.EntityManager.DestroyEntity(query)
        var Tempworld = new World("temp");
        Tempworld.EntityManager.CopyEntitiesFrom(seriWorld.EntityManager, sceneEntities);
        sceneEntities.Dispose();
        unsafe{
        using (var writer = new StreamBinaryWriter(Application.persistentDataPath + "/save"))
            SerializeUtilityHybrid.Serialize(Tempworld.EntityManager, writer, out g);
        }
        if (g != null)
        {
            //dataHold.Array = g.Array;
            //json = JsonUtility.ToJson(dataHold);
            //PlayerPrefs.SetString("Data", json);
        }
        Tempworld.Dispose();
        seriWorld.Dispose();
    }
 
    public void load()
    {
        if (g != null)
        {
            //var data = PlayerPrefs.GetString("Data");
            //dataHold = JsonUtility.FromJson<Data>(data);
            //g.Array = dataHold.Array;
        }
 
        var oldWorld = World.DefaultGameObjectInjectionWorld;
        var newWorld = DefaultWorldInitialization.Initialize("NewWorld " + (i++));
        var newManager = newWorld.EntityManager;
        /*using (var reader = new MyStreamBinaryReader(Application.persistentDataPath + "/save"))
            SerializeUtilityHybrid.Deserialize(newManager, reader, g);
 
        BlockLibrary.Instance.MovePrefabWorld(newWorld);*/
 

        oldWorld.Dispose();
    }

    
}
[System.Serializable]
public class saveData{
    public UnityEngine.Object[] Array;
}

