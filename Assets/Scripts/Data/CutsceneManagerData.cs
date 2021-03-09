using Unity.Entities;
using UnityEngine;

public struct CutsceneManagerData : IComponentData
{
      public float totalTime;

      public CutsceneManagerData(float totalTime = 0)
      {
            this.totalTime = totalTime;
      }
}
