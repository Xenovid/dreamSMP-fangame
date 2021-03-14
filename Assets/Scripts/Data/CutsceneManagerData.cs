using Unity.Entities;
using UnityEngine;

public struct CutsceneManagerData : IComponentData
{
      public float totalTime;
      public float dialogueLength;

      public CutsceneManagerData(float totalTime = 0, float dialogueLength = 0)
      {
            this.dialogueLength = dialogueLength;
            this.totalTime = totalTime;
      }
}
