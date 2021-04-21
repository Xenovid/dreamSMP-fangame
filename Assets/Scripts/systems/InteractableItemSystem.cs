using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

public class InteractableItemSystem : SystemBase
{

    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();


        if (input.select)
        {

            Entities
            .WithoutBurst()
            .WithAll<PlayerTag>()
            .ForEach((ref Translation playerTranslation ,in MovementData move) =>
            {
                float3 playerPosition = playerTranslation.Value;
                Direction playerDirection = move.facing;
            
                Entities
                .WithoutBurst()
                .ForEach((in IteractiveItemData iteractiveItem, in DynamicBuffer<Text> text, in Translation interactiveTranslation) =>
                {
                    float3 interativePosition = interactiveTranslation.Value;
                    if(math.distance(playerPosition, interativePosition) < 0.7f)
                    {
                        switch(playerDirection)
                        {
                            case Direction.up:
                                if(playerPosition.y - interativePosition.y < 0)
                                {
                                    Debug.Log("found iteractive object above");
                                }
                                break;
                            case Direction.down:
                                if(playerPosition.y - interativePosition.y > 0)
                                {
                                    Debug.Log("found interative object below");
                                }
                                break;
                            case Direction.left:
                                if(playerPosition.x - interativePosition.x > 0)
                                {
                                    Debug.Log("found interative object to the left");
                                }
                                break;
                            case Direction.right:
                                if(playerPosition.x - interativePosition.x < 0)
                                {
                                    Debug.Log("found interative object to the right");
                                }
                                break;
                        }
                    }
                }).Run();
            }
        ).Run();
        }
    }

}
