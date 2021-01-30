using Unity.Entities;
using UnityEngine;

public class testSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .ForEach((in Test test) => {
            Locator.init();
            Locator.changeText();}
        ).Run();
    }
}
