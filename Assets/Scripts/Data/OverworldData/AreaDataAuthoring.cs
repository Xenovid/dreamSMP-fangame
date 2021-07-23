using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class AreaDataAuthoring : MonoBehaviour
{
    public string mainAreaSongName;
    public string areaBattleSongName;
}
public struct AreaData : IComponentData{
    public FixedString32 mainAreaSongName;
    public FixedString32 areaBattleSongName;
}
public class AreaDataConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((AreaDataAuthoring areaDataAuthoring) => {
            Entity entity = GetPrimaryEntity(areaDataAuthoring);
            DstEntityManager.AddComponentData(entity, new AreaData{mainAreaSongName = areaDataAuthoring.mainAreaSongName, areaBattleSongName = areaDataAuthoring.areaBattleSongName});
        });
    }
}
