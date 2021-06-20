using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;

public class OverWorldOverlay : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .ForEach((UIDocument UIDoc, ref OverworldUITag overworldUITag) =>{
            VisualElement root = UIDoc.rootVisualElement;
            if(overworldUITag.isNextToInteractive && ! overworldUITag.wasNextToInteractive){
                AudioManager.playSound("menuavailable");
                overworldUITag.wasNextToInteractive = true;
                VisualElement interactive = root.Q<VisualElement>("interactive_item_check");
                ActivateInteractiveUI(interactive);
                // activte it

            }
            else if(!overworldUITag.isNextToInteractive && overworldUITag.wasNextToInteractive){
                overworldUITag.wasNextToInteractive = false;
                VisualElement interactive = root.Q<VisualElement>("interactive_item_check");
                DeActivateInteractiveUI(interactive);
                // deactivate it
            }
            if(overworldUITag.isVisable){
                VisualElement overlay = root.Q<VisualElement>("overlay");
                overlay.visible = true;
            }
            else{
                VisualElement overlay = root.Q<VisualElement>("overlay");
                overlay.visible = false;
            }
            //resets it so that it has to still be next to an interactive item to be active
            overworldUITag.isNextToInteractive = false;
        }).Run();
    }
    public void ActivateInteractiveUI(VisualElement interative){
        interative.RemoveFromClassList("no_interactive");
        interative.AddToClassList("is_interactive");
    }
    public void DeActivateInteractiveUI(VisualElement interative){
        interative.AddToClassList("no_interactive");
        interative.RemoveFromClassList("is_interactive");
    }
}
