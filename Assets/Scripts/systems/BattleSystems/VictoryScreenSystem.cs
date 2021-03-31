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

            EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));
            UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
            UIDoc = UIDocs[0];
      }

      protected override void OnUpdate()
      {

            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            var rootVisualElement = UIDoc.rootVisualElement;
            if(rootVisualElement == null){
                  Debug.Log("didn't find root visual element");
            }
            VisualElement battleUI = rootVisualElement.Q<VisualElement>("BattleUI");
            if(battleUI.visible){
                  AudioManager.stopSong("tempBattleMusic");
                  battleUI.visible = false;
            }

            Entities
            .WithoutBurst()
            .ForEach((Entity entity, in VictoryData victoryData, in UIInputData input) => {
                  if(input.goselected){
                        ecb.RemoveComponent<UIInputData>(entity);
                        ecb.RemoveComponent<VictoryData>(entity);
                  }
            }).Run();
      }
}
