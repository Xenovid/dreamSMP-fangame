using Unity.Entities;
using UnityEngine.Playables;
using UnityEngine;
public class DirectorDataAuthouring : MonoBehaviour{
    public PlayableAsset playableAsset;
}
public class DirectorData : IComponentData{
    public GameObject gameObject;
    public PlayableGraph graph;

    public Playable playable;

}
public struct PlayOnAwakeTag : IComponentData{

}
public class DirectorConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((DirectorDataAuthouring director) =>{
            PlayableGraph graph = PlayableGraph.Create();
            GameObject tempObject = new GameObject("director");
            Entity entity = GetPrimaryEntity(director);
            DstEntityManager.AddComponentObject(entity, new DirectorData{playable = director.playableAsset.CreatePlayable(graph, tempObject), graph = graph, gameObject = tempObject});
        });
    }
}

