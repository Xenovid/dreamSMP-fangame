using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;
using Unity.Collections;

public class BattleMenuSystem : SystemBase
{
    public selectables currentSelection = selectables.attack;
    public VisualElement battleUI;
    public Label attackLabel;
    public Label itemLabel;
    public Label runLabel;
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
        EntityManager.CompleteAllJobs();

        EntityQuery BattleManagerGroup = GetEntityQuery(typeof(BattleManagerTag));
        NativeArray<Entity> battleManagers = BattleManagerGroup.ToEntityArray(Allocator.Temp);
        bool isBattling = false;

        foreach(Entity entity in battleManagers){
            Debug.Log("there is a battle manager");
            isBattling = true;
        }
        if(!isBattling){
            
        }

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Color black = Color.black;
        Color grey = Color.grey;
        var rootVisualElement = UIDoc.rootVisualElement;
        if(rootVisualElement == null){
            Debug.Log("didn't find root visual element");
        }
        else{
        attackLabel = rootVisualElement.Q<Label>("AttackLabel");
        itemLabel= rootVisualElement.Q<Label>("ItemsLabel");
        runLabel = rootVisualElement.Q<Label>("RunLabel");
        battleUI = rootVisualElement.Q<VisualElement>("BattleUI");
        if(!isBattling){
            battleUI.visible = false;
        }
        //attackLabel.ToggleInClassList("test");
        //attackLabel.EnableInClassList("test", true);
        

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((ref BattleData battleData, ref CharacterStats characterStat, in Entity entity, in UIInputData input) =>{
            if(!isBattling){
                battleUI.visible = false;
                EntityManager.RemoveComponent<BattleData>(entity);
                EntityManager.RemoveComponent<UIInputData>(entity);
            }
            else{
                switch(currentSelection){
                    case selectables.attack:
                        if(input.goselected){
                            Debug.Log("selected attack");
                            battleData.selected = selectables.attack;
                            battleData.targetingId = 0;
                        }
                        else{
                            battleData.selected = selectables.none;
                            if(input.moveup){
                                currentSelection = selectables.run;
                                attackLabel.RemoveFromClassList("battle-label-selected");
                                //attackLabel.AddToClassList("battle-label");
                                //runLabel.RemoveFromClassList("battle-label");
                                runLabel.AddToClassList("battle-label-selected");
                            }
                            else if(input.movedown){
                                currentSelection = selectables.items;
                                attackLabel.RemoveFromClassList("battle-label-selected");
                                //attackLabel.AddToClassList("battle-label");
                                //itemLabel.RemoveFromClassList("battle-label");
                                itemLabel.AddToClassList("battle-label-selected");
                            }
                        }
                        break;
                    case selectables.items:
                        if(input.goselected){
                            battleData.selected = selectables.items;
                        }
                        else{
                            battleData.selected = selectables.none;
                            if(input.moveup){
                                currentSelection = selectables.attack;
                                //attackLabel.RemoveFromClassList("battle-label");
                                attackLabel.AddToClassList("battle-label-selected");
                                itemLabel.RemoveFromClassList("battle-label-selected");
                                //itemLabel.AddToClassList("battle-label");
                            }
                            else if(input.movedown){
                                currentSelection = selectables.run;
                                itemLabel.RemoveFromClassList("battle-label-selected");
                                //itemLabel.AddToClassList("battle-label");
                                //runLabel.RemoveFromClassList("battle-label");
                                runLabel.AddToClassList("battle-label-selected");
                            }
                        }
                        break;
                    case selectables.run:
                        if(input.goselected){
                            battleData.selected = selectables.run;
                        }
                        else{
                            battleData.selected = selectables.none;
                            if(input.movedown){
                                currentSelection = selectables.attack;
                                //attackLabel.RemoveFromClassList("battle-label");
                                attackLabel.AddToClassList("battle-label-selected");
                                runLabel.RemoveFromClassList("battle-label-selected");
                                //runLabel.AddToClassList("battle-label");
                            }
                            else if(input.moveup){
                                currentSelection = selectables.items;
                                runLabel.RemoveFromClassList("battle-label-selected");
                                //runLabel.AddToClassList("battle-label");
                                //itemLabel.RemoveFromClassList("battle-label");
                                itemLabel.AddToClassList("battle-label-selected");
                            }
                        }
                        
                        break;
                }
            }
        }).Run();
    }
    }
}
