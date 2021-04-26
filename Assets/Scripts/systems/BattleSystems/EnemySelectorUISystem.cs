using UnityEngine.UIElements;
using Unity.Entities;
using UnityEngine;

public class EnemySelectorUISystem : SystemBase
{
    protected override void OnUpdate()
      {
            Entities
            .WithoutBurst()
            .ForEach((EnemySelectorUI enemySelectorUI,ref EnemySelectorData enemySelectorData, in CharacterStats characterStats, in SpriteRenderer sprite) => {
                VisualElement enemyPicture = enemySelectorUI.enemySelectorUI.Q<VisualElement>("EnemyPicture");
                VisualElement enemyHealthBar = enemySelectorUI.enemySelectorUI.Q<VisualElement>("EnemyHealth");
                Label enemyNameLabel = enemySelectorUI.enemySelectorUI.Q<Label>("EnemyName");

                enemyNameLabel.text = characterStats.characterName.ToString();
                enemyPicture.style.backgroundImage = Background.FromSprite(sprite.sprite);
                enemyHealthBar.style.width = (characterStats.health / characterStats.maxHealth) * enemySelectorUI.enemySelectorUI.contentRect.width;
                //Debug.Log("enemy health is currently" + enemyHealthBar.style.width);

                if (enemySelectorData.isSelected){
                    //make the enemy outline glow
                    float factor = Mathf.Pow(2, 6);
                    MaterialPropertyBlock myMatBlock = new MaterialPropertyBlock();
                    sprite.GetPropertyBlock(myMatBlock);
                    myMatBlock.SetColor("_Color", new Color(1 * factor, 1 * factor, 1 * factor));
                    sprite.SetPropertyBlock(myMatBlock);

                    enemySelectorUI.enemySelectorUI.RemoveFromClassList("enemyBase");
                    enemySelectorUI.enemySelectorUI.AddToClassList("enemySelected");
                }
                else{
                    //make the enemy not glow
                    float factor = Mathf.Pow(2, 1);
                    MaterialPropertyBlock myMatBlock = new MaterialPropertyBlock();
                    sprite.GetPropertyBlock(myMatBlock);
                    myMatBlock.SetColor("_Color", new Color(1 * factor, 1 * factor, 1 * factor));
                    sprite.SetPropertyBlock(myMatBlock);

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