using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Collections;
using Unity.Scenes;
using UnityEngine;

public class SaveAndLoadSystem : SystemBase
{
    ReferencedUnityObjects test;
    SceneSystem tes;
    protected override void OnStartRunning()
    {
        
        tes = World.GetOrCreateSystem<SceneSystem>();
        test = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
    }
    protected override void OnUpdate()
    {
        
        
        if(Input.GetKeyDown(KeyCode.A)){
            EntityManager.CompleteAllJobs();
            Save();
        }
        
    }
    public void Save(){
        /*tes.GetSceneEntity(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
        
        World testWorld = new World("hi");
        NativeArray<Entity> ent =  World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery( typeof(CaravanTag), typeof(SceneTag)).ToEntityArray(Unity.Collections.Allocator.Persistent);
        testWorld.EntityManager.CopyEntitiesFrom(World.DefaultGameObjectInjectionWorld.EntityManager, ent);; 
         
        //testWorld.EntityManager.MoveEntitiesFrom(World.DefaultGameObjectInjectionWorld.EntityManager);

        using( var writer = new StreamBinaryWriter(Application.dataPath + "/save")){
            SerializeUtility.SerializeWorld(testWorld.EntityManager,writer);
            //SerializeUtilityHybrid.Serialize(testWorld.EntityManager, writer,out test);
            Debug.Log("saving");
        }
        ent.Dispose();
        testWorld.Dispose();*/

    }
}
