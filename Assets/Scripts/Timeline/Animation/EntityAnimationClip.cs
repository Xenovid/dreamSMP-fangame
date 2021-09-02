using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class EntityAnimationClip : PlayableAsset
{
    public string AnimationName;
    public int id;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EntityAnimationBehaviour>.Create(graph);

        EntityAnimationBehaviour entityAnimationBehaviour = playable.GetBehaviour();
        entityAnimationBehaviour.AnimationName = AnimationName;
        entityAnimationBehaviour.id = id;
        return playable;
    }
}
