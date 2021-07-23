using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class EnemyBattleDataAuthoring : MonoBehaviour{
    public GameObject[] enemies;
}
public struct EnemyBattleData : IBufferElementData
{
    public Entity enemyEntity;
}

public class EnemyBattleDataConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((EnemyBattleDataAuthoring enemyBattleData) => {
            Entity entity = GetPrimaryEntity(enemyBattleData);
            DynamicBuffer<EnemyBattleData> enemyBattleDatas = DstEntityManager.AddBuffer<EnemyBattleData>(entity);
            foreach(GameObject gameObject in enemyBattleData.enemies){
                enemyBattleDatas.Add(new EnemyBattleData{enemyEntity = GetPrimaryEntity(gameObject)});
            }
        });
    }
}


