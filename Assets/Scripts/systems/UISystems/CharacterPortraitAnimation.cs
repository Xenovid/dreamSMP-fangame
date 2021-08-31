using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
public class CharacterPortraitAnimation : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .ForEach((UIAnimationData animationData) =>{
            if(animationData.active && animationData.sprites.Length != 0){
                animationData.time += dt;
                if (animationData.time > animationData.initialDelay){
                    if(animationData.time - animationData.initialDelay > animationData.spritePerSecond){
                        if(animationData.index == animationData.sprites.Length -1){
                            animationData.index = 0;
                            animationData.time = 0;
                            animationData.visualElement.style.backgroundImage = Background.FromSprite(animationData.sprites[animationData.index]);
                        }
                        else if(!( animationData.index >= animationData.sprites.Length)){
                            animationData.index++;
                            animationData.visualElement.style.backgroundImage = Background.FromSprite(animationData.sprites[animationData.index]);
                            animationData.time -= animationData.spritePerSecond;
                        }
                    }
                }
            } 
        }).Run();
    }

}

public class UIAnimationData : IComponentData{
    public VisualElement visualElement;
    public Sprite[] sprites;
    public float initialDelay;
	public bool active;
	public float time;
    public int index;
    public float spritePerSecond;
}
