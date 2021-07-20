using Unity.Entities;
using UnityEngine;
public class RandomAIAttackList : MonoBehaviour{
    public RandomAttack[] randomAttacks;
}
[System.Serializable]
public struct RandomAttack
{
    public SkillInfo attack;
    public float chance;
}
public struct RandomAIData : IBufferElementData
{
    public Skill attack;
    public float chance;
}
public class RandomAIConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((RandomAIAttackList attackList) => {
            Entity entity = GetPrimaryEntity(attackList);
            DynamicBuffer<RandomAIData> randomAIDatas = DstEntityManager.AddBuffer<RandomAIData>(entity);
            foreach(RandomAttack randomAttack in attackList.randomAttacks){
                randomAIDatas.Add(new RandomAIData{attack = SkillConversionSystem.SkillInfoToSkill(randomAttack.attack), chance = randomAttack.chance});
            }
        });
    }
}