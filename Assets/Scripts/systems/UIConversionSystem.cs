using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class UIConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((UIDocument UIDoc, EventSystem eventSystem) => {
            AddHybridComponent(UIDoc);
            AddHybridComponent(eventSystem);
        });
        
    }
}
