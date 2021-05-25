using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using UnityEngine;

public class SaveAndLoadSystem : SystemBase
{
    ReferencedUnityObjects test;
    SceneSystem tes;
    protected override void OnStartRunning()
    {
        /*
        tes = World.GetOrCreateSystem<SceneSystem>();
        test = ScriptableObject.CreateInstance<ReferencedUnityObjects>();*/
    }
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .ForEach((Entity ent, DynamicBuffer<WeaponData> weapons) => {
            Debug.Log(ent);
        }).Run();
        /*if(Input.GetKeyDown(KeyCode.A)){
            EntityManager.CompleteAllJobs();
            Save();
        }*/
    }
    public void Save(){
        /*

        testWorld.EntityManager.MoveEntitiesFrom(World.DefaultGameObjectInjectionWorld.EntityManager);
        using( var writer = new StreamBinaryWriter(Application.dataPath + "/save")){
            Debug.Log("saving");
            SerializeUtility.SerializeWorld(testWorld.EntityManager, writer);
        }*/

    }
}
