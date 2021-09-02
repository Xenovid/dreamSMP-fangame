using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
public class EntityPositionMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData){
       EntityPlayableManager manager= playerData as EntityPlayableManager;

       float3 currentPosition = new float3();

       if(!manager)return;

       int inputCount = playable.GetInputCount();
       for(int i = 0; i < inputCount; i++){
           float inputWeight = playable.GetInputWeight(i);
           //playable.
           if(inputWeight > 0f){
               ScriptPlayable<EntityPositionBehaviour> inputPlayable = (ScriptPlayable<EntityPositionBehaviour>)playable.GetInput(i);
               

               EntityPositionBehaviour input = inputPlayable.GetBehaviour();
               
               currentPosition = input.position;

               manager.AddPositionClip(input.position, (float)inputPlayable.GetDuration(), input.id);
           }
       }

       
    } 
}
