using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
[CustomEditor(typeof(PolyInteractiveAuthoring))]
public class PolyInteractiveInspector : Editor
{
    private Type[] _implementations;
    private int _implementationTypeIndex;

    public override void OnInspectorGUI()
    {
        PolyInteractiveAuthoring interactiveAuthoring = target as PolyInteractiveAuthoring;

        if(interactiveAuthoring == null){
            return;
        }

        if(_implementations == null || GUILayout.Button("Refresh implementations")){
            _implementations = GetImplementations<IPolyInteractiveData>().Where(impl=>!impl.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();
        }

        EditorGUILayout.LabelField($"Found{_implementations.Length} implementations");


        _implementationTypeIndex = EditorGUILayout.Popup(new GUIContent("Implementation"),
            _implementationTypeIndex, _implementations.Select(impl => impl.FullName).ToArray());

        if (GUILayout.Button("Create instance"))
        {
            var interactive = (IPolyInteractiveData)Activator.CreateInstance(_implementations[_implementationTypeIndex]);

            interactiveAuthoring.interactiveData = interactive;
            interactiveAuthoring.typeId = (PolyInteractiveData.TypeId)Enum.Parse(typeof(PolyInteractiveData.TypeId), _implementations[_implementationTypeIndex].FullName);
            //shared data always stays the same so it shouldn't matter
            //interactiveAuthoring.sharedInteractiveData = new
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
