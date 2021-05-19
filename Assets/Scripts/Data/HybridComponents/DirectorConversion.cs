using Unity.Entities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DirectorConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PlayableDirector director) =>
        {
            Debug.Log("added director");
            AddHybridComponent(director);
        });
    }
}
