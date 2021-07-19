using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ParticleSystemConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {

        Entities
        .ForEach((ParticleSystem particleSystem) =>
        {
            AddHybridComponent(particleSystem);
        });
    }
}
