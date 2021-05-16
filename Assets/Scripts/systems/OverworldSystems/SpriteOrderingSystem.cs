using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpriteOrderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .ForEach((SpriteRenderer spriteRenderer, in Translation translation) => {
            spriteRenderer.sortingOrder = (int)translation.Value.y * -10;
        }).Run();
    }
}
