using UnityEngine.UIElements;
using Unity.Entities;
using UnityEngine;

public class EnemySelectorUISystem : SystemBase
{
     protected override void OnUpdate()
      {
            Entities
            .WithoutBurst()
            .ForEach((EnemySelectorUI enemySelectorUI, in EnemySelectorData enemySelectorData) => {
                Debug.Log("test");
                if(enemySelectorData.isSelected){
                    Debug.Log(enemySelectorData.enemyId.ToString() + ": is selcted");
                    enemySelectorUI.enemySelectorUI.AddToClassList("enemySelected");
                }
                else{
                    enemySelectorUI.enemySelectorUI.RemoveFromClassList("enemySelected");
                }
            }).Run();
      }

}