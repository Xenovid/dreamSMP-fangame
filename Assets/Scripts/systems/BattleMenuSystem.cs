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
        BattleManagerTag battleManager;
        bool isBattling = false;

        foreach(Entity entity in battleManagers){
            Debug.Log("there is a battle manager");
            isBattling = true;
        }
        if(!isBattling){
            battleUI.visible = false;
        }
        battleManagers.Dispose();

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Color black = Color.black;
        Color grey = Color.grey;
        var rootVisualElement = UIDoc.rootVisualElement;
        if(rootVisualElement == null){
            Debug.Log("didn't find root visual element");
        }
        else{
        attackLabel = rootVisualElement.Q<Label>("AttackLabel");
        itemLabel = rootVisualElement.Q<Label>("ItemLabel");
        runLabel = rootVisualElement.Q<Label>("RunLabel");
        battleUI = rootVisualElement.Q<VisualElement>("battleUI");

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((ref BattleData battleData, ref CharacterStats characterStat, in Entity entity, in UIInputData input) =>{
            if(!isBattling){
                battleUI.visible = false;
                Debug.Log(battleUI.visible);
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
                            if(input.moveright){
                                currentSelection = selectables.items;
                                attackLabel.style.color = grey;
                                itemLabel.style.color = black;
                            }
                            else if(input.movedown){
                                currentSelection = selectables.run;
                                attackLabel.style.color = grey;
                                runLabel.style.color = black;
                            }
                        }
                        break;
                    case selectables.items:
                        if(input.goselected){
                            battleData.selected = selectables.items;
                        }
                        else{
                            battleData.selected = selectables.none;
                            if(input.moveleft){
                                currentSelection = selectables.attack;
                                attackLabel.style.color = black;
                                itemLabel.style.color = grey;
                            }
                            else if(input.movedown){
                                //currentSelection = selectables.run;
                                //itemLabel.style.color = grey;
                                //runLabel.style.color = black;
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
                            //currentSelection = selectables.attack;
                            //attackLabel.style.color = black;
                            //runLabel.style.color = grey;
                            }
                            else if(input.moveup){
                                currentSelection = selectables.attack;
                                attackLabel.style.color = black;
                                runLabel.style.color = grey;
                            }
                        }
                        
                        break;
                }
            }
        }).Run();
    }
    }
}
