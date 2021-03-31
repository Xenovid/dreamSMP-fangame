using Unity.Entities;
using System;
using UnityEngine;
using Unity.Mathematics;

public class InputSystem : SystemBase
{

    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .WithNone<UIInputData, CutsceneData>()
        .ForEach((ref MovementData move, ref DelayedInputData delayInputData ,in InputData inputData) => {
            bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
            bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
            bool isUpKeyPressed = Input.GetKey(inputData.upKey);
            bool isDownKeyPressed = Input.GetKey(inputData.downKey);
            bool isSelectKeyPressed= Input.GetKey(inputData.selectKey);

            move.direction.x = isRightKeyPressed ? 1 : 0;
            move.direction.x -= isLeftKeyPressed ? 1 : 0;
            move.direction.y = isUpKeyPressed ? 1 : 0;
            move.direction.y -= isDownKeyPressed ? 1 : 0;

            if(isSelectKeyPressed && delayInputData.isSelectPressed){
                delayInputData.wasSelectPressed = true;
            }
            else if(!isSelectKeyPressed){
                delayInputData.wasSelectPressed = false;
            }
            delayInputData.isSelectPressed = isSelectKeyPressed;
         }).Run();

        Entities
        .WithoutBurst()
        .WithNone<UIInputData>()
        .ForEach((ref MovementData move, ref DelayedInputData delayedInputData, in InputData inputData, in CutsceneData cutsceneData) =>{
            move.direction = new float3(0,0,0);
            bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
            bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
            bool isUpKeyPressed = Input.GetKey(inputData.upKey);
            bool isDownKeyPressed = Input.GetKey(inputData.downKey);
            bool isSelectKeyPressed= Input.GetKey(inputData.selectKey);

            if(isSelectKeyPressed && delayedInputData.isSelectPressed){
                delayedInputData.wasSelectPressed = true;
            }
            else if(!isSelectKeyPressed){
                delayedInputData.wasSelectPressed = false;
            }
            delayedInputData.isSelectPressed = isSelectKeyPressed;
            
            if(isDownKeyPressed && delayedInputData.isDownPressed){
                delayedInputData.wasDownPressed = true;
            }
            else if(!isDownKeyPressed){
                delayedInputData.wasDownPressed = false;
            }
            delayedInputData.isDownPressed = isDownKeyPressed;

            if(isLeftKeyPressed && delayedInputData.isLeftPressed){
                delayedInputData.wasLeftPressed = true;
            }
            else if(!isLeftKeyPressed){
                delayedInputData.wasLeftPressed = false;
            }
            delayedInputData.isLeftPressed = isLeftKeyPressed;

            if(isRightKeyPressed && delayedInputData.isRightPressed){
                delayedInputData.wasRightPressed = true;
            }
            else if(!isRightKeyPressed){
                delayedInputData.wasRightPressed = false;
            }
            delayedInputData.isRightPressed = isRightKeyPressed;

            if(isUpKeyPressed && delayedInputData.isUpPressed){
                delayedInputData.wasUpPressed = true;
            }
            else if(!isUpKeyPressed){
                delayedInputData.wasUpPressed = false;
            }
            delayedInputData.isUpPressed = isUpKeyPressed;

        }).Run();

         Entities
         .WithoutBurst()
         .ForEach((ref DelayedInputData delayedInputData, ref UIInputData uIInputData, in InputData inputData) => {
            bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
            bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
            bool isUpKeyPressed = Input.GetKey(inputData.upKey);
            bool isDownKeyPressed = Input.GetKey(inputData.downKey);
            bool isSelectKeyPressed= Input.GetKey(inputData.selectKey);
            bool isBackKeyPressed = Input.GetKey(inputData.backKey);

            if(isSelectKeyPressed && delayedInputData.isSelectPressed){
                delayedInputData.wasSelectPressed = true;
            }
            else if(!isSelectKeyPressed){
                delayedInputData.wasSelectPressed = false;
            }
            delayedInputData.isSelectPressed = isSelectKeyPressed;
            
            if(isDownKeyPressed && delayedInputData.isDownPressed){
                delayedInputData.wasDownPressed = true;
            }
            else if(!isDownKeyPressed){
                delayedInputData.wasDownPressed = false;
            }
            delayedInputData.isDownPressed = isDownKeyPressed;

            if(isLeftKeyPressed && delayedInputData.isLeftPressed){
                delayedInputData.wasLeftPressed = true;
            }
            else if(!isLeftKeyPressed){
                delayedInputData.wasLeftPressed = false;
            }
            delayedInputData.isLeftPressed = isLeftKeyPressed;

            if(isRightKeyPressed && delayedInputData.isRightPressed){
                delayedInputData.wasRightPressed = true;
            }
            else if(!isRightKeyPressed){
                delayedInputData.wasRightPressed = false;
            }
            delayedInputData.isRightPressed = isRightKeyPressed;

            if(isUpKeyPressed && delayedInputData.isUpPressed){
                delayedInputData.wasUpPressed = true;
            }
            else if(!isUpKeyPressed){
                delayedInputData.wasUpPressed = false;
            }
            delayedInputData.isUpPressed = isUpKeyPressed;
            
            if(isBackKeyPressed && delayedInputData.isBackPressed){
                delayedInputData.wasBackPressed = true;
            }
            else if(!isBackKeyPressed){
                delayedInputData.wasBackPressed = false;
            }
            delayedInputData.isBackPressed = isBackKeyPressed;


            uIInputData.goselected = (isSelectKeyPressed && !delayedInputData.wasSelectPressed) && !isBackKeyPressed;
            uIInputData.goback = (isBackKeyPressed && !delayedInputData.wasSelectPressed) && ! isSelectKeyPressed;

            uIInputData.moveup = isUpKeyPressed && !isDownKeyPressed && !delayedInputData.wasUpPressed;
            uIInputData.movedown = isDownKeyPressed && !isUpKeyPressed && !delayedInputData.wasDownPressed;

            uIInputData.moveleft = isLeftKeyPressed && !isRightKeyPressed && !delayedInputData.wasLeftPressed;
            uIInputData.moveright = isRightKeyPressed && !isLeftKeyPressed && !delayedInputData.wasRightPressed;

            //move.direction = new float3(0, 0, 0);
         }).Run();
    }
}
