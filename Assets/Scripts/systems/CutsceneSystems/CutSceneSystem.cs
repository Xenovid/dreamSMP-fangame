using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
public class CutSceneSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if(EntityPlayableManager.instance != null){
           
            //transfer the data to this system
            List<PositionClip> positionClips = EntityPlayableManager.instance.positionClips;
            List<AnimationClip> animationClips = EntityPlayableManager.instance.animationClips;
            

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Translation translation, in CutsceneEntityData cutsceneEntityData) =>{
                
                foreach(PositionClip positionClip in positionClips){
                    if(positionClip.id == cutsceneEntityData.id){
                        if(!HasComponent<TransitionData>(entity)){
                            EntityManager.AddComponentData(entity, new TransitionData{oldPosition = translation.Value, newPosition = positionClip.position, duration = positionClip.duration});
                        }
                    }
                }
                foreach(AnimationClip animationClip in animationClips){
                    if(animationClip.id == cutsceneEntityData.id){
                        Animator animator = EntityManager.GetComponentObject<Animator>(entity);
                        Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                        animator.Play(animationClip.animationName);
                        
                    }
                }
            }).Run();

            EntityPlayableManager.instance.positionClips.Clear();
            EntityPlayableManager.instance.animationClips.Clear();
        }
        
    }
}
