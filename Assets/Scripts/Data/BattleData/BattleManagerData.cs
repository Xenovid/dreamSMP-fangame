using Unity.Entities;
using UnityEngine;
public class BattleManagerData : IComponentData {
    public float translationTimePast;

    public bool hasPlayerWon;
}