using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class EntityPositionClip : PlayableAsset
{
    public Vector3 position;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EntityPositionBehaviour>.Create(graph);

        EntityPositionBehaviour entityPositionBehaviour = playable.GetBehaviour();
        entityPositionBehaviour.position = position;
        return playable;
    }
}
