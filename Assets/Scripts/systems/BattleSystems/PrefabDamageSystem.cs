using Unity.Entities;
using UnityEngine;

public class PrefabDamageSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithNone<TransitionData, Animator>()
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, in PrefabDamageData damageData, in BattlePrefabData battlePrefabData) =>{
            DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(battlePrefabData.target);
            enemyDamages.Add(new DamageData{damage = damageData.deathDamage, type = damageType.physical});
            EntityManager.DestroyEntity(entity);
        }).Run();
        Entities
        .WithNone<TransitionData, PrefabDamageData>()
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity,  in BattlePrefabData battlePrefabData, in Animator animator) =>{
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("DestroyEntity")){
                EntityManager.DestroyEntity(entity);
            }
        }).Run();
        Entities
        .WithNone<TransitionData>()
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, in PrefabDamageData damageData, in BattlePrefabData battlePrefabData, in Animator animator) =>{
            DynamicBuffer<DamageData> enemyDamages = GetBuffer<DamageData>(battlePrefabData.target);
            enemyDamages.Add(new DamageData{damage = damageData.deathDamage, type = damageType.physical});
            animator.Play("exploded");

            EntityManager.RemoveComponent<PrefabDamageData>(entity);
        }).Run();
         
    }
}
