using System.Collections;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PolySkillsAuthoring : MonoBehaviour
{
    
    public List<PolySkillInfo> skills;
}
[System.Serializable]
public struct PolySkillInfo{
    [SerializeReference]
    public IPolySkillData skill;
    public SharedSkillData data;
}

