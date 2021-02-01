using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    private static CanvasData canvasData;
    private static int numCanvasData = 0;
    GameObject currentCanvasData;
    bool hello = false;

    private void Awake(){
        GameObject canvas = GameObject.Find("UIDocument");
        canvasData = canvas.GetComponent<CanvasData>();
        if(numCanvasData == 0){
            currentCanvasData = this.gameObject;
            numCanvasData++;
        }
        else{
            Debug.Log("there is already a canvasData");
        }
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
