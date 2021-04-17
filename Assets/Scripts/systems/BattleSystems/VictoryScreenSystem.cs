using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class VictoryScreenSystem : SystemBase
{
      UIDocument UIDoc;

      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

      protected override void OnStartRunning(){
            base.OnStartRunning();

            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument), typeof(BattleUITag));
            UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
            UIDoc = UIDocs[0];
      }

      protected override void OnUpdate()
      {
            EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
            UIInputData input = uiInputQuery.GetSingleton<UIInputData>();
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            var rootVisualElement = UIDoc.rootVisualElement;
            if(rootVisualElement == null){
                  Debug.Log("didn't find root visual element");
            }
            VisualElement battleUI = rootVisualElement.Q<VisualElement>("BattleUI");
            VisualElement itemDesc = rootVisualElement.Q<VisualElement>("Itemdesc");
            VisualElement enemySelector = rootVisualElement.Q<VisualElement>("EnemySelector");
            VisualElement victoryScreen = rootVisualElement.Q<VisualElement>("VictoryScreen");

            if(battleUI.visible){
                  AudioManager.stopSong("tempBattleMusic");
                  victoryScreen.visible = true;
                  battleUI.visible = false;
                  itemDesc.visible = false;
            }
            else if(enemySelector.visible){
                  AudioManager.stopSong("tempBattleMusic");
                  victoryScreen.visible = true;
                  enemySelector.visible = false;
            }

            Entities
            .WithoutBurst()
            .ForEach((Entity entity, in VictoryData victoryData) => {
                  if(input.goselected){
                        victoryScreen.visible = false;
                        InputGatheringSystem.currentInput = CurrentInput.overworld;
                        ecb.RemoveComponent<VictoryData>(entity);
                  }
            }).Run();
      }
}
