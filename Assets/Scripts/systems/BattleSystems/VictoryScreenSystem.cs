using Unity.Entities;
using Unity.Physics;
using UnityEngine.UIElements;
using Unity.Transforms;
using UnityEngine;

public class VictoryScreenSystem : SystemBase
{
      UIDocument UIDoc;

      EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

      protected override void OnStartRunning(){
            base.OnStartRunning();

            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
      }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Debug.Log("should not be running");

        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument), typeof(BattleUITag));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDoc = UIDocs[0];

        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        var rootVisualElement = UIDoc.rootVisualElement;
        if (UIDoc == null || rootVisualElement == null) {
            Debug.Log("didn't find root visual element or UIDoc");
        }
        else
        {
            VisualElement battleUI = rootVisualElement.Q<VisualElement>("BattleUI");
            VisualElement itemDesc = rootVisualElement.Q<VisualElement>("Itemdesc");
            Label textBoxText = itemDesc.Q<Label>("itemTextBox");
            VisualElement enemySelector = rootVisualElement.Q<VisualElement>("EnemySelector");

            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Translation translation, ref TextBoxData textBoxData, in VictoryData victoryData, in BeforeBattleData beforeBattleData) => {
            //should read all the text of the victory data
            if (textBoxData.currentPage >= 1)
                {
                    itemDesc.visible = false;
                    ecb.RemoveComponent<TextBoxData>(entity);
                    ecb.RemoveComponent<VictoryData>(entity);
                }
                else if (input.goselected)
                {
                    if (textBoxData.isFinishedPage)
                    {
                        textBoxData.timeFromLastChar = 0.0f;
                        textBoxData.currentPage += 1;
                        textBoxData.isFinishedPage = false;
                        textBoxData.currentChar = 0;
                    }
                    else
                    {
                        string temp = "You Won";
                        textBoxData.currentChar = temp.Length - 1;
                        textBoxData.isFinishedPage = true;
                        textBoxText.text = temp;
                    }
                }
                else
                {
                    AudioManager.stopSong("tempBattleMusic");
                    itemDesc.visible = true;
                    battleUI.visible = false;
                    enemySelector.visible = false;
                    if (!textBoxData.isFinishedPage)
                    {
                        textBoxData.timeFromLastChar += deltaTime;
                    }
                    textBoxData.timeFromLastChar += deltaTime;
                    while (textBoxData.timeFromLastChar >= .1f && !textBoxData.isFinishedPage)
                    {
                        string textstring = "You Won";
                        if (textBoxData.currentChar == 0)
                        {
                            textBoxText.text = "";

                        }
                        textBoxText.text += textstring[textBoxData.currentChar];
                        textBoxData.currentChar++;
                        textBoxData.timeFromLastChar -= .1f;
                        if (textBoxData.currentChar >= textstring.Length)
                        {
                            textBoxData.isFinishedPage = true;
                        }
                    }
                }
            }).Run();
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithNone<VictoryData>()
            .ForEach((Entity entity, ref Translation translation, in BeforeBattleData beforeBattleData) =>
            {
                if (beforeBattleData.shouldChangeBack)
                {
                    translation.Value = Vector3.MoveTowards(translation.Value, beforeBattleData.previousLocation, 10 * deltaTime);
                    if (translation.Value.x == beforeBattleData.previousLocation.x && translation.Value.y == beforeBattleData.previousLocation.y)
                    {
                        InputGatheringSystem.currentInput = CurrentInput.overworld;
                        EntityManager.AddComponentData(entity, new PhysicsCollider { Value = beforeBattleData.colliderRef });
                        EntityManager.RemoveComponent<BeforeBattleData>(entity);

                    }
                    else if (HasComponent<Unity.Physics.PhysicsCollider>(entity))
                    {
                        ecb.RemoveComponent<Unity.Physics.PhysicsCollider>(entity);
                    }
                }
            }).Run();
        }
        

      }
}
