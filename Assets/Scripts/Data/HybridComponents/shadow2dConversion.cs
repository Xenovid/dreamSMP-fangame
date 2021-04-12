using Unity.Entities;
using UnityEngine.Experimental.Rendering.Universal;

public class shadow2dConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ShadowCaster2D shadow) =>
        {
            AddHybridComponent(shadow);
        });
    }
}
