using Unity.Entities;
using System.Reflection;
using UnityEngine;

public struct UsingSkillData : IComponentData
{
    public float timePassed;
    public int skillNumber;
}
