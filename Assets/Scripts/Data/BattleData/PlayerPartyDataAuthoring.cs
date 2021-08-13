using Unity.Entities;
using UnityEngine;
public class PlayerPartyDataAuthoring : MonoBehaviour{
    public GameObject[] players;
}
public struct PlayerPartyData : IBufferElementData
{
    public Entity player;
}

public class PlayerPartyConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PlayerPartyDataAuthoring playerPartyDataAuthoring) => {
            Entity entity = GetPrimaryEntity(playerPartyDataAuthoring);
            DynamicBuffer<PlayerPartyData> players = DstEntityManager.AddBuffer<PlayerPartyData>(entity);
            foreach(GameObject gameObject in playerPartyDataAuthoring.players){
                players.Add(new PlayerPartyData{player = GetPrimaryEntity(gameObject)});
            }
        }); 
    }
}
