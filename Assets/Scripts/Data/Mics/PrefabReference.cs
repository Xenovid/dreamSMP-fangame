using Unity.Entities;
using Unity.Collections;
using UnityEngine;
public class PrefabReference : MonoBehaviour{
    public PrefabRefData[] prefabs;
}
[System.Serializable]
public struct PrefabRefData{
    public GameObject prefab;
    public string prefabName;
}
public struct PrefabReferenceEntity : IBufferElementData
{
    public Entity prefab;
    public FixedString32 prefabName;
}
public class PrefabReferenceConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PrefabReference prefabReference) => {
            Entity entity = GetPrimaryEntity(prefabReference);
            DynamicBuffer<PrefabReferenceEntity> prefabReferenceEntities = DstEntityManager.AddBuffer<PrefabReferenceEntity>(entity);
            foreach(PrefabRefData prefabRef in prefabReference.prefabs){
                prefabReferenceEntities.Add(new PrefabReferenceEntity{ prefab = GetPrimaryEntity(prefabRef.prefab), prefabName = prefabRef.prefabName});
            }
        });
    }
}
[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
class PrefabConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PrefabReference prefabReference) =>
        {
            foreach(PrefabRefData prefabRef in prefabReference.prefabs){
                DeclareReferencedPrefab(prefabRef.prefab);
            }
            
        });
    }
}
