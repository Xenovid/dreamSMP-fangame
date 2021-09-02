using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
[TrackBindingType(typeof(EntityPlayableManager))]
[TrackClipType(typeof(EntityAnimationClip))]
public class EntityAnimationTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<EntityAnimationMixer>.Create(graph, inputCount);
    }
}
