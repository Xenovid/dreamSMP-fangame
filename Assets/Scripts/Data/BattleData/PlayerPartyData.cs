using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerPartyData : IBufferElementData
{
    public int playerId;
}
