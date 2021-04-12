using Unity.Entities;
using UnityEngine;

public class SpriteConvertionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {

        Entities
        .ForEach((SpriteRenderer sprite, Animator anim) =>
        {
            AddHybridComponent(sprite);
            AddHybridComponent(anim);
        });
        Entities
        .WithNone<Animator>()
        .ForEach((SpriteRenderer sprite) =>
        {
            AddHybridComponent(sprite);
        });
    }
}
