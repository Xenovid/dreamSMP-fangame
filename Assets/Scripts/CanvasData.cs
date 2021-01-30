using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class CanvasData : MonoBehaviour
{
    public VisualElement charaterText;
    
    private void OnEnable() {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        charaterText = rootVisualElement.Q<VisualElement>("characterText");
    }
    public void changeVisability(){
        charaterText.visible =true;
    }
    
}
