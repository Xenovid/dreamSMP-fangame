using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class BattleMenuSystem : SystemBase
{
    public selectables currentSelection = selectables.attack;
    public Label attackLabel;
    public Label itemLabel;
    public Label runLabel;
    UIDocument UIDoc;


    protected override void OnStartRunning(){
        base.OnStartRunning();
        EntityQuery UIGroup = GetEntityQuery(typeof(UIDocument));
        UIDocument[] UIDocs = UIGroup.ToComponentArray<UIDocument>();
        UIDoc = UIDocs[0];
    }
    protected override void OnUpdate()
    {
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
        EntityManager.CompleteAllJobs();
        Entities
        .WithoutBurst()
        .ForEach((BattleData battleData, CharacterStats characterStat, UIInputData input) =>{
            switch(currentSelection){
                case selectables.attack:
                    if(input.goselected){
                        battleData.selected = selectables.attack;
                    }
                    else{
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
                    else if(input.movedown){
                        //currentSelection = selectables.attack;
                        //attackLabel.style.color = black;
                        //runLabel.style.color = grey;
                    }
                    else if(input.moveup){
                        currentSelection = selectables.attack;
                        attackLabel.style.color = black;
                        runLabel.style.color = grey;
                    }
                    break;
              }
        }).Run();
    }
    }
}

public enum selectables{
    attack,
    items,
    run,
    none
}
