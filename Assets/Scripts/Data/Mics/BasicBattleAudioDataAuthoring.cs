using Unity.Entities;
using UnityEngine;
using Unity.Collections;
public class BasicBattleAudioDataAuthoring : MonoBehaviour
{
    public string hitSoundName;
    public string attackSoundName;
}
public struct BasicBattleAudioData : IComponentData
{
    public FixedString32 hitSoundName;
    public FixedString32 attackSoundName;
}

public class BasicBattleAudioConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((BasicBattleAudioDataAuthoring audioData) => {
            Entity entity = GetPrimaryEntity(audioData);
            DstEntityManager.AddComponentData(entity, new BasicBattleAudioData{hitSoundName = audioData.hitSoundName, attackSoundName = audioData.attackSoundName});
        });
    }
}
