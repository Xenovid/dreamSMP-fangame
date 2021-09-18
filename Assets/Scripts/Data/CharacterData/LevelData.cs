using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
[GenerateAuthoringComponent]
[System.Serializable]
public struct LevelData : IComponentData
{
    public int LevelCAP;
    public int currentLVL;
    public int currentEXP;
    public int requiredEXP;
}
