using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
public class EntityPositionClip : PlayableAsset
{
    public float3 position;
    public int id;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EntityPositionBehaviour>.Create(graph);

        EntityPositionBehaviour entityPositionBehaviour = playable.GetBehaviour();
        entityPositionBehaviour.position = position;
        entityPositionBehaviour.id = id;
        return playable;
    }
}
