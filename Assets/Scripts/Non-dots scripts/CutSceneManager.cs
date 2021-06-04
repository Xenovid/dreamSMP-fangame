using UnityEngine.Playables;
using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance;

    public PlayableAsset[] assets;
    private void Awake() {
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(this);
        }
    }
}
