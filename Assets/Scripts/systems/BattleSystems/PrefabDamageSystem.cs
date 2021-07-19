using Unity.Entities;
using UnityEngine;

public class PrefabDamageSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithNone<TransitionData>()
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, in PrefabDamageData damageData, in BattlePrefabData battlePrefabData, in Animator animator) =>{
            DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(battlePrefabData.target);
            enemyDamages.Add(new DamageData{damage = damageData.deathDamage, type = damageType.physical});
            EntityManager.DestroyEntity(entity);
        }).Run();
    }
}
