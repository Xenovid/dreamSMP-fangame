using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Playables;
using Unity.Entities;

public class EntityPlayableManager : MonoBehaviour
{
    public PlayableDirector director;
    public static EntityPlayableManager instance;
    public List<PositionClip> positionClips = new List<PositionClip>();
    public List<AnimationClip> animationClips = new List<AnimationClip>();
    public List<PlayableAsset> playableAssets;
    public bool isPlayableFinished;
    
    private void Start() {
        instance = this;
        director.stopped += ResumeWorld_OnPlayableStoped;

        positionClips.Clear();
        animationClips.Clear();
    }
    public void ResumeWorld_OnPlayableStoped(PlayableDirector director){
        isPlayableFinished = true;
    }
    public void AddPositionClip(float3 position, float duration, int id){
        PositionClip positionClip =new PositionClip{position = position, duration = duration, id = id};
        if(!positionClips.Contains(positionClip)){
            positionClips.Add(positionClip);
        }
    }
    public void AddAnimationClip(string animationName, int id){
        AnimationClip animationClip = new AnimationClip{animationName = animationName, id = id};
        if(!animationClips.Contains(animationClip)){
            animationClips.Add(animationClip);
        }
    }

    public void SetPlayableAsset(PlayableAsset playableAsset){
        director.playableAsset = playableAsset;
    }
    public void PlayPlayable(int playableNumber){
        SetPlayableAsset(playableAssets[playableNumber]);
        director.Play();
        
    }
}
[System.Serializable]
public struct AnimationClip{
    public string animationName;
    public int id;
}
[System.Serializable]
public struct PositionClip{
    public float3 position;
    public float duration;
    public int id;
}