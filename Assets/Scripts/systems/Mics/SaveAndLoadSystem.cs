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
        tes = World.GetOrCreateSystem<SceneSystem>();
        test = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
    }
    protected override void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            Save();
        }
    }
    public void Save(){
        /*
        using( var writer = new StreamBinaryWriter(Application.dataPath + "/save")){
            Debug.Log("saving");
            EntityManager.be
            SerializeUtility.SerializeWorld(tes.World.EntityManager, writer);
        }*/

    }
}
