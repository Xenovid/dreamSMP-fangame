using UnityEngine;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using Unity.Collections;

public class SaveAndLoadSystem : MonoBehaviour
{
    ReferencedUnityObjects g;
    //Data dataHold;
    string json;
    int i = 0;
 
    void Start()
    {
        //dataHold = new Data();
        g = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
    }
 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            save();
        if (Input.GetKeyDown(KeyCode.F2))
            load();
    }
 
    public void save()
    {
        print("save");
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
 
        seriWorld.Dispose();
    }
 
    public void load()
    {
        print("load");
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

