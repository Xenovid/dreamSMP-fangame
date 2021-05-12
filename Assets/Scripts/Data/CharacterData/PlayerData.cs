using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class PlayerData : IComponentData
{
    public bool isInBattle = false;
}
