using Unity.Collections;
using UnityEngine;
using Unity.Entities;
public class CharacterSkillAuthoring : MonoBehaviour{
    public SkillInfo skillInfo;
} 
public struct CharacterSkill : IComponentData
{
    public Skill skill;
}

public class CharacterSkillConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((CharacterSkillAuthoring characterSkill) => {
            Entity entity = GetPrimaryEntity(characterSkill);
            DstEntityManager.AddComponentData(entity, new CharacterSkill{skill = SkillConversionSystem.SkillInfoToSkill(characterSkill.skillInfo)});
        });
    }
}