using Unity.Entities;
using System.Reflection;
using UnityEngine;

public struct UsingSkillData : IComponentData
{
    public Entity target;
    public float timePassed;
    public int skillNumber;
}
