using UnityEngine.UIElements;
using Unity.Entities;
using UnityEngine;

public class EnemySelectorUISystem : SystemBase
{
    BattleSystem battleSystem;
    //UIDocument UIDoc;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnStartRunning(){
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleEnd += RemoveSelectorUI_OnBattleEnd;

        EntityQuery UIDocumentGroup = GetEntityQuery(typeof(UIDocument), typeof(UITag));
        //UIDoc = UIDocumentGroup.ToComponentArray<UIDocument>()[0];
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
      {
          var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            Entities
            .WithoutBurst()
            .ForEach((Entity entity,EnemySelectorUI enemySelectorUI,ref EnemySelectorData enemySelectorData, in CharacterStats characterStats, in AnimationData animationData, in SpriteRenderer sprite, in Animator animator) => {
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
                    myMatBlock.SetInt("IsSelected", 1);
                    sprite.SetPropertyBlock(myMatBlock);

                }
                else{
                    //make the enemy not glow
                    float factor = Mathf.Pow(2, 1);
                    MaterialPropertyBlock myMatBlock = new MaterialPropertyBlock();
                    sprite.GetPropertyBlock(myMatBlock);
                    myMatBlock.SetInt("IsSelected", 0);
                    //myMatBlock.SetColor("_Color", new Color(1 * factor, 1 * factor, 1 * factor));
                    sprite.SetPropertyBlock(myMatBlock);

                }
                if(characterStats.health <= 0){
                    enemySelectorData.isDead = true;
                    MaterialPropertyBlock myMatBlock = new MaterialPropertyBlock();
                    myMatBlock.SetInt("IsSelected", 0);
                    animator.Play(animationData.characterDownAnimationName);
                    
                    enemySelectorUI.enemySelectorUI.SetEnabled(false);
                }
            }).Run();
      }
      private void RemoveSelectorUI_OnBattleEnd(System.Object sender, System.EventArgs e){
        EntityQuery UIDocumentGroup = GetEntityQuery(typeof(UIDocument), typeof(UITag));
        UIDocument UIDoc = UIDocumentGroup.ToComponentArray<UIDocument>()[0];
        VisualElement root = UIDoc.rootVisualElement;
            VisualElement enemySelector = root.Q<VisualElement>("EnemySelector");
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, EnemySelectorUI enemySelectorUI,ref EnemySelectorData enemySelectorData, in HeadsUpUIData headsUpUIData) => {
                enemySelectorUI.enemySelectorUI.parent.Remove(enemySelectorUI.enemySelectorUI);
                foreach(Message message in headsUpUIData.messages){
                    message.label.parent.Remove(message.label);
                }
                headsUpUIData.UI.parent.Remove(headsUpUIData.UI);
                
                
                //enemySelector.Remove(enemySelectorUI.enemySelectorUI);
                EntityManager.RemoveComponent<HeadsUpUIData>(entity);
                EntityManager.RemoveComponent<EnemySelectorUI>(entity);
                EntityManager.RemoveComponent<EnemySelectorData>(entity);
            }).Run();
            
            
      }

}