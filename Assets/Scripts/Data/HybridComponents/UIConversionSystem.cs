using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;
using UnityEngine;

public class UIConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((UIDocument UIDoc) => {
            AddHybridComponent(UIDoc);
            //AddHybridComponent(inputSystem);
        });
        Entities.ForEach((InputSystemEventSystem inputSystem) => {
            AddHybridComponent(inputSystem);
        });
    }
}
