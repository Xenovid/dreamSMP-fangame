using UnityEngine.UIElements;
using Unity.Entities;
using UnityEngine;

public class EnemySelectorUISystem : SystemBase
{
     protected override void OnUpdate()
      {
            Entities
            .WithoutBurst()
            .ForEach((EnemySelectorUI enemySelectorUI,ref EnemySelectorData enemySelectorData, in CharacterStats characterStats) => {

                VisualElement enemyPicture = enemySelectorUI.enemySelectorUI.Q<VisualElement>("EnemyPicture");
                Debug.Log("test");
                if(enemySelectorData.isSelected){
                    enemySelectorUI.enemySelectorUI.RemoveFromClassList("enemyBase");
                    enemySelectorUI.enemySelectorUI.AddToClassList("enemySelected");
                }
                else{
                    enemySelectorUI.enemySelectorUI.RemoveFromClassList("enemySelected");
                     enemySelectorUI.enemySelectorUI.AddToClassList("enemyBase");
                }
                if(characterStats.health < 0){
                    enemyPicture.AddToClassList("enemyDown");
                    enemySelectorData.isDead = true;
                    enemySelectorUI.enemySelectorUI.AddToClassList("enemyDown");
                    enemySelectorUI.enemySelectorUI.RemoveFromClassList("enemySelected");
                    enemySelectorUI.enemySelectorUI.RemoveFromClassList("enemyBase");
                }
            }).Run();
      }

}