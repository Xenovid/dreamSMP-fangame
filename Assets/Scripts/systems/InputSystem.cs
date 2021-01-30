using Unity.Entities;
using System;
using UnityEngine;

public class InputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .ForEach((ref MovementData move, in InputData inputData) => {
            bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
            bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
            bool isUpKeyPressed = Input.GetKey(inputData.upKey);
            bool isDownKeyPressed = Input.GetKey(inputData.downKey);

            move.direction.x = isRightKeyPressed ? 1 : 0;
            move.direction.x -= isLeftKeyPressed ? 1 : 0;
            move.direction.y = isUpKeyPressed ? 1 : 0;
            move.direction.y -= isDownKeyPressed ? 1 : 0;
         }).Run();
    }
}
