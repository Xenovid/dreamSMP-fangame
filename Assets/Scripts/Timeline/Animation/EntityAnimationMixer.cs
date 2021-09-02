using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class EntityAnimationMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData){
       EntityPlayableManager manager= playerData as EntityPlayableManager;

       if(!manager)return;

       int inputCount = playable.GetInputCount();
       for(int i = 0; i < inputCount; i++){
           float inputWeight = playable.GetInputWeight(i);
           //playable.
           if(inputWeight > 0f){
               ScriptPlayable<EntityAnimationBehaviour> inputPlayable = (ScriptPlayable<EntityAnimationBehaviour>)playable.GetInput(i);
               

               EntityAnimationBehaviour input = inputPlayable.GetBehaviour();
               
               

               manager.AddAnimationClip(input.AnimationName,  input.id);
           }
       }

       
    } 
}
