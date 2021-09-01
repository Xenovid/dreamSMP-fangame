using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class AlphaTranslationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, SpriteRenderer sprite, ref AlphaTranslationData translationData) => {
            translationData.timePassed += 2 * dt;

            MaterialPropertyBlock material = new MaterialPropertyBlock();
            sprite.GetPropertyBlock(material);
            material.SetFloat("Alpha", Mathf.Lerp(translationData.a,translationData.b, translationData.timePassed));
            sprite.SetPropertyBlock(material);
            if(translationData.timePassed > 1){
                EntityManager.RemoveComponent<AlphaTranslationData>(entity);
            }
        }).Run();
    }
}
