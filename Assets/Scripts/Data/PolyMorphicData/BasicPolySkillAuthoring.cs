using Unity.Entities;
using UnityEngine;

public class BasicPolySkillAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public BasicPolySkill skill;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
    }
}
