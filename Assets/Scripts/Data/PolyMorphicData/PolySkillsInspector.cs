using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using UnityEngine;
[CustomEditor(typeof(PolySkillsAuthoring))]
public class PolySkillsInspector : Editor
{
    // based on the tutorial from https://medium.com/@trepala.aleksander/serializereference-in-unity-b4ee10274f48
    private int listLength;
    private Type[] _implementations;
    private int _implementationTypeIndex;

    public override void OnInspectorGUI()
    {
        PolySkillsAuthoring skillsAuthoring = target as PolySkillsAuthoring;

        if(skillsAuthoring == null){
            return;
        }

        if(_implementations == null || GUILayout.Button("Refresh implementations")){
            _implementations = GetImplementations<IPolySkillData>().Where(impl=>!impl.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();
        }

        EditorGUILayout.LabelField($"Found{_implementations.Length} implementations");


        _implementationTypeIndex = EditorGUILayout.Popup(new GUIContent("Implementation"),
            _implementationTypeIndex, _implementations.Select(impl => impl.FullName).ToArray());

        if (GUILayout.Button("Create instance"))
        {
            skillsAuthoring.skills.Add(new PolySkillInfo{
                skill =(IPolySkillData) Activator.CreateInstance(_implementations[_implementationTypeIndex]), 
                data = new SharedSkillData(), 
                typeID = (PolySkillData.TypeId)Enum.Parse(typeof(PolySkillData.TypeId), _implementations[_implementationTypeIndex].FullName)
            });
        }
        base.OnInspectorGUI();
    }

    private static Type[] GetImplementations<T>()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());


        var interfaceType = typeof(T);
        return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
    }

}
