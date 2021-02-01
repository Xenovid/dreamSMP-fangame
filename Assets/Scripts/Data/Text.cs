using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Text : IComponentData
{
        public FixedString64 text;
}