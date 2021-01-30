using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    private static CanvasData canvasData;
    bool hello = false;

    public static void init(){
        GameObject canvas = GameObject.Find("UIDocument");
        Debug.Log("hello");
        canvasData = canvas.GetComponent<CanvasData>();
    }
    public static void changeText(){
        if(canvasData == null)
        {
            Debug.Log("did not find canvasData");
        }
        else{
            canvasData.changeVisability();
        }
    }
}
