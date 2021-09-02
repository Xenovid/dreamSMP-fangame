using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
[TrackBindingType(typeof(EntityPlayableManager))]
[TrackClipType(typeof(EntityPositionClip))]
public class EntityPositionTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<EntityPositionMixer>.Create(graph, inputCount);
    }
}
