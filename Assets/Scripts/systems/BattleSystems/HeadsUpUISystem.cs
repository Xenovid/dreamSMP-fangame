using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;
using UnityEngine;

public class HeadsUpUISystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities
        .WithoutBurst()
        .ForEach((HeadsUpUIData headsUpUI) => {
            //go through each message and move its message
            for(int i = 0; i < headsUpUI.messages.Count; i++){
                Message message = headsUpUI.messages[i];
                message.timePassed += dt;
                headsUpUI.messages[i] = message;
                if(message.timePassed > 2){
                    headsUpUI.UI.Q<VisualElement>("messages").Remove(message.label);
                    headsUpUI.messages.RemoveAt(i);
                    i--;
                }
                else if(message.timePassed > 1){
                    headsUpUI.messages[i].label.style.opacity = math.lerp(1, 0, message.timePassed - 1); 
                }
                else{
                    Vector3 newPosition = headsUpUI.messages[i].label.transform.position;
                    newPosition.y += 20 * dt * headsUpUI.messages[i].direction.y;
                    newPosition.x += 20 * dt * headsUpUI.messages[i].direction.x;
                    headsUpUI.messages[i].label.transform.position = newPosition;
                }
            }
        }).Run();
    }
}
