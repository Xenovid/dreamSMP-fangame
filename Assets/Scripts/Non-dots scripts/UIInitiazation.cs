using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class UIInitiazation : MonoBehaviour
{
    public UIDocument UIDoc;
    Button button;
    private void OnCreate() {
        UISystem uiSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<UISystem>();
        
    }

    void Update()
    {
        Debug.Log("zooom");
        Debug.Log(button == null);
        //tempcall();
    }
    public void tempcall(){
        Debug.Log("hello");
    }
}
